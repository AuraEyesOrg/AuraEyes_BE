using Application.Common.Constants;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Application.Scheduling.Appointments.Queries.GetPatientClinicAppointments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/patients")]
public class PatientsController : BaseApiController
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{patientId:guid}/clinic-appointments")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ClinicAppointmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatientClinicAppointments(Guid patientId)
    {
        var result = await _mediator.Send(new GetPatientClinicAppointmentsQuery(patientId));
        return HandleResult(result);
    }
}
