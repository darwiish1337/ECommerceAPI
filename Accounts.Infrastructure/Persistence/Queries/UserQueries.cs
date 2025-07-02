using System.Data;
using Accounts.Application.DTOs;
using Accounts.Application.Interfaces.Persistence.Queries;
using Accounts.Domain.ValueObjects;
using Dapper;
using Infrastructure.Common.Tenant;
using Microsoft.Extensions.Logging;

namespace Accounts.Infrastructure.Persistence.Queries;

public class UserQueries(IShardDbConnectionFactory connectionFactory, ITenantProvider tenantProvider,
    ILogger<UserQueries> logger) : IUserQueries
{
    private IDbConnection CreateConnection()
    {
        var tenantKey = tenantProvider.GetTenantKey();
        logger.LogInformation("🔍 TenantKey used: {TenantKey}", tenantKey);

        var conn = connectionFactory.CreateConnectionForTenant(tenantKey);
        logger.LogInformation("✅ DB Connection String: {ConnectionString}", conn.ConnectionString);
        return conn;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT u.Id, u.Username, u.Email, u.FullName, r.Name AS RoleName
            FROM Users u
            LEFT JOIN UserRoles ur ON ur.UsersId = u.Id
            LEFT JOIN Roles r ON r.Id = ur.RolesId
            WHERE u.Id = @Id;";

        try
        {
            logger.LogInformation("Getting user by ID: {UserId}", id);

            using var connection = CreateConnection();

            var userDictionary = new Dictionary<Guid, UserDto>();

            await connection.QueryAsync<UserDto, string, UserDto>(
                sql,
                (user, roleName) =>
                {
                    if (!userDictionary.TryGetValue(user.Id, out var userEntry))
                    {
                        userEntry = user;
                        userEntry.Roles = new List<string>();
                        userDictionary.Add(user.Id, userEntry);
                    }

                    if (!string.IsNullOrWhiteSpace(roleName))
                        userEntry.Roles.Add(roleName);

                    return userEntry;
                },
                new { Id = id },
                splitOn: "RoleName"
            );

            return userDictionary.Values.FirstOrDefault();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting user by ID: {UserId}", id);
            throw;
        }
    }

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        const string sql = @"
        SELECT u.Id, u.Username, u.Email, u.FullName, u.PasswordHash, r.Name AS RoleName
        FROM Users u
        LEFT JOIN UserRoles ur ON ur.UsersId = u.Id
        LEFT JOIN Roles r ON r.Id = ur.RolesId
        WHERE u.Email = @Email;";

        try
        {
            logger.LogInformation("Getting user by Email: {Email}", email);

            using var connection = CreateConnection();

            var result = await connection.QueryAsync(sql, new { Email = email });

            var userDictionary = new Dictionary<Guid, UserDto>();

            foreach (var row in result)
            {
                Guid id = row.id;

                if (!userDictionary.TryGetValue(id, out var userDto))
                {
                    var emailVo = Email.FromTrustedSource((string)row.email);
                    var passwordHashVo = PasswordHash.FromHashed((string)row.passwordhash);

                    userDto = new UserDto
                    {
                        Id = id,
                        Username = row.username,
                        FullName = row.fullname,
                        Email = emailVo,
                        PasswordHash = passwordHashVo,
                        Roles = new List<string>()
                    };

                    userDictionary.Add(id, userDto);
                }

                string? roleName = row.rolename;
                if (!string.IsNullOrWhiteSpace(roleName))
                    userDto.Roles.Add(roleName);
            }

            return userDictionary.Values.FirstOrDefault();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting user by Email: {Email}", email);
            throw;
        }
    }

    public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT u.Id, u.Username, u.Email, u.FullName, r.Name AS RoleName
            FROM Users u
            LEFT JOIN UserRoles ur ON ur.UsersId = u.Id
            LEFT JOIN Roles r ON r.Id = ur.RolesId;";

        try
        {
            logger.LogInformation("Getting all users");

            using var connection = CreateConnection();

            var userDictionary = new Dictionary<Guid, UserDto>();

            await connection.QueryAsync<UserDto, string, UserDto>(
                sql,
                (user, roleName) =>
                {
                    if (!userDictionary.TryGetValue(user.Id, out var userEntry))
                    {
                        userEntry = user;
                        userEntry.Roles = new List<string>();
                        userDictionary.Add(user.Id, userEntry);
                    }

                    if (!string.IsNullOrWhiteSpace(roleName))
                        userEntry.Roles.Add(roleName);

                    return userEntry;
                },
                splitOn: "RoleName"
            );

            return userDictionary.Values.ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting all users");
            throw;
        }
    }

    public async Task<UserDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT u.Id, u.Username, u.Email, u.FullName
                     FROM Users u 
                     WHERE u.Username = @Username;";
        try
        {
            logger.LogInformation("Getting user by Username: {Username}", username);

            using var connection = CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<UserDto>(sql, new { Username = username });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting user by Username: {Username}", username);
            throw;
        }
    }

    public async Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT r.Name FROM UserRoles ur
                             INNER JOIN Roles r ON r.Id = ur.RolesId
                             WHERE ur.UsersId = @UserId;";
        try
        {
            logger.LogInformation("Getting roles for user {UserId}", userId);

            using var connection = CreateConnection();
            var roles = await connection.QueryAsync<string>(sql, new { UserId = userId });

            return roles.ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting roles for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<UserDto?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT u.Id, u.Username, u.Email, u.FullName, u.PasswordHash,
                   r.Name AS RoleName
            FROM Users u
            LEFT JOIN UserRoles ur ON ur.UsersId = u.Id
            LEFT JOIN Roles r ON r.Id = ur.RolesId
            WHERE u.Id = @Id;
        ";

        try
        {
            using var connection = CreateConnection();

            var result = await connection.QueryAsync(sql, new { Id = id });

            UserDto? userDto = null;

            foreach (var row in result)
            {
                userDto ??= new UserDto
                {
                    Id = row.id,
                    Username = row.username,
                    FullName = row.fullname,
                    Email = Email.FromTrustedSource((string)row.email),
                    PasswordHash = PasswordHash.FromHashed((string)row.passwordhash),
                    Roles = new List<string>()
                };

                string? roleName = row.rolename;
                if (!string.IsNullOrWhiteSpace(roleName))
                    userDto.Roles.Add(roleName);
            }

            return userDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting user with roles by ID: {UserId}", id);
            throw;
        }
    }
}