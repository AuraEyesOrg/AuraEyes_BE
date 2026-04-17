using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums.Network;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Network.Posts.Commands.CreatePost;
/// <summary>
/// Handler for CreatePostCommand
/// </summary>
public class CreatePostCommandHandler : ICommandHandler<CreatePostCommand, Guid>
{
    private readonly IPostRepository _postRepository;
    private readonly IConsultationSessionRepository _consultationSessionRepository;
    private readonly IRepository<AiScreening> _aiScreeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<MedicalDiagnosis> _medicalDiagnosisRepository;
    private readonly IOrganisationPatientsRepository _organisationPatientsRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public CreatePostCommandHandler(
        IPostRepository postRepository,
        IConsultationSessionRepository consultationSessionRepository,
        IRepository<AiScreening> aiScreeningRepository,
        IRepository<Patient> patientRepository,
        IRepository<MedicalDiagnosis> medicalDiagnosisRepository,
        IOrganisationPatientsRepository organisationPatientsRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _postRepository = postRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _aiScreeningRepository = aiScreeningRepository;
        _patientRepository = patientRepository;
        _medicalDiagnosisRepository = medicalDiagnosisRepository;
        _organisationPatientsRepository = organisationPatientsRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Guid>> Handle(
        CreatePostCommand request,
        CancellationToken cancellationToken)
    {
        // Validate anonymization confirmation when attachments are provided
        if (request.Attachments is { Count: > 0 } && !request.IsAnonymizationConfirmed)
        {
            return Result<Guid>.Failure("Bạn phải xác nhận đã ẩn danh dữ liệu bệnh nhân trước khi đính kèm tệp.");
        }

        var hasConsultationSource = request.ConsultationSessionId.HasValue;
        var hasAiScreeningSource = request.AiScreeningId.HasValue;

        if (hasConsultationSource && hasAiScreeningSource)
        {
            return Result<Guid>.Failure("Only one source reference is allowed: consultationSessionId or aiScreeningId.");
        }

        if (!hasConsultationSource && !hasAiScreeningSource && string.IsNullOrWhiteSpace(request.Content))
        {
            return Result<Guid>.Failure("Post content is required when no source reference is provided.");
        }

        if (request.IsInternalCase && !hasConsultationSource)
        {
            return Result<Guid>.Failure("consultationSessionId is required for internal case posts.");
        }

        var resolvedData = new ResolvedSourceData(
            request.Content.Trim(),
            request.OrganisationId,
            request.IsInternalCase,
            request.ConsultationSessionId,
            request.AiScreeningId,
            request.PatientAge,
            request.PatientGender,
            request.AiScreeningId);

        if (hasConsultationSource)
        {
            var sourceResult = await ResolveFromConsultationSourceAsync(request, cancellationToken);
            if (!sourceResult.IsSuccess || sourceResult.Data is null)
            {
                return MapSourceFailure<Guid>(sourceResult);
            }

            resolvedData = sourceResult.Data;
        }
        else if (hasAiScreeningSource)
        {
            var sourceResult = await ResolveFromAiScreeningSourceAsync(request, cancellationToken);
            if (!sourceResult.IsSuccess || sourceResult.Data is null)
            {
                return MapSourceFailure<Guid>(sourceResult);
            }

            resolvedData = sourceResult.Data;
        }

        var post = new ProfessionalPost(
            request.AuthorId,
            request.AuthorType,
            resolvedData.Content,
            request.Category,
            resolvedData.OrganisationId,
            request.AllowComments);

        var hasClinicalMetadata =
            resolvedData.IsInternalCase
            || resolvedData.ConsultationSessionId.HasValue
            || resolvedData.AiScreeningId.HasValue
            || resolvedData.PatientAge.HasValue
            || !string.IsNullOrWhiteSpace(resolvedData.PatientGender);

        if (hasClinicalMetadata)
        {
            post.SetClinicalCaseMetadata(
                resolvedData.IsInternalCase,
                resolvedData.ConsultationSessionId,
                resolvedData.AiScreeningId,
                resolvedData.PatientAge,
                resolvedData.PatientGender);
        }

        // Handle file uploads
        if (request.Attachments is { Count: > 0 })
        {
            var order = 0;
            foreach (var file in request.Attachments)
            {
                var subFolder = $"network/posts/{post.Id}";
                await using var stream = file.OpenReadStream();
                var fileUrl = await _fileStorageService.SaveFileAsync(
                    stream, file.FileName, subFolder, cancellationToken);

                var attachmentType = GetAttachmentType(file.ContentType);

                var attachment = new PostAttachment(
                    post.Id,
                    attachmentType,
                    file.FileName,
                    fileUrl,
                    file.ContentType,
                    file.Length,
                    order++);

                post.AddAttachment(attachment);
            }
        }

        if ((request.Attachments == null || request.Attachments.Count == 0) &&
            resolvedData.AttachmentAiScreeningId.HasValue)
        {
            await AddRetinalAttachmentsFromAiScreeningAsync(
                post,
                resolvedData.AttachmentAiScreeningId.Value,
                cancellationToken);
        }

        await _postRepository.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(post.Id);
    }

    private static AttachmentType GetAttachmentType(string? contentType)
    {
        if (string.IsNullOrEmpty(contentType))
            return AttachmentType.Document;

        if (contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return AttachmentType.Image;

        if (contentType == "application/pdf")
            return AttachmentType.Document;

        return AttachmentType.Document;
    }

    private async Task<Result<ResolvedSourceData>> ResolveFromConsultationSourceAsync(
        CreatePostCommand request,
        CancellationToken cancellationToken)
    {
        if (request.AuthorType != AuthorType.Ophthalmologist)
        {
            return Result<ResolvedSourceData>.Failure(
                "consultationSessionId source is only supported for ophthalmologist posts.");
        }

        if (!request.CurrentProfileId.HasValue)
        {
            return Result<ResolvedSourceData>.Failure("Authenticated ophthalmologist profile is required.");
        }

        var consultationSessionId = request.ConsultationSessionId!.Value;
        var session = await _consultationSessionRepository
            .Query()
            .AsNoTracking()
            .Select(s => new { s.Id, s.PatientId, s.OphthalmologistId, s.AiScreeningId })
            .FirstOrDefaultAsync(s => s.Id == consultationSessionId, cancellationToken);

        if (session is null)
        {
            return Result<ResolvedSourceData>.NotFound("Consultation session was not found.");
        }

        if (!session.OphthalmologistId.HasValue || session.OphthalmologistId.Value != request.CurrentProfileId.Value)
        {
            return Result<ResolvedSourceData>.Forbidden("You are not allowed to share this consultation case.");
        }

        AiScreening? screening = null;
        if (session.AiScreeningId.HasValue)
        {
            screening = await _aiScreeningRepository
                .Query()
                .AsNoTracking()
                .Include(x => x.ScreeningResults)
                .FirstOrDefaultAsync(x => x.Id == session.AiScreeningId.Value, cancellationToken);
        }

        var diagnosis = await _medicalDiagnosisRepository
            .Query()
            .AsNoTracking()
            .Where(d => d.ConsultationSessionId == session.Id)
            .OrderByDescending(d => d.FinalizedAt ?? d.CreatedAt)
            .ThenByDescending(d => d.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var latestResult = screening?.ScreeningResults
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefault();

        var (age, gender) = await GetPatientDemographicsAsync(session.PatientId, cancellationToken);
        var content = BuildConsultationShareContent(
            age,
            gender,
            diagnosis,
            latestResult,
            request.Content);

        return Result<ResolvedSourceData>.Success(new ResolvedSourceData(
            content,
            request.OrganisationId,
            true,
            session.Id,
            screening?.Id,
            age,
            gender,
            screening?.Id));
    }

    private async Task<Result<ResolvedSourceData>> ResolveFromAiScreeningSourceAsync(
        CreatePostCommand request,
        CancellationToken cancellationToken)
    {
        if (request.AuthorType != AuthorType.Organisation)
        {
            return Result<ResolvedSourceData>.Failure(
                "aiScreeningId source is only supported for organisation posts.");
        }

        var user = await _identityService.GetUserByIdAsync(request.AuthorId, cancellationToken);
        if (user?.OrganizationId is null)
        {
            return Result<ResolvedSourceData>.Failure(
                "Authenticated organisation admin context is required to share this screening case.");
        }

        var aiScreeningId = request.AiScreeningId!.Value;
        var screening = await _aiScreeningRepository
            .Query()
            .AsNoTracking()
            .Include(x => x.ScreeningResults)
            .FirstOrDefaultAsync(x => x.Id == aiScreeningId, cancellationToken);

        if (screening is null)
        {
            return Result<ResolvedSourceData>.NotFound("AI screening was not found.");
        }

        var isManagedByOrganisation = await _organisationPatientsRepository
            .IsPatientManagedByOrganisationAdminAsync(request.AuthorId, screening.PatientId, cancellationToken);

        if (!isManagedByOrganisation)
        {
            return Result<ResolvedSourceData>.Forbidden(
                "You are not allowed to share this screening case for the selected patient.");
        }

        var latestResult = screening.ScreeningResults
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefault();

        var (age, gender) = await GetPatientDemographicsAsync(screening.PatientId, cancellationToken);
        var content = BuildOrganisationShareContent(age, gender, latestResult, request.Content);

        return Result<ResolvedSourceData>.Success(new ResolvedSourceData(
            content,
            user.OrganizationId,
            false,
            null,
            screening.Id,
            age,
            gender,
            screening.Id));
    }

    private async Task AddRetinalAttachmentsFromAiScreeningAsync(
        ProfessionalPost post,
        Guid aiScreeningId,
        CancellationToken cancellationToken)
    {
        var screening = await _aiScreeningRepository
            .Query()
            .AsNoTracking()
            .Include(s => s.RetinalImages)
            .FirstOrDefaultAsync(s => s.Id == aiScreeningId, cancellationToken);

        if (screening is null)
        {
            return;
        }

        var imageUrls = screening.RetinalImages
            .OrderBy(i => i.CapturedAt)
            .ThenBy(i => i.CreatedAt)
            .Select(i => i.ImageUrl)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        for (var i = 0; i < imageUrls.Count; i++)
        {
            var attachment = new PostAttachment(
                post.Id,
                AttachmentType.Image,
                $"retinal-image-{i + 1}.jpg",
                imageUrls[i],
                "image/jpeg",
                null,
                i);

            post.AddAttachment(attachment);
        }
    }

    private async Task<(int? Age, string? Gender)> GetPatientDemographicsAsync(
        Guid patientId,
        CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
        if (patient is null)
        {
            return (null, null);
        }

        if (patient.IsWalkIn)
        {
            return (CalculateAge(patient.DateOfBirth), patient.GenderId switch
            {
                1 => "Male",
                2 => "Female",
                _ => "Other"
            });
        }

        if (!patient.UserId.HasValue)
        {
            return (null, null);
        }

        var userDetails = await _identityService.GetUserDetailsAsync(patient.UserId.Value, cancellationToken);
        return (CalculateAge(userDetails?.DateOfBirth), userDetails?.Gender?.ToString());
    }

    private static string BuildConsultationShareContent(
        int? patientAge,
        string? patientGender,
        MedicalDiagnosis? diagnosis,
        ScreeningResult? latestResult,
        string? notes)
    {
        var sections = new List<string>
        {
            "Internal clinical case shared from consultation."
        };

        if (patientAge.HasValue || !string.IsNullOrWhiteSpace(patientGender))
        {
            sections.Add($"Patient profile: Age {patientAge?.ToString() ?? "N/A"}, Gender {patientGender ?? "N/A"}.");
        }

        if (diagnosis is not null)
        {
            var diagnosisLines = new List<string> { "Medical diagnosis:" };

            if (!string.IsNullOrWhiteSpace(diagnosis.DiagnosisCode))
                diagnosisLines.Add($"- Diagnosis code: {diagnosis.DiagnosisCode}");
            if (!string.IsNullOrWhiteSpace(diagnosis.ClinicalFindings))
                diagnosisLines.Add($"- Clinical findings: {diagnosis.ClinicalFindings}");
            if (!string.IsNullOrWhiteSpace(diagnosis.SeverityLevel))
                diagnosisLines.Add($"- Severity level: {diagnosis.SeverityLevel}");
            if (diagnosis.ConfidenceLevel.HasValue)
                diagnosisLines.Add($"- Confidence level: {diagnosis.ConfidenceLevel.Value:0.##}%");
            if (!string.IsNullOrWhiteSpace(diagnosis.TreatmentPlan))
                diagnosisLines.Add($"- Treatment plan: {diagnosis.TreatmentPlan}");
            if (!string.IsNullOrWhiteSpace(diagnosis.Recommendations))
                diagnosisLines.Add($"- Recommendations: {diagnosis.Recommendations}");

            sections.Add(string.Join(Environment.NewLine, diagnosisLines));
        }
        else if (latestResult is not null)
        {
            var aiLines = new List<string>
            {
                $"AI risk level: {latestResult.RiskLevel}.",
                $"AI confidence: {latestResult.ConfidenceScore:0.##}%."
            };

            if (!string.IsNullOrWhiteSpace(latestResult.Summary))
                aiLines.Add($"Summary: {latestResult.Summary}");
            if (!string.IsNullOrWhiteSpace(latestResult.Findings))
                aiLines.Add($"Findings: {latestResult.Findings}");

            sections.Add(string.Join(Environment.NewLine, aiLines));
        }

        var normalizedNotes = notes?.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedNotes))
        {
            sections.Add($"Doctor notes:{Environment.NewLine}{normalizedNotes}");
        }

        sections.Add("Patient identity has been masked for professional discussion.");

        return string.Join(Environment.NewLine + Environment.NewLine, sections);
    }

    private static string BuildOrganisationShareContent(
        int? patientAge,
        string? patientGender,
        ScreeningResult? latestResult,
        string? notes)
    {
        var sections = new List<string>
        {
            "Organisation walk-in screening case shared for professional discussion."
        };

        if (patientAge.HasValue || !string.IsNullOrWhiteSpace(patientGender))
        {
            sections.Add($"Patient profile: Age {patientAge?.ToString() ?? "N/A"}, Gender {patientGender ?? "N/A"}.");
        }

        if (latestResult is not null)
        {
            var aiLines = new List<string>
            {
                $"AI risk level: {latestResult.RiskLevel}.",
                $"AI confidence: {latestResult.ConfidenceScore:0.##}%."
            };

            if (!string.IsNullOrWhiteSpace(latestResult.Summary))
                aiLines.Add($"Summary: {latestResult.Summary}");
            if (!string.IsNullOrWhiteSpace(latestResult.Findings))
                aiLines.Add($"Findings: {latestResult.Findings}");

            sections.Add(string.Join(Environment.NewLine, aiLines));
        }

        var normalizedNotes = notes?.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedNotes))
        {
            sections.Add($"Organisation notes:{Environment.NewLine}{normalizedNotes}");
        }

        sections.Add("Patient identity has been masked for professional discussion.");

        return string.Join(Environment.NewLine + Environment.NewLine, sections);
    }

    private static int? CalculateAge(DateTime? dateOfBirth)
    {
        if (!dateOfBirth.HasValue)
        {
            return null;
        }

        var today = DateTime.UtcNow.Date;
        var dob = dateOfBirth.Value.Date;

        if (dob > today)
        {
            return null;
        }

        var age = today.Year - dob.Year;
        if (dob > today.AddYears(-age))
        {
            age--;
        }

        return age < 0 ? null : age;
    }

    private static Result<T> MapSourceFailure<T>(Result<ResolvedSourceData> sourceResult)
    {
        if (sourceResult.IsForbidden)
            return Result<T>.Forbidden(sourceResult.ErrorMessage);

        if (sourceResult.IsUnauthorized)
            return Result<T>.Unauthorized(sourceResult.ErrorMessage);

        if (sourceResult.IsNotFound)
            return Result<T>.NotFound(sourceResult.ErrorMessage);

        if (sourceResult.IsConflict)
            return Result<T>.Conflict(sourceResult.ErrorMessage);

        if (sourceResult.IsPaymentRequired)
            return Result<T>.PaymentRequired(sourceResult.ErrorMessage);

        return Result<T>.Failure(sourceResult.ErrorMessage);
    }

    private sealed record ResolvedSourceData(
        string Content,
        Guid? OrganisationId,
        bool IsInternalCase,
        Guid? ConsultationSessionId,
        Guid? AiScreeningId,
        int? PatientAge,
        string? PatientGender,
        Guid? AttachmentAiScreeningId);
}
