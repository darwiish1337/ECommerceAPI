using Accounts.Application.DTOs;

namespace Accounts.Application.Interfaces.Persistence.Queries;

public interface IUserQueries
{
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<UserDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    
    Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);
    
    Task<UserDto?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default);
}
