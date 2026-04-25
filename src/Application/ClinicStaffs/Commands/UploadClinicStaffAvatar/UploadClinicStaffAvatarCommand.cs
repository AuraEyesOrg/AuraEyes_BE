using Application.Common.Interfaces;

namespace Application.ClinicStaffs.Commands.UploadClinicStaffAvatar;

/// <summary>
/// Upload and assign avatar for current clinic staff user.
/// </summary>
public record UploadClinicStaffAvatarCommand : ICommand<UploadClinicStaffAvatarResponse>
{
    public Guid UserId { get; init; }
    public Stream FileStream { get; init; } = null!;
    public string FileName { get; init; } = string.Empty;
}

public record UploadClinicStaffAvatarResponse
{
    public string AvatarUrl { get; init; } = string.Empty;
}

