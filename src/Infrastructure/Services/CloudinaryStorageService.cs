using Application.Common.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

/// <summary>
/// Cloudinary Storage file service.
/// Implementation of IFileStorageService using Cloudinary.
/// </summary>
public sealed class CloudinaryStorageService : IFileStorageService
{
    private readonly CloudinarySettings _settings;
    private readonly ILogger<CloudinaryStorageService> _logger;
    private readonly Cloudinary _cloudinary;

    public CloudinaryStorageService(
        IOptions<CloudinarySettings> settings,
        ILogger<CloudinaryStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        var account = new Account(
            _settings.CloudName,
            _settings.ApiKey,
            _settings.ApiSecret);

        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;

        _logger.LogInformation(
            "CloudinaryStorageService initialised – CloudName={CloudName}, Folder={Folder}",
            _settings.CloudName, _settings.Folder);
    }

    /// <inheritdoc />
    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        string subFolder,
        CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var safeFileName = Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_");
        
        // Combine folder and subfolder
        var folderPath = string.IsNullOrWhiteSpace(subFolder)
            ? _settings.Folder
            : $"{_settings.Folder}/{subFolder.Trim('/')}";

        // Ensure stream is at beginning
        if (fileStream.CanSeek)
            fileStream.Position = 0;

        // Determine resource type based on extension
        var isImage = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".tiff" }.Contains(extension);
        var isVideo = new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm" }.Contains(extension);

        RawUploadResult finalResult;

        if (isImage)
        {
            finalResult = await _cloudinary.UploadAsync(new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = folderPath,
                PublicId = $"{Guid.NewGuid():N}_{safeFileName}",
                Overwrite = true,
                AccessMode = "public"
            });
        }
        else if (isVideo)
        {
            finalResult = await _cloudinary.UploadAsync(new VideoUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = folderPath,
                PublicId = $"{Guid.NewGuid():N}_{safeFileName}",
                Overwrite = true,
                AccessMode = "public"
            });
        }
        else
        {
            // For DOCX, PDF, etc. - use Raw resource type
            finalResult = await _cloudinary.UploadAsync(new RawUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = folderPath,
                PublicId = $"{Guid.NewGuid():N}_{safeFileName}{extension}",
                Overwrite = true,
                AccessMode = "public"
            });
        }

        if (finalResult.Error != null)
        {
            _logger.LogError("Cloudinary upload failed: {Error}", finalResult.Error.Message);
            throw new Exception($"Cloudinary upload failed (Type: {finalResult.ResourceType}): {finalResult.Error.Message}");
        }

        _logger.LogInformation("Uploaded to Cloudinary ({Type}): {Url}", finalResult.ResourceType, finalResult.SecureUrl);
        return finalResult.SecureUrl.ToString();
    }

    /// <inheritdoc />
    public bool DeleteFile(string relativePath)
    {
        try
        {
            var publicId = ExtractPublicId(relativePath);
            if (string.IsNullOrEmpty(publicId)) return false;

            // Detect resource type from URL if possible, otherwise try all or default to image
            var resourceType = DetectResourceTypeFromUrl(relativePath);
            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = resourceType
            };
            
            var result = _cloudinary.Destroy(deletionParams);

            if (result.Result == "ok")
            {
                _logger.LogInformation("Deleted ({Type}) from Cloudinary: {PublicId}", resourceType, publicId);
                return true;
            }

            _logger.LogWarning("Failed to delete ({Type}) from Cloudinary: {PublicId}, Result: {Result}", resourceType, publicId, result.Result);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from Cloudinary: {Path}", relativePath);
            return false;
        }
    }

    /// <inheritdoc />
    public bool FileExists(string relativePath)
    {
        try
        {
            var publicId = ExtractPublicId(relativePath);
            if (string.IsNullOrEmpty(publicId)) return false;

            var resourceType = DetectResourceTypeFromUrl(relativePath);
            var getResourceParams = new GetResourceParams(publicId)
            {
                ResourceType = resourceType
            };
            
            var result = _cloudinary.GetResource(getResourceParams);
            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }

    private static ResourceType DetectResourceTypeFromUrl(string url)
    {
        if (url.Contains("/video/", StringComparison.OrdinalIgnoreCase)) return ResourceType.Video;
        if (url.Contains("/raw/", StringComparison.OrdinalIgnoreCase)) return ResourceType.Raw;
        return ResourceType.Image;
    }

    /// <summary>
    /// Extract the public ID from a Cloudinary URL.
    /// Format: https://res.cloudinary.com/{cloud_name}/{resource_type}/upload/v{version}/{folder}/{public_id}.{format}
    /// </summary>
    private string ExtractPublicId(string urlOrPath)
    {
        if (string.IsNullOrWhiteSpace(urlOrPath)) return string.Empty;

        if (!urlOrPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return urlOrPath;

        try
        {
            var uri = new Uri(urlOrPath);
            var segments = uri.Segments;
            
            // Find "upload/" segment
            int uploadIdx = -1;
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].Equals("upload/", StringComparison.OrdinalIgnoreCase))
                {
                    uploadIdx = i;
                    break;
                }
            }

            if (uploadIdx == -1 || uploadIdx >= segments.Length - 1)
                return urlOrPath;

            // Skip version segment if present (e.g., v123456789/)
            int startIdx = uploadIdx + 1;
            if (segments[startIdx].StartsWith("v") && 
                segments[startIdx].Length > 1 && 
                char.IsDigit(segments[startIdx][1]))
            {
                startIdx++;
            }

            if (startIdx >= segments.Length)
                return urlOrPath;

            // Join remaining segments (folder + filename)
            var publicIdWithExt = string.Join("", segments.Skip(startIdx)).Trim('/');
            
            // For 'raw' files, we don't want to remove the extension as it's part of the public ID
            if (DetectResourceTypeFromUrl(urlOrPath) == ResourceType.Raw)
            {
                return publicIdWithExt;
            }

            // For image/video, remove extension
            var lastDotIdx = publicIdWithExt.LastIndexOf('.');
            return lastDotIdx > 0 ? publicIdWithExt[..lastDotIdx] : publicIdWithExt;
        }
        catch
        {
            return urlOrPath;
        }
    }
}
