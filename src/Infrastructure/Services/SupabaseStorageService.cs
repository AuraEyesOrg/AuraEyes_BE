using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

/// <summary>
/// Supabase S3-compatible file storage service.
/// Uses AWSSDK.S3 with Supabase's S3-compatible endpoint.
/// </summary>
public sealed class SupabaseStorageService : IFileStorageService, IDisposable
{
    private readonly AmazonS3Client _s3Client;
    private readonly SupabaseStorageSettings _settings;
    private readonly ILogger<SupabaseStorageService> _logger;

    public SupabaseStorageService(
        IOptions<SupabaseStorageSettings> settings,
        ILogger<SupabaseStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        var config = new AmazonS3Config
        {
            ServiceURL = _settings.Endpoint,
            ForcePathStyle = true
        };

        _s3Client = new AmazonS3Client(
            _settings.AccessKey,
            _settings.SecretKey,
            config);
    }

    /// <inheritdoc />
    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        string subFolder,
        CancellationToken cancellationToken = default)
    {
        // Generate unique key: subFolder/guid_filename
        var safeFileName = Path.GetFileName(fileName);
        var key = string.IsNullOrWhiteSpace(subFolder)
            ? $"{Guid.NewGuid():N}_{safeFileName}"
            : $"{subFolder.TrimEnd('/')}/{Guid.NewGuid():N}_{safeFileName}";

        var putRequest = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = GetContentType(safeFileName),
            DisablePayloadSigning = true
        };

        await _s3Client.PutObjectAsync(putRequest, cancellationToken);
        _logger.LogInformation("Uploaded file to Supabase S3: {Key}", key);

        // Return public URL
        return $"https://{_settings.ProjectRef}.supabase.co/storage/v1/object/public/{_settings.BucketName}/{key}";
    }

    /// <inheritdoc />
    public bool DeleteFile(string relativePath)
    {
        try
        {
            // Extract key from full URL or use as-is
            var key = ExtractKey(relativePath);
            if (string.IsNullOrEmpty(key)) return false;

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key
            };

            _s3Client.DeleteObjectAsync(deleteRequest).GetAwaiter().GetResult();
            _logger.LogInformation("Deleted file from Supabase S3: {Key}", key);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete file: {Path}", relativePath);
            return false;
        }
    }

    /// <inheritdoc />
    public bool FileExists(string relativePath)
    {
        try
        {
            var key = ExtractKey(relativePath);
            if (string.IsNullOrEmpty(key)) return false;

            var request = new GetObjectMetadataRequest
            {
                BucketName = _settings.BucketName,
                Key = key
            };

            _s3Client.GetObjectMetadataAsync(request).GetAwaiter().GetResult();
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error checking file existence: {Path}", relativePath);
            return false;
        }
    }

    /// <summary>
    /// Extract the S3 key from a full Supabase public URL or a plain key.
    /// </summary>
    private string ExtractKey(string pathOrUrl)
    {
        if (string.IsNullOrWhiteSpace(pathOrUrl)) return string.Empty;

        // If it's a full URL, extract the key after /object/public/{bucket}/
        var marker = $"/object/public/{_settings.BucketName}/";
        var idx = pathOrUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx >= 0)
        {
            return pathOrUrl[(idx + marker.Length)..];
        }

        return pathOrUrl;
    }

    private static string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }

    public void Dispose()
    {
        _s3Client.Dispose();
    }
}
