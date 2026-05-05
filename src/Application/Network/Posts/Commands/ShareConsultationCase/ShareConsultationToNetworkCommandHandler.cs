using System.Text;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums.Network;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Network.Posts.Commands.ShareConsultationCase;

/// <summary>
/// Creates a professional network post from an internal consultation session.
/// </summary>
public class ShareConsultationToNetworkCommandHandler : ICommandHandler<ShareConsultationToNetworkCommand, Guid>
{
    private readonly IConsultationSessionRepository _consultationSessionRepository;
    private readonly IRepository<AiScreening> _aiScreeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ShareConsultationToNetworkCommandHandler(
        IConsultationSessionRepository consultationSessionRepository,
        IRepository<AiScreening> aiScreeningRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService,
        IPostRepository postRepository,
        IUnitOfWork unitOfWork)
    {
        _consultationSessionRepository = consultationSessionRepository;
        _aiScreeningRepository = aiScreeningRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        ShareConsultationToNetworkCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _consultationSessionRepository
            .Query()
            .FirstOrDefaultAsync(s => s.Id == request.ConsultationSessionId, cancellationToken);

        if (session is null)
        {
            return Result<Guid>.NotFound("Consultation session was not found.");
        }

        if (!session.OphthalmologistId.HasValue || session.OphthalmologistId.Value != request.CurrentProfileId)
        {
            return Result<Guid>.Forbidden("You are not allowed to share this consultation case.");
        }

        AiScreening? screening = null;
        if (session.AiScreeningId.HasValue)
        {
            screening = await _aiScreeningRepository
                .Query()
                .Include(x => x.RetinalImages)
                .Include(x => x.ScreeningResults)
                .FirstOrDefaultAsync(x => x.Id == session.AiScreeningId.Value, cancellationToken);
        }

        var latestResult = screening?.ScreeningResults
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault();

        var patient = await _patientRepository.GetByIdAsync(session.PatientId, cancellationToken);

        int? patientAge;
        string? patientGender;

        if (patient is not null && patient.IsWalkIn)
        {
            patientAge = CalculateAge(patient.DateOfBirth);
            patientGender = patient.GenderId switch
            {
                1 => "Male",
                2 => "Female",
                _ => "Other"
            };
        }
        else if (patient is not null && patient.UserId.HasValue)
        {
            var userDetails = await _identityService.GetUserDetailsAsync(patient.UserId.Value, cancellationToken);
            patientAge = CalculateAge(userDetails?.DateOfBirth);
            patientGender = userDetails?.Gender?.ToString();
        }
        else
        {
            patientAge = null;
            patientGender = null;
        }

        var post = new ProfessionalPost(
            request.CurrentUserId,
            AuthorType.Ophthalmologist,
            BuildShareContent(patientAge, patientGender, latestResult, request.DoctorNote, request.AiSummary, request.FinalDiagnosis),
            PostCategory.CasePresentation,
            allowComments: true);

        post.SetClinicalCaseMetadata(
            isInternalCase: true,
            consultationSessionId: session.Id,
            aiScreeningId: session.AiScreeningId,
            patientAge: patientAge,
            patientGender: patientGender);

        if (screening is not null)
        {
            AddRetinalImageAttachments(post, screening.RetinalImages);
        }

        await _postRepository.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(post.Id);
    }

    private static string BuildShareContent(
        int? patientAge,
        string? patientGender,
        ScreeningResult? latestResult,
        string? doctorNote = null,
        string? aiSummary = null,
        string? finalDiagnosis = null)
    {
        var builder = new StringBuilder();
        
        if (!string.IsNullOrWhiteSpace(doctorNote))
        {
            builder.AppendLine(doctorNote);
            builder.AppendLine();
        }

        builder.AppendLine("Internal clinical case shared from consultation.");

        if (patientAge.HasValue || !string.IsNullOrWhiteSpace(patientGender))
        {
            builder.AppendLine($"Patient profile: Age {patientAge?.ToString() ?? "N/A"}, Gender {patientGender ?? "N/A"}.");
        }

        if (latestResult is not null || !string.IsNullOrWhiteSpace(aiSummary) || !string.IsNullOrWhiteSpace(finalDiagnosis))
        {
            if (latestResult is not null)
            {
                builder.AppendLine($"AI risk level: {latestResult.RiskLevel}.");
                builder.AppendLine($"AI confidence: {latestResult.ConfidenceScore:0.##}%.");
            }

            var summary = !string.IsNullOrWhiteSpace(aiSummary) ? aiSummary : latestResult?.Summary;
            if (!string.IsNullOrWhiteSpace(summary))
            {
                builder.AppendLine($"Summary: {summary}");
            }

            var findings = !string.IsNullOrWhiteSpace(finalDiagnosis) ? finalDiagnosis : latestResult?.Findings;
            if (!string.IsNullOrWhiteSpace(findings))
            {
                builder.AppendLine($"Findings: {findings}");
            }
        }

        builder.AppendLine("Patient identity has been masked for professional discussion.");
        return builder.ToString().Trim();
    }

    private static void AddRetinalImageAttachments(ProfessionalPost post, IEnumerable<RetinalImage> retinalImages)
    {
        var orderedUrls = retinalImages
            .OrderBy(x => x.CapturedAt)
            .ThenBy(x => x.CreatedAt)
            .Select(x => x.ImageUrl)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        for (var i = 0; i < orderedUrls.Count; i++)
        {
            var fileUrl = orderedUrls[i];
            var attachment = new PostAttachment(
                post.Id,
                AttachmentType.Image,
                $"retinal-image-{i + 1}.jpg",
                fileUrl,
                "image/jpeg",
                null,
                i);

            post.AddAttachment(attachment);
        }
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
}
