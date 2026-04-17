using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace Application.Patients.Commands.UploadAvatar;

public class UploadAvatarCommandHandler : ICommandHandler<UploadAvatarCommand, UploadAvatarResponse>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<UploadAvatarCommandHandler> _logger;

    public UploadAvatarCommandHandler(
        IFileStorageService fileStorageService,
        IIdentityService identityService,
        ILogger<UploadAvatarCommandHandler> logger)
    {
        _fileStorageService = fileStorageService;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result<UploadAvatarResponse>> Handle(
        UploadAvatarCommand request,
        CancellationToken cancellationToken)
    {
        // Upload file to storage
        var avatarUrl = await _fileStorageService.SaveFileAsync(
            request.FileStream,
            request.FileName,
            $"patients/avatars/{request.UserId}",
            cancellationToken);

        var (succeeded, errors) = await _identityService.UpdateAvatarUrlAsync(
            request.UserId,
            avatarUrl,
            cancellationToken);

        if (!succeeded)
            return Result<UploadAvatarResponse>.Failure(errors);

        _logger.LogInformation("Avatar uploaded for user {UserId}: {Url}", request.UserId, avatarUrl);

        return Result<UploadAvatarResponse>.Success(new UploadAvatarResponse
        {
            AvatarUrl = avatarUrl
        });
    }
}
