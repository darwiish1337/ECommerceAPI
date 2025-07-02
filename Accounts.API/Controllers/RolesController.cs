using Accounts.Application.DTOs.Requests;
using Accounts.Application.DTOs.Requests.Permission;
using Accounts.Application.DTOs.Requests.Role;
using Accounts.Application.Interfaces.Persistence.Queries;
using Accounts.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constants;

namespace Accounts.API.Controllers;

[ApiController]
public class RolesController(IRoleService roleService, IPermissionQueries permissionQueries, IRoleQueries roleQueries) : ControllerBase
{
    [HttpGet(ApiEndpoints.Accounts.Roles.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await roleQueries.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet(ApiEndpoints.Accounts.Roles.GetById)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await roleQueries.GetByIdAsync(id, cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost(ApiEndpoints.Accounts.Roles.Create)]
    public async Task<IActionResult> Create(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var id = await roleService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut(ApiEndpoints.Accounts.Roles.Update)]
    public async Task<IActionResult> Update(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id) return BadRequest("Id mismatch");
        await roleService.UpdateAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpDelete(ApiEndpoints.Accounts.Roles.Delete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await roleService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost(ApiEndpoints.Accounts.Roles.AssignPermissions)]
    public async Task<IActionResult> AssignPermissions(Guid id, AssignPermissionsRequest request, CancellationToken cancellationToken)
    {
        if (id != request.RoleId) return BadRequest("Id mismatch");
        await roleService.AssignPermissionsAsync(request.RoleId, request.PermissionIds, cancellationToken);
        return NoContent();
    }
    
    [HttpGet(ApiEndpoints.Accounts.Roles.GetRolePermissions)]
    public async Task<IActionResult> GetRolePermissions(Guid id, CancellationToken cancellationToken)
    {
        var permissions = await permissionQueries.GetByRoleIdAsync(id, cancellationToken);
        return Ok(permissions);
    }
    
}