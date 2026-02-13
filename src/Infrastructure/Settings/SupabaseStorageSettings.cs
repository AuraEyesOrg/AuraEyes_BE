namespace Infrastructure.Settings;

/// <summary>
/// Supabase S3 storage configuration.
/// Mapped from appsettings.json "SupabaseStorage" section.
/// </summary>
public sealed class SupabaseStorageSettings
{
    public const string SectionName = "SupabaseStorage";

    /// <summary>
    /// Supabase S3 endpoint URL.
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// S3 Access Key.
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// S3 Secret Key.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Supabase storage bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Supabase project reference ID (for building public URLs).
    /// </summary>
    public string ProjectRef { get; set; } = string.Empty;
}
