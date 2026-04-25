using Application.ClinicQueue.Commands.SendToDoctor;
using Application.ClinicQueue.Queries.GetClinicQueue;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Infrastructure.Identity.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/clinic-queue")]
// [AuthorizePermission(Permissions.ScreeningRead)]
public class ClinicQueueController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ClinicQueueController(
        IMediator mediator,
        ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Get the clinic queue for the current organisation.
    /// Returns all active patient visits with their screening and consultation status.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ClinicQueueItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQueue(CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var result = await _mediator.Send(
            new GetClinicQueueQuery { RequestedByUserId = _currentUser.UserId.Value },
            cancellationToken);
        return HandleResult(result, "Clinic queue loaded successfully");
    }

    /// <summary>
    /// Send a patient case to a doctor for consultation.
    /// Creates a consultation session and updates the visit status.
    /// </summary>
    [HttpPost("{visitId:guid}/send-to-doctor")]
    [AuthorizePermission(Permissions.ScreeningCreate)]
    [ProducesResponseType(typeof(ApiResponse<SendToDoctorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendToDoctor(
        [FromRoute] Guid visitId,
        [FromBody] SendToDoctorRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var command = new SendToDoctorCommand
        {
            VisitId = visitId,
            ScreeningId = request.ScreeningId,
            DoctorId = request.DoctorId,
            RequestedByUserId = _currentUser.UserId.Value,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Case sent to doctor successfully");
    }
}

public record SendToDoctorRequest
{
    public Guid ScreeningId { get; init; }
    public Guid? DoctorId { get; init; }
    public string? Notes { get; init; }
}
