using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network;
using Domain.Entities.Screening;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public CreatePostCommandHandler(
        IPostRepository postRepository,
        IConsultationSessionRepository consultationSessionRepository,
        IRepository<AiScreening> aiScreeningRepository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _postRepository = postRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _aiScreeningRepository = aiScreeningRepository;
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

        var post = new ProfessionalPost(
            request.AuthorId,
            request.AuthorType,
            request.Content,
            request.Category,
            request.OrganisationId,
            request.AllowComments);

        var hasClinicalMetadata =
            request.IsInternalCase
            || request.ConsultationSessionId.HasValue
            || request.PatientAge.HasValue
            || !string.IsNullOrWhiteSpace(request.PatientGender);

        if (hasClinicalMetadata)
        {
            post.SetClinicalCaseMetadata(
                request.IsInternalCase,
                request.ConsultationSessionId,
                request.PatientAge,
                request.PatientGender);
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
            request.IsInternalCase &&
            request.ConsultationSessionId.HasValue)
        {
            await AddRetinalAttachmentsFromConsultationAsync(
                post,
                request.ConsultationSessionId.Value,
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

    private async Task AddRetinalAttachmentsFromConsultationAsync(
        ProfessionalPost post,
        Guid consultationSessionId,
        CancellationToken cancellationToken)
    {
        var session = await _consultationSessionRepository
            .Query()
            .AsNoTracking()
            .Select(s => new { s.Id, s.AiScreeningId })
            .FirstOrDefaultAsync(s => s.Id == consultationSessionId, cancellationToken);

        if (session?.AiScreeningId is null)
        {
            return;
        }

        var screening = await _aiScreeningRepository
            .Query()
            .AsNoTracking()
            .Include(s => s.RetinalImages)
            .FirstOrDefaultAsync(s => s.Id == session.AiScreeningId.Value, cancellationToken);

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
}
