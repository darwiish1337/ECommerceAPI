namespace SharedKernel.Abstractions.Auth;

public interface ICurrentRequestContext
{
    string IpAddress { get; }
    string? UserAgent { get; }
    Guid? UserId { get; } 
    string? TenantId { get; } 
}
