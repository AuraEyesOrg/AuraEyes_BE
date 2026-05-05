using Application.Common.Constants;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Application.Scheduling.Appointments.Commands.ConfirmAppointmentCancellation;
using Application.Scheduling.Appointments.Queries.GetPendingCancellations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

[Route("api/system-admin/appointments")]
[Authorize(Policy = Policies.SystemAdminOnly)]
public class AdminAppointmentsController : BaseApiController
{
    private readonly IMediator _mediator;

    public AdminAppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("pending-cancellations")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ClinicAppointmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingCancellations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetPendingCancellationsQuery(pageNumber, pageSize));
        return HandleResult(result);
    }

    public record ConfirmRefundPayload(string? RefundTransactionId, string? AdminNote);

    [HttpPost("{appointmentId:guid}/confirm-refund")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConfirmRefund(Guid appointmentId, [FromBody] ConfirmRefundPayload payload)
    {
        var result = await _mediator.Send(new ConfirmAppointmentCancellationCommand(appointmentId, payload.RefundTransactionId, payload.AdminNote));
        return HandleResult(result);
    }

    public record RejectRefundPayload(string? AdminNote);

    [HttpPost("{appointmentId:guid}/reject-refund")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectRefund(Guid appointmentId, [FromBody] RejectRefundPayload payload)
    {
        var result = await _mediator.Send(new Application.Scheduling.Appointments.Commands.RejectAppointmentCancellation.RejectAppointmentCancellationCommand(appointmentId, payload.AdminNote));
        return HandleResult(result);
    }
}
