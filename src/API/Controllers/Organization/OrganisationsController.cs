using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Contracts.GetMyContract;
using Application.Ophthalmologists.Contracts.UploadSignedContract;
using Application.Organisations.Queries.GetBillingSummary;
using Application.Organisations.Queries.GetDashboardMetrics;
using Application.Organisations.Queries.GetScreeningReports;
using Application.OrganisationScreenings;
using Application.Scheduling.Appointments.Common;
using Application.Scheduling.Appointments.Queries.GetOrganisationAppointments;
using Application.Scheduling.Appointments.Queries.GetOrganisationAvailableSlots;
using Application.SystemAdmin.Contracts.Common;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Organization;

[Route("api/organisations")]
public class OrganisationsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public OrganisationsController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    [HttpGet("dashboard-metrics")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<OrganisationDashboardMetricsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(new GetDashboardMetricsQuery(_currentUserService.UserId.Value));
        return HandleResult(result);

    }

    [HttpGet("{orgId:guid}/available-slots")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrganisationAvailableSlotDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganisationAvailableSlots(
        Guid orgId,
        [FromQuery] DateOnly? date = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        var query = new GetOrganisationAvailableSlotsQuery
        {
            OrganisationId = orgId,
            Date = date,
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{orgId:guid}/appointments")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ClinicAppointmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganisationAppointments(
        Guid orgId,
        [FromQuery] DateOnly? date = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] AppointmentStatus? status = null)
    {
        var query = new GetOrganisationAppointmentsQuery
        {
            OrganisationId = orgId,
            Date = date,
            FromDate = fromDate,
            ToDate = toDate,
            Status = status
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("my-contract")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<ContractDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyContract()
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(new GetMyContractQuery(userId.Value));
        return HandleResult(result);
    }

    [HttpPost("my-contract/upload")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadSignedContract(IFormFile contractImage)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        if (contractImage == null || contractImage.Length == 0)
            return BadRequest(ApiResponseFactory.Error("Contract image file is required."));

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "application/pdf" };
        if (!allowedTypes.Contains(contractImage.ContentType.ToLowerInvariant()))
            return BadRequest(ApiResponseFactory.Error("Only JPEG, PNG, WebP and PDF files are allowed."));

        if (contractImage.Length > 10 * 1024 * 1024)
            return BadRequest(ApiResponseFactory.Error("File size must not exceed 10MB."));

        string scannedUrl;
        await using (var stream = contractImage.OpenReadStream())
        {
            scannedUrl = await _fileStorageService.SaveFileAsync(
                stream,
                contractImage.FileName,
                $"contracts/{userId.Value}");
        }

        var result = await _mediator.Send(new UploadSignedContractCommand
        {
            UserId = userId.Value,
            ScannedDocumentUrl = scannedUrl
        });

        return HandleResult(result, "Contract uploaded successfully. Waiting for admin verification.");
    }

    [HttpGet("billing/summary")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<OrgBillingSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBillingSummary(CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(
            new GetBillingSummaryQuery(_currentUserService.UserId.Value),
            cancellationToken);

        return HandleResult(result, "Billing summary loaded");
    }

    [HttpGet("screening-reports")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<OrgScreeningReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScreeningReports(CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(
            new GetScreeningReportsQuery(_currentUserService.UserId.Value),
            cancellationToken);

        return HandleResult(result, "Screening reports loaded");
    }
}
