using FluentAssertions;
using Infrastructure.Services;
using Infrastructure.Settings;
using Infrastructure.UnitTests.Common;
using Microsoft.Extensions.Options;

namespace Infrastructure.UnitTests.Services;

public class EmailServiceTests
{
    [Theory]
    [InlineData("", "subject", "body")]
    [InlineData(" ", "subject", "body")]
    [InlineData(null, "subject", "body")]
    [InlineData("to@test.local", "", "body")]
    [InlineData("to@test.local", " ", "body")]
    [InlineData("to@test.local", null, "body")]
    [InlineData("to@test.local", "subject", "")]
    [InlineData("to@test.local", "subject", " ")]
    [InlineData("to@test.local", "subject", null)]
    [InlineData("", "", "")]
    public async Task SendAsync_WithInvalidArguments_ShouldThrowArgumentException(string? to, string? subject, string? body)
    {
        var service = CreateService();

        var act = async () => await service.SendAsync(to!, subject!, body!, isHtml: true);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("bad-email")]
    [InlineData("no-at-sign")]
    [InlineData("@domain-only.com")]
    [InlineData("local-only@")]
    [InlineData("test@@domain.com")]
    [InlineData("name withspace@domain.com")]
    [InlineData("name@domain")]
    [InlineData("name@.com")]
    public async Task SendEmailConfirmationAsync_WithInvalidRecipient_ShouldThrow(string email)
    {
        var service = CreateService();

        var act = async () => await service.SendEmailConfirmationAsync(email, "https://example.com/confirm");

        await act.Should().ThrowAsync<Exception>();
    }

    private static EmailService CreateService()
    {
        var settings = new SmtpSettings
        {
            Host = "localhost",
            Port = 2525,
            UseSsl = false,
            UseStartTls = false,
            FromName = "AuraEyes Test",
            FromEmail = "noreply@test.local"
        };
        return new EmailService(Options.Create(settings), new TestLogger<EmailService>());
    }
}
