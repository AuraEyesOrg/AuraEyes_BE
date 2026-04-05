using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Patients.Common;
using Application.Patients.Queries.GetPatientRoadmapById;
using Application.Patients.Queries.GetPatientRoadmaps;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/patient/roadmaps")]
[Authorize(Policy = Policies.PatientOnly)]
public class PatientRoadmapsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public PatientRoadmapsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PatientRoadmapDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRoadmaps()
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));

        var result = await _mediator.Send(new GetPatientRoadmapsQuery(_currentUserService.UserId.Value));
        return HandleResult(result);
    }

    [HttpGet("{roadmapId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientRoadmapDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoadmapById(Guid roadmapId)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));

        var result = await _mediator.Send(new GetPatientRoadmapByIdQuery(_currentUserService.UserId.Value, roadmapId));
        return HandleResult(result);
    }
}
