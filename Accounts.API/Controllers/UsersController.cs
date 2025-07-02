using Accounts.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constants;
using System.Net.Mime;
using Accounts.Application.DTOs.Requests.Role;
using Accounts.Application.DTOs.Requests.User;
using Accounts.Application.Interfaces.Persistence.Queries;
using SharedKernel.Abstractions.Auth;

namespace Accounts.API.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class UsersController(IUserService userService, IUserQueries userQueries, ICurrentRequestContext currentRequestContext) : ControllerBase
{
    [HttpPost(ApiEndpoints.Accounts.Users.AssignRoles)]
    public async Task<IActionResult> AssignRoles(Guid id, [FromBody] AssignRolesRequest request, CancellationToken cancellationToken)
    {
        if (id != request.UserId)
            return BadRequest("Id mismatch");

        var result = await userService.AssignRolesAsync(id, request.RoleIds, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
    
    [HttpPut(ApiEndpoints.Accounts.Users.Update)]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var userId = currentRequestContext.UserId;
        if (userId is null)
            return Unauthorized();

        var result = await userService.UpdateAsync(userId.Value, request, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete(ApiEndpoints.Accounts.Users.Delete)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var result = await userService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet(ApiEndpoints.Accounts.Users.GetById)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var result = await userQueries.GetByIdAsync(id, cancellationToken);
        return result != null ? Ok(result) : NotFound("User not found");
    }

    [HttpGet(ApiEndpoints.Accounts.Users.GetAll)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var result = await userQueries.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet(ApiEndpoints.Accounts.Users.GetUserRoles)]
    public async Task<IActionResult> GetUserRoles(Guid id, CancellationToken cancellationToken)
    {
        var result = await userQueries.GetUserRolesAsync(id, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet(ApiEndpoints.Accounts.Users.GetUserInfo)]
    public async Task<IActionResult> GetUserInfo(CancellationToken cancellationToken)
    {
        var userId = currentRequestContext.UserId;
        if (userId is null)
            return Unauthorized();

        var user = await userQueries.GetByIdAsync(userId.Value, cancellationToken);
        return user is not null ? Ok(user) : NotFound("User not found");
    }
    
    [HttpDelete(ApiEndpoints.Accounts.Users.DeleteAccount)]
    public async Task<IActionResult> DeleteAccount(CancellationToken cancellationToken)
    {
        var userId = currentRequestContext.UserId;
        if (userId is null)
            return Unauthorized();

        var result = await userService.DeleteAsync(userId.Value, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
    
}