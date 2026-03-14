using Domain.Enums.Network;

namespace Application.Network.Common;

/// <summary>
/// Attachment DTO
/// </summary>
public class AttachmentDto
{
    public Guid Id { get; set; }
    public AttachmentType Type { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? MimeType { get; set; }
    public long? FileSize { get; set; }
    public int DisplayOrder { get; set; }
}
