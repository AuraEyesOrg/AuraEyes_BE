using Application.Common.Interfaces;
using Application.Common.Models;
using Application.OrganisationScreenings.Queries.ExportOrgScreeningReportPdf;
using Application.OrganisationScreenings.Queries.GetOrgScreeningSessionDetail;
using Domain.Common;
using Domain.Entities.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.OrganisationScreenings.Commands.ShareOrgScreeningResult;

public sealed class ShareOrgScreeningResultCommandHandler
    : ICommandHandler<ShareOrgScreeningResultCommand, ShareOrgScreeningResultResponse>
{
    private readonly IMediator _mediator;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ShareOrgScreeningResultCommandHandler> _logger;

    public ShareOrgScreeningResultCommandHandler(
        IMediator mediator,
        IRepository<Patient> patientRepository,
        IIdentityService identityService,
        IEmailService emailService,
        ILogger<ShareOrgScreeningResultCommandHandler> logger)
    {
        _mediator = mediator;
        _patientRepository = patientRepository;
        _identityService = identityService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<ShareOrgScreeningResultResponse>> Handle(
        ShareOrgScreeningResultCommand request,
        CancellationToken cancellationToken)
    {
        if (!request.IncludePdf && !request.IncludeRetinalImages)
        {
            return Result<ShareOrgScreeningResultResponse>.Failure(
                "At least one sharing option is required.");
        }

        var detailResult = await _mediator.Send(
            new GetOrgScreeningSessionDetailQuery(request.OrgAdminUserId, request.ScreeningId),
            cancellationToken);

        if (!detailResult.IsSuccess || detailResult.Data is null)
            return MapFailure(detailResult);

        var detail = detailResult.Data;

        var patient = await _patientRepository.GetByIdAsync(detail.PatientId, cancellationToken);
        if (patient is null)
            return Result<ShareOrgScreeningResultResponse>.NotFound("Patient not found");

        var recipientEmail = (request.RecipientEmail ?? string.Empty).Trim();
        if (!patient.IsWalkIn && patient.UserId.HasValue && string.IsNullOrWhiteSpace(recipientEmail))
        {
            var user = await _identityService.GetUserByIdAsync(patient.UserId.Value, cancellationToken);
            recipientEmail = user?.Email?.Trim() ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            var message = patient.IsWalkIn
                ? "Recipient email is required for walk-in patients."
                : "Recipient email is not available for this patient.";
            return Result<ShareOrgScreeningResultResponse>.Failure(message);
        }

        var attachments = new List<EmailAttachment>();
        if (request.IncludePdf)
        {
            var pdfResult = await _mediator.Send(
                new ExportOrgScreeningReportPdfQuery(request.OrgAdminUserId, request.ScreeningId),
                cancellationToken);

            if (!pdfResult.IsSuccess || pdfResult.Data is null)
                return MapFailure(pdfResult);

            attachments.Add(new EmailAttachment(
                pdfResult.Data.FileName,
                pdfResult.Data.Content,
                pdfResult.Data.ContentType));
        }

        var retinalImageUrls = request.IncludeRetinalImages
            ? detail.Images
                .Where(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
                .Select(x => x.ImageUrl.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(6)
                .ToList()
            : [];

        var payload = new OrganisationScreeningResultShareEmailPayload(
            request.ScreeningId,
            string.IsNullOrWhiteSpace(detail.PatientName) ? "Bệnh nhân" : detail.PatientName.Trim(),
            detail.CreatedAt,
            detail.LatestResult?.RiskLevel ?? "N/A",
            detail.LatestResult?.ConfidenceScore,
            detail.LatestResult?.Summary,
            request.IncludePdf,
            retinalImageUrls);

        try
        {
            await _emailService.SendOrganisationScreeningResultShareAsync(
                recipientEmail,
                payload,
                attachments,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to share screening result {ScreeningId} to {RecipientEmail}",
                request.ScreeningId,
                recipientEmail);
            return Result<ShareOrgScreeningResultResponse>.Failure("Failed to send share email.");
        }

        _logger.LogInformation(
            "Organisation screening {ScreeningId} shared to {RecipientEmail}. IncludePdf={IncludePdf}, IncludeRetinalImages={IncludeRetinalImages}",
            request.ScreeningId,
            recipientEmail,
            request.IncludePdf,
            request.IncludeRetinalImages);

        return Result<ShareOrgScreeningResultResponse>.Success(new ShareOrgScreeningResultResponse
        {
            RecipientEmail = recipientEmail,
            SharedAt = DateTime.UtcNow
        });
    }

    private static Result<ShareOrgScreeningResultResponse> MapFailure<T>(Result<T> source)
    {
        if (source.IsUnauthorized)
            return Result<ShareOrgScreeningResultResponse>.Unauthorized(source.ErrorMessage);

        if (source.IsForbidden)
            return Result<ShareOrgScreeningResultResponse>.Forbidden(source.ErrorMessage);

        if (source.IsNotFound)
            return Result<ShareOrgScreeningResultResponse>.NotFound(source.ErrorMessage);

        if (source.IsConflict)
            return Result<ShareOrgScreeningResultResponse>.Conflict(source.ErrorMessage);

        if (source.IsPaymentRequired)
            return Result<ShareOrgScreeningResultResponse>.PaymentRequired(source.ErrorMessage);

        return Result<ShareOrgScreeningResultResponse>.Failure(source.Errors);
    }
}
