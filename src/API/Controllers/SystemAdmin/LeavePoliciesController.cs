using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.LeavePolicies.Commands.ApplyLeavePolicy;
using Application.SystemAdmin.LeavePolicies.Commands.CreateLeavePolicy;
using Application.SystemAdmin.LeavePolicies.Commands.DeleteLeavePolicy;
using Application.SystemAdmin.LeavePolicies.Commands.UpdateLeavePolicy;
using Application.SystemAdmin.LeavePolicies.Common;
using Application.SystemAdmin.LeavePolicies.Queries.GetLeavePolicies;
using Infrastructure.Identity.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// Manage leave compensation policies and apply them to ophthalmologists.
/// </summary>
[Route("api/system-admin/leave-policies")]
public class LeavePoliciesController : BaseApiController
{
    private readonly IMediator _mediator;

    public LeavePoliciesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get paginated list of leave policies.
    /// </summary>
    [HttpGet]
    [AuthorizePermission(Permissions.SettingsRead)]
    public async Task<ActionResult<Result<GetLeavePoliciesResult>>> GetPaged([FromQuery] GetLeavePoliciesQuery query)
    {
        return Ok(await _mediator.Send(query));
    }

    /// <summary>
    /// Create a new leave policy.
    /// </summary>
    [HttpPost]
    [AuthorizePermission(Permissions.SettingsManage)]
    public async Task<ActionResult<Result<LeavePolicyDto>>> Create(CreateLeavePolicyCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Update an existing leave policy.
    /// </summary>
    [HttpPut("{id}")]
    [AuthorizePermission(Permissions.SettingsManage)]
    public async Task<ActionResult<Result>> Update(Guid id, UpdateLeavePolicyCommand command)
    {
        if (id != command.PolicyId)
            return BadRequest(Result.Failure("ID mismatch."));

        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Delete a leave policy (soft delete).
    /// </summary>
    [HttpDelete("{id}")]
    [AuthorizePermission(Permissions.SettingsManage)]
    public async Task<ActionResult<Result>> Delete(Guid id)
    {
        return Ok(await _mediator.Send(new DeleteLeavePolicyCommand { PolicyId = id }));
    }

    /// <summary>
    /// Apply a leave policy to an ophthalmologist (adds days to their fund).
    /// </summary>
    [HttpPost("{id}/apply/{ophthalmologistId}")]
    [AuthorizePermission(Permissions.OphthalmologistsUpdate)]
    public async Task<ActionResult<Result>> Apply(Guid id, Guid ophthalmologistId)
    {
        return Ok(await _mediator.Send(new ApplyLeavePolicyCommand 
        { 
            PolicyId = id, 
            OphthalmologistId = ophthalmologistId 
        }));
    }
}
