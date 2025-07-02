using Accounts.Application.DTOs.Requests.Auth;
using Accounts.Application.DTOs.Requests.User;
using Accounts.Application.Interfaces.Persistence.Queries;
using Accounts.Application.Interfaces.Services;
using Accounts.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constants;

namespace Accounts.API.Controllers;

[ApiController]
public class AuthController(IAuthService authService, IUserVerificationService userVerificationService,
    IUserQueries userQueries, IUserService userService) : ControllerBase
{
    [HttpPost(ApiEndpoints.Accounts.Auth.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        // 1. Register the user and ensure no error occurred during registration.
        await authService.RegisterAsync(request, cancellationToken);
    
        // 2. Fetch the user by email to ensure the user exists after registration.
        var userDto = await userQueries.GetByEmailAsync(request.Email, cancellationToken);
    
        // 3. If no user is found after registration, return an error response.
        if (userDto == null)
            return BadRequest(new { error = "User not found after registration." });

        // 4. Generate a verification code for email confirmation.
        var generateResult = await userVerificationService.GenerateVerificationCodeAsync(
            userDto.Id, VerificationType.EmailConfirmation, cancellationToken);

        if (!generateResult.IsSuccess)
            return BadRequest(new { error = generateResult.Error });

        // 5. Send the verification code to the user's email.
        var sendResult = await userVerificationService.SendVerificationCodeAsync(
            userDto.Id, userDto.Email.Value, VerificationType.EmailConfirmation, cancellationToken);

        if (!sendResult.IsSuccess)
            return BadRequest(new { error = sendResult.Error });

        // 6. Respond with a success message indicating the user needs to verify their email.
        return Ok(new
        {
            message = "Registration successful. Please verify your email to activate your account."
        });
    }

    [HttpPost(ApiEndpoints.Accounts.Auth.Login)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        var authResult = await authService.LoginAsync(request, cancellationToken);

        if (!authResult.IsEmailConfirmed)
        {
            return Unauthorized(new
            {
                error = "Email not confirmed. Please check your email for the verification code."
            });
        }

        return Ok(authResult);
    }

    [HttpPost(ApiEndpoints.Accounts.Auth.RefreshToken)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(request, cancellationToken);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost(ApiEndpoints.Accounts.Auth.Logout)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await authService.LogoutAsync(cancellationToken);
        return Ok(new { message = "Logged out successfully" });
    }
    
    [HttpPost(ApiEndpoints.Accounts.Auth.ResendVerificationEmail)]
    public async Task<IActionResult> Resend([FromBody] ResendCodeRequest request, CancellationToken cancellationToken)
    {
        var result = await userVerificationService.ResendVerificationCodeAsync(
            request.UserId, request.Email, VerificationType.EmailConfirmation, cancellationToken);

        return result.IsSuccess
            ? Ok(new { message = "Verification code sent again." })
            : BadRequest(new { error = result.Error });
    }
    
    [HttpPost(ApiEndpoints.Accounts.Auth.VerifyEmail)]
    public async Task<IActionResult> Verify([FromBody] VerifyCodeRequest request, CancellationToken cancellationToken)
    {
        var result = await userVerificationService.VerifyCodeAsync(
            request.UserId, request.Code, VerificationType.EmailConfirmation, cancellationToken);

        return result.IsSuccess
            ? Ok(new { message = result.Error ?? "Email confirmed." })
            : BadRequest(new { error = result.Error });
    }
    
    [HttpPost(ApiEndpoints.Accounts.Auth.ChangePassword)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await userService.ChangePasswordAsync(request.OldPassword, request.NewPassword, cancellationToken);

        return result.IsSuccess ? Ok(new { message = result.Error }) : BadRequest(new { error = result.Error });
    }

    [HttpPost(ApiEndpoints.Accounts.Auth.ForgotPassword)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await userService.ForgotPasswordAsync(request.Email, cancellationToken);

        return result.IsSuccess ? Ok(new { message = result.Error }) : BadRequest(new { error = result.Error });
    }

    [HttpPost(ApiEndpoints.Accounts.Auth.ChangeEmail)]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request, CancellationToken cancellationToken)
    {
        
        var result = await userService.ChangeEmailAsync(request.NewEmail, request.Password, cancellationToken);

        return result.IsSuccess ? Ok(new { message = result.Error }) : BadRequest(new { error = result.Error });
    }
    
}