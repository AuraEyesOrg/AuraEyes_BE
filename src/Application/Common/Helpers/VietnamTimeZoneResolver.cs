namespace Application.Common.Helpers;

/// <summary>
/// Resolves and caches Vietnam time zone for cross-platform usage.
/// </summary>
public static class VietnamTimeZoneResolver
{
    private static readonly string[] VietnamTimeZoneIds =
    [
        "SE Asia Standard Time", // Windows
        "Asia/Ho_Chi_Minh"       // Linux/macOS (IANA)
    ];

    private static readonly Lazy<TimeZoneInfo> CachedTimeZone = new(ResolveInternal);

    public static TimeZoneInfo TimeZone => CachedTimeZone.Value;

    private static TimeZoneInfo ResolveInternal()
    {
        foreach (var timeZoneId in VietnamTimeZoneIds)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                // Try next ID.
            }
            catch (InvalidTimeZoneException)
            {
                // Try next ID.
            }
        }

        throw new InvalidOperationException(
            "Unable to resolve Vietnam time zone. Checked: SE Asia Standard Time, Asia/Ho_Chi_Minh.");
    }
}
