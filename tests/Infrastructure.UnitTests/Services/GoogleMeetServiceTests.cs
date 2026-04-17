using FluentAssertions;
using Infrastructure.Services;
using Infrastructure.Settings;
using Infrastructure.UnitTests.Common;
using Microsoft.Extensions.Options;

namespace Infrastructure.UnitTests.Services;

public class GoogleMeetServiceTests
{
    [Fact]
    public void Constructor_WithMinimalSettings_ShouldCreateInstance()
    {
        var service = CreateService();

        service.Should().NotBeNull();
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        var service = CreateService();

        var act = () =>
        {
            service.Dispose();
            service.Dispose();
        };

        act.Should().NotThrow();
    }

    private static GoogleMeetService CreateService()
    {
        var settings = new GoogleMeetSettings
        {
            ClientId = "client-id",
            ClientSecret = "client-secret",
            RefreshToken = "refresh-token",
            CalendarId = "primary",
            ApplicationName = "AuraEyes Tests"
        };
        return new GoogleMeetService(Options.Create(settings), new TestLogger<GoogleMeetService>());
    }
}
