using Application.Common.Interfaces;

namespace Application.Patients.Commands.UploadAvatar;

/// <summary>
/// Command to upload and update patient avatar.
/// </summary>
public record UploadAvatarCommand : ICommand<UploadAvatarResponse>
{
    public Guid UserId { get; init; }
    public Stream FileStream { get; init; } = null!;
    public string FileName { get; init; } = string.Empty;
}

public record UploadAvatarResponse
{
    public string AvatarUrl { get; init; } = string.Empty;
}
