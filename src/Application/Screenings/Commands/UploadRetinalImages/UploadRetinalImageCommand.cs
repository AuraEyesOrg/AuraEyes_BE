using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Screenings.Commands.UploadRetinalImages;

/// <summary>
/// Command to upload and save retinal images for a screening.
/// Images are saved to Supabase storage and metadata to database.
/// </summary>
public record UploadRetinalImageCommand : ICommand<UploadRetinalImageResponse>
{
    /// <summary>
    /// Public URL of the image in Supabase storage
    /// </summary>
    public string ImageUrl { get; init; } = string.Empty;

    /// <summary>
    /// Which eye: Left (OS) or Right (OD)
    /// </summary>
    public EyeSide EyeSide { get; init; }

    /// <summary>
    /// Optional: screening session ID to associate image with
    /// </summary>
    public Guid? ScreeningId { get; init; }

    /// <summary>
    /// Optional: device name
    /// </summary>
    public string? DeviceName { get; init; }
}

/// <summary>
/// Response from uploading a retinal image
/// </summary>
public record UploadRetinalImageResponse
{
    /// <summary>
    /// The retinal image ID
    /// </summary>
    public Guid ImageId { get; init; }

    /// <summary>
    /// Image URL in Supabase
    /// </summary>
    public string ImageUrl { get; init; } = string.Empty;

    /// <summary>
    /// Eye side
    /// </summary>
    public string EyeSide { get; init; } = string.Empty;

    /// <summary>
    /// When saved
    /// </summary>
    public DateTime SavedAt { get; init; }
}
