namespace SharedKernel.DTOs;

public class JwtUserData
{
    public Guid Id { get; init; }
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public List<string> Roles { get; init; } = new();
    public List<string> Permissions { get; set; } = new();

}