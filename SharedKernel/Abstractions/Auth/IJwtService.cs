using System.Security.Claims;
using SharedKernel.DTOs;

namespace SharedKernel.Abstractions.Auth;

public interface IJwtService
{
    string GenerateAccessToken(JwtUserData user);
    
    string GenerateRefreshToken();
    
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}