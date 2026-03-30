using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network;
using Domain.Enums.Network;
using Domain.Repositories;

namespace Application.Network.Posts.Commands.CreatePost;

/// <summary>
/// Handler for CreatePostCommand
/// </summary>
public class CreatePostCommandHandler : ICommandHandler<CreatePostCommand, Guid>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public CreatePostCommandHandler(
        IPostRepository postRepository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _postRepository = postRepository;
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
}
