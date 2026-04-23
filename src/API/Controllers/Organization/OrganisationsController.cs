using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Contracts.GetMyContract;
using Application.Ophthalmologists.Contracts.UploadSignedContract;
using Application.Organisations.Commands.UpdateOrganisationSettings;
using Application.Organisations.Common;
using Application.Organisations.Queries.GetBillingSummary;
using Application.Organisations.Queries.GetDashboardMetrics;
using Application.Organisations.Queries.GetOrganisationSettings;
using Application.Organisations.Queries.GetScreeningReports;
using Application.OrganisationScreenings;
using Application.Scheduling.Appointments.Common;
using Application.Scheduling.Appointments.Common;
using Application.SystemAdmin.Contracts.Common;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Identity.Authorization;

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
    [AuthorizePermission(Permissions.DashboardRead)]
    [ProducesResponseType(typeof(ApiResponse<OrganisationDashboardMetricsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(new GetDashboardMetricsQuery(_currentUserService.UserId.Value));
        return HandleResult(result);

    }



    [HttpGet("my-contract")]
    [AuthorizePermission(Permissions.ContractsRead)]
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
    [AuthorizePermission(Permissions.ContractsRead)]
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
                $"organisations/contracts/{userId.Value}");
        }

        var result = await _mediator.Send(new UploadSignedContractCommand
        {
            UserId = userId.Value,
            ScannedDocumentUrl = scannedUrl
        });

        return HandleResult(result, "Contract uploaded successfully. Waiting for admin verification.");
    }

    [HttpGet("billing/summary")]
    [AuthorizePermission(Permissions.WalletsRead)]
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
    [AuthorizePermission(Permissions.ScreeningRead)]
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

    [HttpGet("settings")]
    [AuthorizePermission(Permissions.OrganisationsRead)]
    [ProducesResponseType(typeof(ApiResponse<OrganisationSettingsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSettings(CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(
            new GetOrganisationSettingsQuery(_currentUserService.UserId.Value),
            cancellationToken);

        return HandleResult(result, "Organisation settings loaded.");
    }

    [HttpPut("settings")]
    [AuthorizePermission(Permissions.OrganisationsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<OrganisationSettingsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSettings(
        [FromBody] UpdateOrganisationSettingsRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var command = new UpdateOrganisationSettingsCommand
        {
            OrgAdminUserId = _currentUserService.UserId.Value,
            Name = request.Name,
            Address = request.Address,
            LicenseNumber = request.LicenseNumber,
            TaxCode = request.TaxCode,
            Description = request.Description,
            ContactFullName = request.ContactFullName,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            AvatarUrl = request.AvatarUrl
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Organisation settings updated successfully.");
    }
}

public record UpdateOrganisationSettingsRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? LicenseNumber { get; init; }
    public string? TaxCode { get; init; }
    public string? Description { get; init; }
    public string? ContactFullName { get; init; }
    public string? ContactEmail { get; init; }
    public string? ContactPhone { get; init; }
    public string? AvatarUrl { get; init; }
}
