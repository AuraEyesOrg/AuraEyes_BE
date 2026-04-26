using Application.Common.Interfaces;
using FluentAssertions;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Infrastructure.UnitTests.Services;

public class EmailServiceTests
{
    private readonly Mock<IOptions<SmtpSettings>> _settingsMock;
    private readonly Mock<ILogger<EmailService>> _loggerMock;
    private readonly SmtpSettings _settings;

    public EmailServiceTests()
    {
        _settings = new SmtpSettings
        {
            Host = "localhost",
            Port = 25,
            Username = "test",
            Password = "test",
            FromEmail = "no-reply@test.local",
            FromName = "AuraEyes"
        };
        _settingsMock = new Mock<IOptions<SmtpSettings>>();
        _settingsMock.Setup(s => s.Value).Returns(_settings);
        _loggerMock = new Mock<ILogger<EmailService>>();
    }

    [Fact]
    public void Constructor_WithValidSettings_ShouldNotThrow()
    {
        var service = new EmailService(_settingsMock.Object, _loggerMock.Object);
        service.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task SendAsync_WithInvalidRecipient_ShouldThrow(string? email)
    {
        var service = new EmailService(_settingsMock.Object, _loggerMock.Object);
        var act = async () => await service.SendAsync(email!, "subject", "body");
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendEmailConfirmationAsync_WithValidInput_ShouldNotThrow()
    {
        var service = new EmailService(_settingsMock.Object, _loggerMock.Object);
        var act = async () => await service.SendEmailConfirmationAsync("test@t.l", "https://confirm.url");
        await act.Should().NotThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendPasswordResetAsync_WithValidInput_ShouldNotThrow()
    {
        var service = new EmailService(_settingsMock.Object, _loggerMock.Object);
        var act = async () => await service.SendPasswordResetAsync("test@t.l", "https://reset.url");
        await act.Should().NotThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendStaffOnboardingEmailAsync_WithValidInput_ShouldNotThrow()
    {
        var service = new EmailService(_settingsMock.Object, _loggerMock.Object);
        var act = async () => await service.SendStaffOnboardingEmailAsync("test@t.l", "Staff Name", "Password123");
        await act.Should().NotThrowAsync<ArgumentException>();
    }
}
