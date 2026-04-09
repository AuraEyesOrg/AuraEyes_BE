using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.AiQuota.Interfaces;
using Application.Ophthalmologists.Contracts.GetMyContract;
using Application.Ophthalmologists.Contracts.UploadSignedContract;
using Application.Organisations.Queries.GetDashboardMetrics;
using Application.Screenings.Commands.CreateAiScreeningSession;
using Application.Scheduling.Appointments.Common;
using Application.Scheduling.Appointments.Queries.GetOrganisationAppointments;
using Application.Scheduling.Appointments.Queries.GetOrganisationAvailableSlots;
using Application.SystemAdmin.Contracts.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace API.Controllers;

[Route("api/organisations")]
public class OrganisationsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAiQuotaService _aiQuotaService;
    private readonly ApplicationDbContext _context;

    public OrganisationsController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        IAiQuotaService aiQuotaService,
        ApplicationDbContext context)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _aiQuotaService = aiQuotaService;
        _context = context;
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

    [HttpPost("screenings/create-session")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<CreateAiScreeningSessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateOrganisationScreeningSession(
        [FromBody] CreateOrganisationScreeningSessionRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        if (request.PatientId == Guid.Empty)
            return BadRequest(ApiResponseFactory.Error("PatientId is required."));

        if (request.RetinalImages is null || request.RetinalImages.Count == 0)
            return BadRequest(ApiResponseFactory.Error("At least one retinal image is required."));

        var currentUser = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId.Value, cancellationToken);

        if (currentUser?.OrganizationId is null)
            return NotFound(ApiResponseFactory.NotFound("Organisation not found for this user."));

        var organisation = await _context.Organisations
            .FirstOrDefaultAsync(o => o.Id == currentUser.OrganizationId.Value, cancellationToken);
        if (organisation is null)
            return NotFound(ApiResponseFactory.NotFound("Organisation not found."));

        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);
        if (patient is null)
            return NotFound(ApiResponseFactory.NotFound("Patient not found."));

        if (!organisation.HasAvailableQuota())
        {
            return StatusCode(StatusCodes.Status402PaymentRequired,
            ApiResponseFactory.Error("Organisation AI screening quota exhausted. Please purchase additional credits or update contract quota."));
        }

        var screening = new AiScreening(
            request.PatientId,
            string.IsNullOrWhiteSpace(request.ModelVersion) ? "CFP_v1" : request.ModelVersion,
            organisation.Id);

        var responseImages = new List<RetinalImageResponse>();
        foreach (var imageData in request.RetinalImages)
        {
            if (string.IsNullOrWhiteSpace(imageData.ImageUrl))
                continue;

            var retinalImage = new RetinalImage(
                patientId: request.PatientId,
                imageUrl: imageData.ImageUrl,
                eyeSide: imageData.EyeSide,
                capturedAt: DateTime.UtcNow,
                deviceName: imageData.DeviceName);

            retinalImage.AssignToScreening(screening.Id);
            screening.AddRetinalImage(retinalImage);

            responseImages.Add(new RetinalImageResponse
            {
                Id = retinalImage.Id,
                ImageUrl = retinalImage.ImageUrl,
                EyeSide = retinalImage.EyeSide.ToString()
            });
        }

        screening.RecordConsent(
            request.PatientId,
            "Organisation confirms patient consent for AI screening.");

        organisation.ConsumeQuota();
        _context.AiScreenings.Add(screening);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponseFactory.Success(
            new CreateAiScreeningSessionResponse
            {
                ScreeningId = screening.Id,
                PatientId = screening.PatientId,
                ModelVersion = screening.ModelVersion,
                Images = responseImages,
                CreatedAt = screening.CreatedAt
            },
            "Screening session created successfully"));
    }

    [HttpGet("screenings/history")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrganisationScreeningHistoryItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganisationScreeningHistory(
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var orgId = await ResolveOrganisationIdAsync(_currentUserService.UserId.Value, cancellationToken);
        if (orgId is null)
            return NotFound(ApiResponseFactory.NotFound("Organisation not found for this user."));

        var safeTake = Math.Clamp(take, 1, 100);

        var rows = await (
            from screening in _context.AiScreenings.AsNoTracking()
            join patient in _context.Patients.AsNoTracking() on screening.PatientId equals patient.Id
            join user in _context.Users.AsNoTracking() on patient.UserId equals user.Id
            where screening.OrganisationId == orgId.Value
            orderby screening.CreatedAt descending
            select new
            {
                screening.Id,
                screening.PatientId,
                PatientName = user.FullName,
                screening.CreatedAt,
                screening.ProcessedAt,
                ImagesCount = screening.RetinalImages.Count,
                LatestResult = screening.ScreeningResults
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new { RiskLevel = r.RiskLevel.ToString(), r.ConfidenceScore })
                    .FirstOrDefault()
            })
            .Take(safeTake)
            .ToListAsync(cancellationToken);

        var items = rows.Select(row => new OrganisationScreeningHistoryItemResponse
        {
            ScreeningId = row.Id,
            PatientId = row.PatientId,
            PatientName = string.IsNullOrWhiteSpace(row.PatientName) ? "Patient" : row.PatientName,
            CreatedAt = row.CreatedAt,
            ProcessedAt = row.ProcessedAt,
            ImagesCount = row.ImagesCount,
            LatestRiskLevel = row.LatestResult?.RiskLevel,
            ConfidenceScore = row.LatestResult?.ConfidenceScore,
            Status = row.ProcessedAt.HasValue
                ? "completed"
                : row.LatestResult is not null
                    ? "saved"
                    : "pending"
        }).ToList();

        return Ok(ApiResponseFactory.Success(items, "Organisation screening history loaded"));
    }

    [HttpGet("screenings/{screeningId:guid}")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<OrganisationScreeningDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganisationScreeningDetail(
        Guid screeningId,
        CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var orgId = await ResolveOrganisationIdAsync(_currentUserService.UserId.Value, cancellationToken);
        if (orgId is null)
            return NotFound(ApiResponseFactory.NotFound("Organisation not found for this user."));

        var item = await _context.AiScreenings
            .AsNoTracking()
            .Where(s => s.Id == screeningId && s.OrganisationId == orgId.Value)
            .Select(s => new OrganisationScreeningDetailResponse
            {
                ScreeningId = s.Id,
                PatientId = s.PatientId,
                ModelVersion = s.ModelVersion,
                CreatedAt = s.CreatedAt,
                ProcessedAt = s.ProcessedAt,
                RawJsonOutput = s.RawJsonOutput,
                IsActive = s.IsActive,
                Images = s.RetinalImages
                    .OrderBy(i => i.CreatedAt)
                    .Select(i => new OrganisationRetinalImageDetailResponse
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        EyeSide = i.EyeSide.ToString(),
                        DeviceName = i.DeviceName,
                        QualityScore = i.QualityScore,
                        CapturedAt = i.CapturedAt
                    })
                    .ToList(),
                LatestResult = s.ScreeningResults
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new OrganisationScreeningResultDetailResponse
                    {
                        ScreeningResultId = r.Id,
                        RiskLevel = r.RiskLevel.ToString(),
                        ConfidenceScore = r.ConfidenceScore,
                        Summary = r.Summary,
                        Findings = r.Findings,
                        AssessedAt = r.CreatedAt
                    })
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
            return NotFound(ApiResponseFactory.NotFound("Screening session not found"));

        return Ok(ApiResponseFactory.Success(item, "Screening session loaded"));
    }

    [HttpGet("billing/summary")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<OrganisationBillingSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganisationBillingSummary(CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var orgId = await ResolveOrganisationIdAsync(_currentUserService.UserId.Value, cancellationToken);
        if (orgId is null)
            return NotFound(ApiResponseFactory.NotFound("Organisation not found for this user."));

        var quota = await _aiQuotaService.GetQuotaAsync(_currentUserService.UserId.Value, Roles.OrgAdmin, cancellationToken);
        var org = await _context.Organisations
            .AsNoTracking()
            .FirstAsync(o => o.Id == orgId.Value, cancellationToken);

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var totalScreeningsThisMonth = await _context.AiScreenings
            .AsNoTracking()
            .CountAsync(s => s.OrganisationId == orgId.Value && s.CreatedAt >= monthStart, cancellationToken);

        var totalScreeningsAllTime = await _context.AiScreenings
            .AsNoTracking()
            .CountAsync(s => s.OrganisationId == orgId.Value, cancellationToken);

        var patientUnitPrice = await GetDecimalSettingAsync("AI_QUOTA_UNIT_PRICE", 10000m, cancellationToken);
        var organisationUnitPrice = quota.UnitPrice ?? Math.Round(patientUnitPrice * 0.60m, 0, MidpointRounding.AwayFromZero);

        var response = new OrganisationBillingSummaryResponse
        {
            TotalScreeningsThisMonth = totalScreeningsThisMonth,
            TotalScreeningsAllTime = totalScreeningsAllTime,
            RemainingQuota = quota.RemainingQuota,
            PurchasedQuota = org.PurchasedAiQuota,
            MonthlyQuotaLimit = quota.MonthlyQuotaLimit ?? 0,
            MonthlyQuotaUsed = quota.MonthlyQuotaUsed ?? 0,
            MonthlyQuotaRemaining = quota.MonthlyQuotaRemaining ?? 0,
            PatientUnitPrice = patientUnitPrice,
            OrganisationUnitPrice = organisationUnitPrice
        };

        return Ok(ApiResponseFactory.Success(response, "Organisation billing summary loaded"));
    }

    [HttpGet("screening-reports")]
    [Authorize(Policy = Policies.OrgAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<OrganisationScreeningReportResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganisationScreeningReports(CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var orgId = await ResolveOrganisationIdAsync(_currentUserService.UserId.Value, cancellationToken);
        if (orgId is null)
            return NotFound(ApiResponseFactory.NotFound("Organisation not found for this user."));

        var rows = await _context.AiScreenings
            .AsNoTracking()
            .Where(s => s.OrganisationId == orgId.Value)
            .Select(s => new
            {
                s.CreatedAt,
                Latest = s.ScreeningResults
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new { RiskLevel = r.RiskLevel.ToString(), r.ConfidenceScore })
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        var completed = rows.Where(r => r.Latest is not null).ToList();
        var highRiskCount = completed.Count(r => string.Equals(r.Latest!.RiskLevel, "High", StringComparison.OrdinalIgnoreCase));
        var moderateRiskCount = completed.Count(r => string.Equals(r.Latest!.RiskLevel, "Moderate", StringComparison.OrdinalIgnoreCase));
        var lowRiskCount = completed.Count - highRiskCount - moderateRiskCount;
        var averageConfidence = completed.Count == 0
            ? 0
            : Math.Round(completed.Average(r => r.Latest!.ConfidenceScore), 1);

        var monthlyBreakdown = completed
            .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g => new OrganisationMonthlyBreakdownItem
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Count = g.Count(),
                HighRisk = g.Count(x => string.Equals(x.Latest!.RiskLevel, "High", StringComparison.OrdinalIgnoreCase)),
                ModerateRisk = g.Count(x => string.Equals(x.Latest!.RiskLevel, "Moderate", StringComparison.OrdinalIgnoreCase)),
                LowRisk = g.Count(x => !string.Equals(x.Latest!.RiskLevel, "High", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(x.Latest!.RiskLevel, "Moderate", StringComparison.OrdinalIgnoreCase))
            })
            .ToList();

        var response = new OrganisationScreeningReportResponse
        {
            TotalScreenings = completed.Count,
            HighRiskCount = highRiskCount,
            ModerateRiskCount = moderateRiskCount,
            LowRiskCount = lowRiskCount,
            AverageConfidence = averageConfidence,
            MonthlyBreakdown = monthlyBreakdown
        };

        return Ok(ApiResponseFactory.Success(response, "Organisation screening reports loaded"));
    }

    private async Task<Guid?> ResolveOrganisationIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<decimal> GetDecimalSettingAsync(string key, decimal defaultValue, CancellationToken cancellationToken)
    {
        var value = await _context.SystemSettings
            .AsNoTracking()
            .Where(s => s.Key == key)
            .Select(s => s.Value)
            .FirstOrDefaultAsync(cancellationToken);

        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : defaultValue;
    }
}

public record CreateOrganisationScreeningSessionRequest
{
    public Guid PatientId { get; init; }
    public string? ModelVersion { get; init; }
    public List<RetinalImageData> RetinalImages { get; init; } = new();
}

public record OrganisationScreeningHistoryItemResponse
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public int ImagesCount { get; init; }
    public string? LatestRiskLevel { get; init; }
    public decimal? ConfidenceScore { get; init; }
    public string Status { get; init; } = "pending";
}

public record OrganisationScreeningDetailResponse
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string ModelVersion { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public string? RawJsonOutput { get; init; }
    public bool IsActive { get; init; }
    public List<OrganisationRetinalImageDetailResponse> Images { get; init; } = new();
    public OrganisationScreeningResultDetailResponse? LatestResult { get; init; }
}

public record OrganisationRetinalImageDetailResponse
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string EyeSide { get; init; } = string.Empty;
    public string? DeviceName { get; init; }
    public decimal? QualityScore { get; init; }
    public DateTime CapturedAt { get; init; }
}

public record OrganisationScreeningResultDetailResponse
{
    public Guid ScreeningResultId { get; init; }
    public string RiskLevel { get; init; } = string.Empty;
    public decimal ConfidenceScore { get; init; }
    public string? Summary { get; init; }
    public string? Findings { get; init; }
    public DateTime AssessedAt { get; init; }
}

public record OrganisationBillingSummaryResponse
{
    public int TotalScreeningsThisMonth { get; init; }
    public int TotalScreeningsAllTime { get; init; }
    public int RemainingQuota { get; init; }
    public int PurchasedQuota { get; init; }
    public int MonthlyQuotaLimit { get; init; }
    public int MonthlyQuotaUsed { get; init; }
    public int MonthlyQuotaRemaining { get; init; }
    public decimal PatientUnitPrice { get; init; }
    public decimal OrganisationUnitPrice { get; init; }
}

public record OrganisationScreeningReportResponse
{
    public int TotalScreenings { get; init; }
    public int HighRiskCount { get; init; }
    public int ModerateRiskCount { get; init; }
    public int LowRiskCount { get; init; }
    public decimal AverageConfidence { get; init; }
    public List<OrganisationMonthlyBreakdownItem> MonthlyBreakdown { get; init; } = new();
}

public record OrganisationMonthlyBreakdownItem
{
    public string Month { get; init; } = string.Empty;
    public int Count { get; init; }
    public int HighRisk { get; init; }
    public int ModerateRisk { get; init; }
    public int LowRisk { get; init; }
}
