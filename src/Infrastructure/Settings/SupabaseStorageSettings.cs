namespace Infrastructure.Settings;

/// <summary>
/// Supabase Storage configuration.
/// Mapped from appsettings.json "SupabaseStorage" section.
/// Uses supabase-csharp SDK with service_role key (bypasses RLS).
/// </summary>
public sealed class SupabaseStorageSettings
{
    public const string SectionName = "SupabaseStorage";

    /// <summary>
    /// Supabase project URL (e.g. https://xxxx.supabase.co).
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Supabase service_role key (server-side only, bypasses RLS).
    /// </summary>
    public string ServiceKey { get; set; } = string.Empty;

    /// <summary>
    /// Supabase storage bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;
}
