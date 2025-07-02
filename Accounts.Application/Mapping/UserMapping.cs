using Accounts.Application.DTOs;
using Accounts.Domain.Users;
using Accounts.Domain.ValueObjects;

namespace Accounts.Application.Mapping;

public static class UserMapping
{
    public static User ToDomain(this UserDto dto)
    {
        var user = new User(
            dto.Id,
            PasswordHash.FromHashed(dto.PasswordHash.Value),
            Email.FromTrustedSource(dto.Email.Value),
            dto.Username,
            dto.FullName
        );

        if (dto.Roles.Any())
        {
            var roleEntities = dto.Roles.Select(name => new Role(Guid.NewGuid(), name)).ToList(); // لو عندك الـ IDs في الأصل عدلها
            user.AssignRoles(roleEntities);
        }

        return user;
    }
}