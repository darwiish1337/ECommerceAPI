using Accounts.Application.DTOs.Requests.Permission;
using Accounts.Application.Interfaces.Persistence.Queries;
using Accounts.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constants;

namespace Accounts.API.Controllers;

[ApiController]
public class PermissionsController(IPermissionService permissionService, IPermissionQueries permissionQueries) : ControllerBase
{
    [HttpGet(ApiEndpoints.Accounts.Permissions.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await permissionQueries.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet(ApiEndpoints.Accounts.Permissions.GetById)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await permissionQueries.GetByIdAsync(id, cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost(ApiEndpoints.Accounts.Permissions.Create)]
    public async Task<IActionResult> Create(CreatePermissionRequest request, CancellationToken cancellationToken)
    {
        var id = await permissionService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut(ApiEndpoints.Accounts.Permissions.Update)]
    public async Task<IActionResult> Update(Guid id, UpdatePermissionRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id) return BadRequest("Id mismatch");
        await permissionService.UpdateAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpDelete(ApiEndpoints.Accounts.Permissions.Delete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await permissionService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}