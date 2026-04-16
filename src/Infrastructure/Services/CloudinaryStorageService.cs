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
        var safeFileName = Path.GetFileNameWithoutExtension(fileName)
            .Replace(" ", "_");
        
        // Combine folder and subfolder
        var folderPath = string.IsNullOrWhiteSpace(subFolder)
            ? _settings.Folder
            : $"{_settings.Folder}/{subFolder.Trim('/')}";

        // Ensure stream is at beginning
        if (fileStream.CanSeek)
            fileStream.Position = 0;

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = folderPath,
            // Use Guid to ensure uniqueness, similar to Supabase implementation
            PublicId = $"{Guid.NewGuid():N}_{safeFileName}",
            Overwrite = true,
            AccessMode = "public"
        };

        _logger.LogDebug(
            "Uploading to Cloudinary – Folder={Folder}, PublicId={PublicId}",
            uploadParams.Folder, uploadParams.PublicId);

        var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (uploadResult.Error != null)
        {
            _logger.LogError("Cloudinary upload failed: {Error}", uploadResult.Error.Message);
            throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");
        }

        _logger.LogInformation("Uploaded to Cloudinary: {Url}", uploadResult.SecureUrl);
        return uploadResult.SecureUrl.ToString();
    }

    /// <inheritdoc />
    public bool DeleteFile(string relativePath)
    {
        try
        {
            var publicId = ExtractPublicId(relativePath);
            if (string.IsNullOrEmpty(publicId)) return false;

            var deletionParams = new DeletionParams(publicId);
            var result = _cloudinary.Destroy(deletionParams);

            if (result.Result == "ok")
            {
                _logger.LogInformation("Deleted file from Cloudinary: {PublicId}", publicId);
                return true;
            }

            _logger.LogWarning("Failed to delete file from Cloudinary: {PublicId}, Result: {Result}", publicId, result.Result);
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

            var getResourceParams = new GetResourceParams(publicId);
            var result = _cloudinary.GetResource(getResourceParams);
            
            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Extract the public ID from a Cloudinary URL.
    /// Format: https://res.cloudinary.com/{cloud_name}/image/upload/v{version}/{public_id}.{format}
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

            // Join remaining segments and remove extension
            var publicIdWithExt = string.Join("", segments.Skip(startIdx)).Trim('/');
            var lastDotIdx = publicIdWithExt.LastIndexOf('.');
            
            return lastDotIdx > 0 ? publicIdWithExt[..lastDotIdx] : publicIdWithExt;
        }
        catch
        {
            return urlOrPath;
        }
    }
}
