using FluentAssertions;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Infrastructure.UnitTests.Services;

public class EmailServiceValidationTests
{
    private static EmailService CreateSut()
    {
        var settings = Options.Create(new SmtpSettings
        {
            Host = "localhost",
            Port = 25,
            FromEmail = "noreply@test.local",
            FromName = "Test"
        });
        var logger = Substitute.For<ILogger<EmailService>>();
        return new EmailService(settings, logger);
    }

    [Fact]
    public async Task SendAsync_EmptyTo_ShouldThrow()
    {
        var sut = CreateSut();

        var act = async () => await sut.SendAsync("", "subj", "body", true, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendAsync_EmptySubject_ShouldThrow()
    {
        var sut = CreateSut();

        var act = async () => await sut.SendAsync("a@b.c", "", "body", true, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendAsync_EmptyBody_ShouldThrow()
    {
        var sut = CreateSut();

        var act = async () => await sut.SendAsync("a@b.c", "subj", "", true, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }
}
