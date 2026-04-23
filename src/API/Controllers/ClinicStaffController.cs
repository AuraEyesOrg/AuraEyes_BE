using Application.ClinicStaffs.Commands.CreateClinicStaff;
using Application.ClinicStaffs.Commands.DeleteClinicStaff;
using Application.ClinicStaffs.Commands.UpdateClinicStaff;
using Application.ClinicStaffs.Queries.GetAllClinicStaff;
using Application.ClinicStaffs.Queries.GetClinicStaffById;
using Application.Common.Constants;
using Domain.Enums;
using Infrastructure.Identity.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages clinic staff profiles (Receptionist, Coordinator, Cashier).
/// All endpoints require SystemAdmin permissions except read operations which
/// are accessible to all clinical team members.
/// </summary>
[Route("api/clinic-staff")]
public class ClinicStaffController : BaseApiController
{
    private readonly IMediator _mediator;

    public ClinicStaffController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ── GET /api/clinic-staff ──────────────────────────────────────────────

    /// <summary>
    /// Returns all active clinic staff members, optionally filtered by sub-role.
    /// </summary>
    /// <param name="subRole">Optional: filter by Receptionist, Coordinator, or Cashier.</param>
    [HttpGet]
    [AuthorizePermission(Permissions.ClinicStaffRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] ClinicStaffRole? subRole,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllClinicStaffQuery { SubRole = subRole }, cancellationToken);
        return HandleResult(result, "Clinic staff list retrieved successfully.");
    }

    // ── GET /api/clinic-staff/{id} ─────────────────────────────────────────

    /// <summary>Gets a single clinic staff member by domain entity ID.</summary>
    [HttpGet("{id:guid}")]
    [AuthorizePermission(Permissions.ClinicStaffRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetClinicStaffByIdQuery { StaffId = id }, cancellationToken);
        return HandleResult(result, "Clinic staff retrieved successfully.");
    }

    // ── POST /api/clinic-staff ─────────────────────────────────────────────

    /// <summary>
    /// Creates a new clinic staff profile for an existing Identity user.
    /// The ClinicStaff role is automatically assigned to the user.
    /// </summary>
    [HttpPost]
    [AuthorizePermission(Permissions.ClinicStaffCreate)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateClinicStaffRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateClinicStaffCommand
        {
            UserId = request.UserId,
            SubRoles = request.SubRoles,
            Department = request.Department,
            EmployeeCode = request.EmployeeCode,
            Phone = request.Phone
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleResult(result);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data },
            result.Data);
    }

    // ── PUT /api/clinic-staff/{id} ─────────────────────────────────────────

    /// <summary>Updates sub-roles and profile details of a clinic staff member.</summary>
    [HttpPut("{id:guid}")]
    [AuthorizePermission(Permissions.ClinicStaffUpdate)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateClinicStaffRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateClinicStaffCommand
        {
            StaffId = id,
            SubRoles = request.SubRoles,
            Department = request.Department,
            EmployeeCode = request.EmployeeCode,
            Phone = request.Phone
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Clinic staff updated successfully.");
    }

    // ── DELETE /api/clinic-staff/{id} ─────────────────────────────────────

    /// <summary>Deactivates (soft-deletes) a clinic staff profile.</summary>
    [HttpDelete("{id:guid}")]
    [AuthorizePermission(Permissions.ClinicStaffDelete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteClinicStaffCommand { StaffId = id }, cancellationToken);
        return HandleResult(result, "Clinic staff deactivated successfully.");
    }
}

// ─── Request models ────────────────────────────────────────────────────────

/// <summary>Request body for creating a new clinic staff profile.</summary>
public record CreateClinicStaffRequest(
    Guid UserId,
    IReadOnlyList<ClinicStaffRole> SubRoles,
    string? Department,
    string? EmployeeCode,
    string? Phone);

/// <summary>Request body for updating a clinic staff profile.</summary>
public record UpdateClinicStaffRequest(
    IReadOnlyList<ClinicStaffRole> SubRoles,
    string? Department,
    string? EmployeeCode,
    string? Phone);
