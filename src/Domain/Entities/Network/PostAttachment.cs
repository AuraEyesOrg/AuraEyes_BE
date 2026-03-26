using Domain.Common;
using Domain.Enums.Network;

namespace Domain.Entities.Network;

/// <summary>
/// Attachment on a professional post
/// </summary>
public class PostAttachment : BaseEntity
{
    /// <summary>
    /// Post that this attachment belongs to
    /// </summary>
    public Guid PostId { get; private set; }

    /// <summary>
    /// Type of attachment
    /// </summary>
    public AttachmentType Type { get; private set; }

    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; private set; } = string.Empty;

    /// <summary>
    /// URL to the file
    /// </summary>
    public string FileUrl { get; private set; } = string.Empty;

    /// <summary>
    /// MIME type of the file
    /// </summary>
    public string? MimeType { get; private set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long? FileSize { get; private set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; private set; }

    // Navigation property (within network module only)
    public virtual ProfessionalPost Post { get; private set; } = null!;

    private PostAttachment() { } // EF Core

    /// <summary>
    /// Create an attachment
    /// </summary>
    public PostAttachment(
        Guid postId,
        AttachmentType type,
        string fileName,
        string fileUrl,
        string? mimeType = null,
        long? fileSize = null,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));

        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("File URL cannot be empty", nameof(fileUrl));

        PostId = postId;
        Type = type;
        FileName = fileName;
        FileUrl = fileUrl;
        MimeType = mimeType;
        FileSize = fileSize;
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Update display order
    /// </summary>
    public void UpdateDisplayOrder(int order)
    {
        DisplayOrder = order;
        UpdatedAt = DateTime.UtcNow;
    }
}
