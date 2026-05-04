using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Common.Models;

/// <summary>
/// Helper class for creating bilingual notification content (English and Vietnamese)
/// </summary>
public class NotificationMessage
{
    [JsonPropertyName("en")]
    public string En { get; set; } = string.Empty;

    [JsonPropertyName("vi")]
    public string Vi { get; set; } = string.Empty;

    public NotificationMessage() { }

    public NotificationMessage(string vi, string en)
    {
        Vi = vi;
        En = en;
    }

    /// <summary>
    /// Serializes the bilingual message to a JSON string
    /// </summary>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    /// <summary>
    /// Create a bilingual message from Vietnamese and English strings
    /// </summary>
    public static string Create(string vi, string en)
    {
        return new NotificationMessage(vi, en).ToString();
    }
}
