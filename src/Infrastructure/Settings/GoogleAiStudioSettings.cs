namespace Infrastructure.Settings;

public sealed class GoogleAiStudioSettings
{
    public const string SectionName = "GoogleAiStudio";

    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta/models";
    public string Model { get; set; } = "gemini-2.5-flash";
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 2;
    public int InitialBackoffMs { get; set; } = 500;
}
