using Accounts.Application.DTOs.Requests.User;
using Accounts.Application.DTOs.Responses;
using Accounts.Application.Interfaces.Persistence;
using Accounts.Application.Interfaces.Persistence.Queries;
using Accounts.Application.Interfaces.Persistence.Repositories;
using Accounts.Application.Interfaces.Services;
using Accounts.Domain.Users;
using Accounts.Domain.ValueObjects;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Abstractions.Auth;
using SharedKernel.Abstractions.DateTime;
using SharedKernel.Configuration;
using SharedKernel.DTOs;
using System.Security.Claims;
using Accounts.Application.Mapping;
using SharedKernel.Exceptions;

namespace Accounts.Application.Services;

public class AuthService(IUserRepository userRepository, IUserQueries userQueries, IRefreshTokenRepository refreshTokenRepository, IRefreshTokenQueries refreshTokenQueries,
    IUnitOfWork unitOfWork, IJwtService jwtService, IDateTimeProvider dateTimeProvider, IMapper mapper, ILogger<AuthService> logger,
    ICurrentRequestContext requestContext, IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Registering new user with Email: {Email}", request.Email);

        var existingUser = await userQueries.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
            throw new ConflictException("Email already registered");

        var passwordHash = PasswordHash.FromPlainText(request.Password);
        var user = new User(Guid.NewGuid(), request.Username, request.Email, passwordHash, request.FullName);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var jwtUser = mapper.Map<JwtUserData>(user);
        var accessToken = jwtService.GenerateAccessToken(jwtUser);
        var refreshToken = jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken(refreshToken, dateTimeProvider.UtcNow.AddDays(7), requestContext.IpAddress, user.Id);
        await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} registered successfully", user.Id);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            IsEmailConfirmed = user.IsEmailConfirmed
        };
    }
    public async Task<AuthResponse> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Login requested for Email: {Email}", request.Email);

        var userDto = await userQueries.GetByEmailAsync(request.Email, cancellationToken);
        if (userDto is null || !userDto.PasswordHash.Verify(request.Password))
        {
            logger.LogWarning("Invalid login attempt for email: {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // التحويل هنا باستخدام ToDomain
        var user = userDto.ToDomain();

        var jwtUser = mapper.Map<JwtUserData>(user);
        var accessToken = jwtService.GenerateAccessToken(jwtUser);
        var refreshToken = jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken(refreshToken, dateTimeProvider.UtcNow.AddDays(7), requestContext.IpAddress, user.Id);
        await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            IsEmailConfirmed = user.IsEmailConfirmed // تم استخدام الكائن الدومين هنا
        };
    }
    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Refresh token attempt");

        var principal = jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var oldToken = await refreshTokenQueries.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (oldToken is null || !oldToken.IsActive || oldToken.UserId != userId)
        {
            logger.LogWarning("Invalid refresh token for user: {UserId}", userId);
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        await refreshTokenRepository.RevokeTokenAsync(request.RefreshToken, requestContext.IpAddress, cancellationToken);

        var userDto = await userQueries.GetByIdAsync(userId, cancellationToken);
        if (userDto is null)
            throw new UnauthorizedAccessException("User not found");

        // التحويل هنا باستخدام ToDomain
        var user = userDto.ToDomain();

        var jwtUser = mapper.Map<JwtUserData>(user);
        var newAccessToken = jwtService.GenerateAccessToken(jwtUser);
        var newRefreshToken = jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken(newRefreshToken, dateTimeProvider.UtcNow.AddDays(7), requestContext.IpAddress, userId);
        await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            IsEmailConfirmed = user.IsEmailConfirmed // تم استخدام الكائن الدومين هنا
        };
    }
    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Logging out user");

        var userId = requestContext.UserId;
        if (userId is null)
        {
            logger.LogWarning("Attempted logout without user context");
            throw new UnauthorizedAccessException("User not authenticated");
        }

        await refreshTokenRepository.RevokeAllTokensForUserAsync(userId.Value, requestContext.IpAddress, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User logged out: {UserId}", userId);
    }
}