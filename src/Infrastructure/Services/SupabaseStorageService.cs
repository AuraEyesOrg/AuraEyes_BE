using System.Text.RegularExpressions;
using Application.Common.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Supabase;

namespace Infrastructure.Services;

/// <summary>
/// Supabase Storage file service.
/// Uses service_role key to bypass RLS policies.
/// </summary>
public sealed class SupabaseStorageService : IFileStorageService
{
    private readonly SupabaseStorageSettings _settings;
    private readonly ILogger<SupabaseStorageService> _logger;

    // Lazily initialised – reused across all calls within the same DI scope so
    // concurrent uploads (e.g. licenseImage + degreeImage) share one connection.
    private Client? _supabaseClient;

    public SupabaseStorageService(
        IOptions<SupabaseStorageSettings> settings,
        ILogger<SupabaseStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        _logger.LogInformation(
            "SupabaseStorageService initialised – Url={Url}, Bucket={Bucket}, HasKey={HasKey}",
            _settings.Url, _settings.BucketName, !string.IsNullOrEmpty(_settings.ServiceKey));
    }

    // ── Lazy async initialisation ───────────────────────────────────────────

    private async Task<Client> GetSupabaseClientAsync()
    {
        if (_supabaseClient is not null)
            return _supabaseClient;

        var client = new Client(_settings.Url, _settings.ServiceKey, new SupabaseOptions
        {
            AutoConnectRealtime = false
        });
        await client.InitializeAsync();
        _supabaseClient = client;
        return _supabaseClient;
    }

    /// <summary>
    /// Sync helper used by <see cref="DeleteFile"/> and <see cref="FileExists"/>
    /// which cannot be made async without changing the <see cref="IFileStorageService"/> contract.
    /// Runs the async init on the thread pool to avoid ASP.NET sync-context deadlocks.
    /// </summary>
    private Client GetSupabaseClientSync()
        => _supabaseClient ?? Task.Run(GetSupabaseClientAsync).GetAwaiter().GetResult();

    /// <inheritdoc />
    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        string subFolder,
        CancellationToken cancellationToken = default)
    {
        var safeFileName = SanitizeFileName(fileName);
        var storagePath = string.IsNullOrWhiteSpace(subFolder)
            ? $"{Guid.NewGuid():N}_{safeFileName}"
            : $"{subFolder.TrimEnd('/')}/{Guid.NewGuid():N}_{safeFileName}";

        // Convert stream to byte array (Supabase SDK requires byte[])
        if (fileStream.CanSeek)
            fileStream.Position = 0;

        byte[] fileBytes;
        using (var ms = new MemoryStream())
        {
            await fileStream.CopyToAsync(ms, cancellationToken);
            fileBytes = ms.ToArray();
        }

        if (fileBytes.Length == 0)
            throw new InvalidOperationException("File stream is empty, cannot upload.");

        _logger.LogDebug(
            "Uploading to Supabase Storage – Bucket={Bucket}, Path={Path}, Size={Size} bytes",
            _settings.BucketName, storagePath, fileBytes.Length);

        var supabase = await GetSupabaseClientAsync();

        // Upload file
        await supabase.Storage
            .From(_settings.BucketName)
            .Upload(fileBytes, storagePath, new Supabase.Storage.FileOptions
            {
                ContentType = GetContentType(safeFileName),
                Upsert = true
            });

        // Get public URL
        var publicUrl = supabase.Storage
            .From(_settings.BucketName)
            .GetPublicUrl(storagePath);

        // Fallback: build URL manually if SDK returns relative path
        if (!string.IsNullOrEmpty(publicUrl) && !publicUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            publicUrl = $"{_settings.Url}/storage/v1/object/public/{_settings.BucketName}/{storagePath}";
        }

        _logger.LogInformation("Uploaded to Supabase Storage: {Url}", publicUrl);
        return publicUrl;
    }

    /// <inheritdoc />
    public bool DeleteFile(string relativePath)
    {
        try
        {
            var filePath = ExtractStoragePath(relativePath);
            if (string.IsNullOrEmpty(filePath)) return false;

            var supabase = GetSupabaseClientSync();

            supabase.Storage
                .From(_settings.BucketName)
                .Remove(new List<string> { filePath })
                .GetAwaiter().GetResult();

            _logger.LogInformation("Deleted file from Supabase Storage: {Path}", filePath);
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
        // Supabase SDK doesn't have a direct "exists" check.
        // We attempt to get a public URL — if the file was uploaded, the URL is valid.
        // For a more robust check, we could do a HEAD request to the public URL.
        try
        {
            var filePath = ExtractStoragePath(relativePath);
            if (string.IsNullOrEmpty(filePath)) return false;

            // Use public URL check – no need to init Supabase SDK for a HEAD request.
            using var httpClient = new HttpClient();
            var url = $"{_settings.Url}/storage/v1/object/public/{_settings.BucketName}/{filePath}";
            var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).GetAwaiter().GetResult();
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error checking file existence: {Path}", relativePath);
            return false;
        }
    }

    /// <summary>
    /// Extract the storage path from a full Supabase public URL or a plain path.
    /// </summary>
    private string ExtractStoragePath(string pathOrUrl)
    {
        if (string.IsNullOrWhiteSpace(pathOrUrl)) return string.Empty;

        var marker = $"/object/public/{_settings.BucketName}/";
        var idx = pathOrUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx >= 0)
            return pathOrUrl[(idx + marker.Length)..];

        return pathOrUrl;
    }

    /// <summary>
    /// Sanitize file name — remove spaces and special characters.
    /// </summary>
    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var safe = Regex.Replace(name, @"[^a-zA-Z0-9_\-]", "_");
        return $"{safe}{ext}";
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
}
