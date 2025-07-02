using Accounts.Application.DTOs.Requests.User;
using SharedKernel.Results;

namespace Accounts.Application.Interfaces.Services;

public interface IUserService
{
    Task<Result> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> AssignRolesAsync(Guid userId, List<Guid> roleIds, CancellationToken cancellationToken);
    Task<Result> ChangePasswordAsync(string oldPassword, string newPassword, CancellationToken cancellationToken);
    Task<Result> ForgotPasswordAsync(string email, CancellationToken cancellationToken);
    Task<Result> ChangeEmailAsync(string newEmail, string password, CancellationToken cancellationToken);
}
