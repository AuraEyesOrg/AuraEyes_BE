using FluentAssertions;
using Infrastructure.Identity;

namespace Infrastructure.UnitTests.Identity;

public class JwtSettingsTests
{
    [Fact]
    public void SectionName_ShouldBeJwtSettings()
    {
        JwtSettings.SectionName.Should().Be("JwtSettings");
    }

    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        var settings = new JwtSettings();

        settings.SecretKey.Should().BeEmpty();
        settings.Issuer.Should().BeEmpty();
        settings.Audience.Should().BeEmpty();
        settings.AccessTokenExpiryMinutes.Should().Be(15);
        settings.RefreshTokenExpiryDays.Should().Be(7);
        settings.ValidateIssuer.Should().BeTrue();
        settings.ValidateAudience.Should().BeTrue();
        settings.ValidateLifetime.Should().BeTrue();
        settings.ClockSkewSeconds.Should().Be(0);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var settings = new JwtSettings
        {
            SecretKey = "MySuperSecretKeyThatIsLongEnough123!",
            Issuer = "AuraEyesAPI",
            Audience = "AuraEyesClient",
            AccessTokenExpiryMinutes = 30,
            RefreshTokenExpiryDays = 14,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ClockSkewSeconds = 60
        };

        settings.SecretKey.Should().Be("MySuperSecretKeyThatIsLongEnough123!");
        settings.Issuer.Should().Be("AuraEyesAPI");
        settings.Audience.Should().Be("AuraEyesClient");
        settings.AccessTokenExpiryMinutes.Should().Be(30);
        settings.RefreshTokenExpiryDays.Should().Be(14);
        settings.ValidateIssuer.Should().BeFalse();
        settings.ValidateAudience.Should().BeFalse();
        settings.ValidateLifetime.Should().BeFalse();
        settings.ClockSkewSeconds.Should().Be(60);
    }
}
