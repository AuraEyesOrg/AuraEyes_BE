using FluentAssertions;
using Infrastructure.Settings;

namespace Infrastructure.UnitTests.Settings;

public class AdminNotificationSettingsTests
{
    [Fact]
    public void SectionName_ShouldBeAdminNotifications()
    {
        AdminNotificationSettings.SectionName.Should().Be("AdminNotifications");
    }

    [Fact]
    public void DefaultValues_ShouldBeNull()
    {
        var settings = new AdminNotificationSettings();

        settings.OrganisationOnboardingEmail.Should().BeNull();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var settings = new AdminNotificationSettings
        {
            OrganisationOnboardingEmail = "admin@auraeyes.vn"
        };

        settings.OrganisationOnboardingEmail.Should().Be("admin@auraeyes.vn");
    }
}

public class GoogleAiStudioSettingsTests
{
    [Fact]
    public void SectionName_ShouldBeGoogleAiStudio()
    {
        GoogleAiStudioSettings.SectionName.Should().Be("GoogleAiStudio");
    }

    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        var settings = new GoogleAiStudioSettings();

        settings.ApiKey.Should().BeEmpty();
        settings.BaseUrl.Should().Be("https://generativelanguage.googleapis.com/v1beta/models");
        settings.Model.Should().Be("gemini-2.5-flash");
        settings.TimeoutSeconds.Should().Be(30);
        settings.MaxRetries.Should().Be(2);
        settings.InitialBackoffMs.Should().Be(500);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var settings = new GoogleAiStudioSettings
        {
            ApiKey = "test-api-key",
            BaseUrl = "https://custom.endpoint.com/v1",
            Model = "gemini-pro",
            TimeoutSeconds = 60,
            MaxRetries = 5,
            InitialBackoffMs = 1000
        };

        settings.ApiKey.Should().Be("test-api-key");
        settings.BaseUrl.Should().Be("https://custom.endpoint.com/v1");
        settings.Model.Should().Be("gemini-pro");
        settings.TimeoutSeconds.Should().Be(60);
        settings.MaxRetries.Should().Be(5);
        settings.InitialBackoffMs.Should().Be(1000);
    }
}

public class GoogleAuthSettingsTests
{
    [Fact]
    public void SectionName_ShouldBeGoogleAuth()
    {
        GoogleAuthSettings.SectionName.Should().Be("GoogleAuth");
    }

    [Fact]
    public void DefaultValues_ShouldBeEmpty()
    {
        var settings = new GoogleAuthSettings();

        settings.ClientId.Should().BeEmpty();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var settings = new GoogleAuthSettings
        {
            ClientId = "123456789.apps.googleusercontent.com"
        };

        settings.ClientId.Should().Be("123456789.apps.googleusercontent.com");
    }
}

public class GoogleMeetSettingsTests
{
    [Fact]
    public void SectionName_ShouldBeGoogleMeet()
    {
        GoogleMeetSettings.SectionName.Should().Be("GoogleMeet");
    }

    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        var settings = new GoogleMeetSettings();

        settings.ClientId.Should().BeEmpty();
        settings.ClientSecret.Should().BeEmpty();
        settings.RefreshToken.Should().BeEmpty();
        settings.CalendarId.Should().Be("primary");
        settings.ApplicationName.Should().Be("AURA Telemedicine");
        settings.DefaultDurationMinutes.Should().Be(30);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var settings = new GoogleMeetSettings
        {
            ClientId = "meet-client-id",
            ClientSecret = "meet-client-secret",
            RefreshToken = "meet-refresh-token",
            CalendarId = "custom-calendar@group.calendar.google.com",
            ApplicationName = "Custom App",
            DefaultDurationMinutes = 45
        };

        settings.ClientId.Should().Be("meet-client-id");
        settings.ClientSecret.Should().Be("meet-client-secret");
        settings.RefreshToken.Should().Be("meet-refresh-token");
        settings.CalendarId.Should().Be("custom-calendar@group.calendar.google.com");
        settings.ApplicationName.Should().Be("Custom App");
        settings.DefaultDurationMinutes.Should().Be(45);
    }
}
