namespace Infrastructure.Settings;

public sealed class AdminNotificationSettings
{
    public const string SectionName = "AdminNotifications";

    public string? OrganisationOnboardingEmail { get; set; }
}