using System.Net;
using System.Text;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.OrganisationScreenings.Queries.ExportOrgScreeningReportPdf;
using Application.OrganisationScreenings.Queries.GetOrgScreeningSessionDetail;
using Application.Screenings.Queries.GetScreeningSessionDetail;
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

        var subject = $"AuraEyes Screening Result - {request.ScreeningId.ToString("N")[..8]}";
        var body = BuildEmailBody(detail, request.IncludePdf, request.IncludeRetinalImages);

        try
        {
            if (attachments.Count > 0)
            {
                await _emailService.SendWithAttachmentsAsync(
                    recipientEmail,
                    subject,
                    body,
                    attachments,
                    isHtml: true,
                    cancellationToken);
            }
            else
            {
                await _emailService.SendAsync(
                    recipientEmail,
                    subject,
                    body,
                    isHtml: true,
                    cancellationToken);
            }
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

    private static string BuildEmailBody(
        ScreeningSessionDetailDto detail,
        bool includePdf,
        bool includeRetinalImages)
    {
        var patientName = string.IsNullOrWhiteSpace(detail.PatientName) ? "Patient" : detail.PatientName.Trim();
        var riskLevel = detail.LatestResult?.RiskLevel ?? "N/A";
        var confidence = detail.LatestResult?.ConfidenceScore;
        var summary = detail.LatestResult?.Summary;

        var sb = new StringBuilder();
        sb.Append("<p>Hello,</p>");
        sb.Append("<p>Your organisation has shared a retinal screening result from AuraEyes.</p>");
        sb.Append("<ul>");
        sb.Append($"<li><strong>Patient:</strong> {WebUtility.HtmlEncode(patientName)}</li>");
        sb.Append($"<li><strong>Screening ID:</strong> {detail.ScreeningId}</li>");
        sb.Append($"<li><strong>Created At (UTC):</strong> {detail.CreatedAt:yyyy-MM-dd HH:mm:ss}</li>");
        sb.Append($"<li><strong>Risk Level:</strong> {WebUtility.HtmlEncode(riskLevel)}</li>");
        if (confidence.HasValue)
        {
            sb.Append($"<li><strong>Confidence:</strong> {Math.Round(confidence.Value, 2)}%</li>");
        }

        sb.Append("</ul>");

        if (!string.IsNullOrWhiteSpace(summary))
        {
            sb.Append($"<p><strong>Summary:</strong> {WebUtility.HtmlEncode(summary)}</p>");
        }

        if (includePdf)
        {
            sb.Append("<p>The report PDF is attached to this email.</p>");
        }

        if (includeRetinalImages)
        {
            var imageUrls = detail.Images
                .Where(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
                .Take(6)
                .Select(x => x.ImageUrl)
                .ToList();

            if (imageUrls.Count > 0)
            {
                sb.Append("<p><strong>Retinal Images:</strong></p><ul>");
                foreach (var imageUrl in imageUrls)
                {
                    var encodedUrl = WebUtility.HtmlEncode(imageUrl);
                    sb.Append($"<li><a href=\"{encodedUrl}\">{encodedUrl}</a></li>");
                }

                sb.Append("</ul>");
            }
        }

        sb.Append("<p>Regards,<br/>AuraEyes</p>");
        return sb.ToString();
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
