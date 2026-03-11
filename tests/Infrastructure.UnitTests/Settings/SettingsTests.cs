using FluentAssertions;
using Infrastructure.Settings;

namespace Infrastructure.UnitTests.Settings;

public class SmtpSettingsTests
{
    [Fact]
    public void SectionName_ShouldBeSmtp()
    {
        SmtpSettings.SectionName.Should().Be("Smtp");
    }

    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        var settings = new SmtpSettings();

        settings.Host.Should().BeEmpty();
        settings.Port.Should().Be(587);
        settings.Username.Should().BeEmpty();
        settings.Password.Should().BeEmpty();
        settings.UseSsl.Should().BeFalse();
        settings.UseStartTls.Should().BeTrue();
        settings.FromName.Should().Be("Hệ thống Aura – Quản lý khám sàng lọc mắt");
        settings.FromEmail.Should().BeEmpty();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var settings = new SmtpSettings
        {
            Host = "smtp.gmail.com",
            Port = 465,
            Username = "user@gmail.com",
            Password = "app-password",
            UseSsl = true,
            UseStartTls = false,
            FromName = "Custom Name",
            FromEmail = "custom@example.com"
        };

        settings.Host.Should().Be("smtp.gmail.com");
        settings.Port.Should().Be(465);
        settings.Username.Should().Be("user@gmail.com");
        settings.Password.Should().Be("app-password");
        settings.UseSsl.Should().BeTrue();
        settings.UseStartTls.Should().BeFalse();
        settings.FromName.Should().Be("Custom Name");
        settings.FromEmail.Should().Be("custom@example.com");
    }
}

public class PayOSSettingsTests
{
    [Fact]
    public void SectionName_ShouldBePayOS()
    {
        PayOSSettings.SectionName.Should().Be("PayOS");
    }

    [Fact]
    public void DefaultValues_ShouldBeEmpty()
    {
        var settings = new PayOSSettings();

        settings.ClientId.Should().BeEmpty();
        settings.ApiKey.Should().BeEmpty();
        settings.ChecksumKey.Should().BeEmpty();
        settings.DefaultReturnUrl.Should().BeEmpty();
        settings.DefaultCancelUrl.Should().BeEmpty();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var settings = new PayOSSettings
        {
            ClientId = "test-client-id",
            ApiKey = "test-api-key",
            ChecksumKey = "test-checksum-key",
            DefaultReturnUrl = "https://example.com/return",
            DefaultCancelUrl = "https://example.com/cancel"
        };

        settings.ClientId.Should().Be("test-client-id");
        settings.ApiKey.Should().Be("test-api-key");
        settings.ChecksumKey.Should().Be("test-checksum-key");
        settings.DefaultReturnUrl.Should().Be("https://example.com/return");
        settings.DefaultCancelUrl.Should().Be("https://example.com/cancel");
    }
}

public class SupabaseStorageSettingsTests
{
    [Fact]
    public void SectionName_ShouldBeSupabaseStorage()
    {
        SupabaseStorageSettings.SectionName.Should().Be("SupabaseStorage");
    }

    [Fact]
    public void DefaultValues_ShouldBeEmpty()
    {
        var settings = new SupabaseStorageSettings();

        settings.Url.Should().BeEmpty();
        settings.ServiceKey.Should().BeEmpty();
        settings.BucketName.Should().BeEmpty();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var settings = new SupabaseStorageSettings
        {
            Url = "https://xxxx.supabase.co",
            ServiceKey = "test-service-key",
            BucketName = "avatars"
        };

        settings.Url.Should().Be("https://xxxx.supabase.co");
        settings.ServiceKey.Should().Be("test-service-key");
        settings.BucketName.Should().Be("avatars");
    }
}
