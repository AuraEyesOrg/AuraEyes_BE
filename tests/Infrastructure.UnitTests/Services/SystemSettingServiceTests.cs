using Domain.Entities.Platform;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Infrastructure.UnitTests.Services;

public class SystemSettingServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetSettingAsync_WhenSettingExists_ShouldReturnValue()
    {
        await using var context = CreateContext();
        context.SystemSettings.Add(new SystemSetting("PAYOS_CLIENT_ID", "client-1"));
        await context.SaveChangesAsync();

        var service = new SystemSettingService(context, Substitute.For<ILogger<SystemSettingService>>());

        var value = await service.GetSettingAsync("PAYOS_CLIENT_ID");

        value.Should().Be("client-1");
    }

    [Fact]
    public async Task GetAllSettingsAsync_ShouldReturnDictionary()
    {
        await using var context = CreateContext();
        context.SystemSettings.Add(new SystemSetting("A", "1"));
        context.SystemSettings.Add(new SystemSetting("B", "2"));
        await context.SaveChangesAsync();

        var service = new SystemSettingService(context, Substitute.For<ILogger<SystemSettingService>>());

        var settings = await service.GetAllSettingsAsync();

        settings.Should().ContainKey("A").WhoseValue.Should().Be("1");
        settings.Should().ContainKey("B").WhoseValue.Should().Be("2");
    }

    [Fact]
    public async Task UpdateSettingsAsync_WhenNewAndExisting_ShouldUpsert()
    {
        await using var context = CreateContext();
        context.SystemSettings.Add(new SystemSetting("A", "1"));
        await context.SaveChangesAsync();

        var service = new SystemSettingService(context, Substitute.For<ILogger<SystemSettingService>>());

        await service.UpdateSettingsAsync(new Dictionary<string, string>
        {
            ["A"] = "10",
            ["B"] = "20"
        });

        (await context.SystemSettings.CountAsync()).Should().Be(2);
        (await context.SystemSettings.FirstAsync(x => x.Key == "A")).Value.Should().Be("10");
        (await context.SystemSettings.FirstAsync(x => x.Key == "B")).Value.Should().Be("20");
    }

    [Fact]
    public async Task UpdateSettingsAsync_EmptyDictionary_ShouldDoNothing()
    {
        await using var context = CreateContext();
        var service = new SystemSettingService(context, Substitute.For<ILogger<SystemSettingService>>());

        await service.UpdateSettingsAsync(new Dictionary<string, string>());

        (await context.SystemSettings.CountAsync()).Should().Be(0);
    }
}