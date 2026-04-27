using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace Application.ClinicStaffs.Commands.UploadClinicStaffAvatar;

public class UploadClinicStaffAvatarCommandHandler : ICommandHandler<UploadClinicStaffAvatarCommand, UploadClinicStaffAvatarResponse>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<UploadClinicStaffAvatarCommandHandler> _logger;

    public UploadClinicStaffAvatarCommandHandler(
        IFileStorageService fileStorageService,
        IIdentityService identityService,
        ILogger<UploadClinicStaffAvatarCommandHandler> logger)
    {
        _fileStorageService = fileStorageService;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result<UploadClinicStaffAvatarResponse>> Handle(
        UploadClinicStaffAvatarCommand request,
        CancellationToken cancellationToken)
    {
        var avatarUrl = await _fileStorageService.SaveFileAsync(
            request.FileStream,
            request.FileName,
            $"clinic-staff/avatars/{request.UserId}",
            cancellationToken);

        var (succeeded, errors) = await _identityService.UpdateAvatarUrlAsync(
            request.UserId,
            avatarUrl,
            cancellationToken);

        if (!succeeded)
            return Result<UploadClinicStaffAvatarResponse>.Failure(errors);

        _logger.LogInformation("Clinic staff avatar uploaded for user {UserId}: {Url}", request.UserId, avatarUrl);

        return Result<UploadClinicStaffAvatarResponse>.Success(new UploadClinicStaffAvatarResponse
        {
            AvatarUrl = avatarUrl
        });
    }
}

