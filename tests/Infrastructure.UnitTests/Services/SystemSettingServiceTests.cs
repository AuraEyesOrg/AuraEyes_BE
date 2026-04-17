using Domain.Entities.Platform;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.UnitTests.Services;

public class SystemSettingServiceTests
{
    [Fact]
    public async Task GetSettingAsync_WhenKeyExists_ShouldReturnValue()
    {
        await using var context = CreateContext();
        await context.SystemSettings.AddAsync(new SystemSetting("FREE_AI_QUOTA", "3"));
        await context.SaveChangesAsync();

        var logger = new TestLogger<SystemSettingService>();
        var service = new SystemSettingService(context, logger);

        var value = await service.GetSettingAsync("FREE_AI_QUOTA");

        value.Should().Be("3");
    }

    [Fact]
    public async Task GetSettingAsync_WhenKeyNotExists_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var logger = new TestLogger<SystemSettingService>();
        var service = new SystemSettingService(context, logger);

        var value = await service.GetSettingAsync("NOT_EXISTS");

        value.Should().BeNull();
    }

    [Fact]
    public async Task GetAllSettingsAsync_ShouldReturnDictionaryWithAllValues()
    {
        await using var context = CreateContext();
        await context.SystemSettings.AddRangeAsync(
            new SystemSetting("FREE_AI_QUOTA", "3"),
            new SystemSetting("AI_QUOTA_UNIT_PRICE", "10000"));
        await context.SaveChangesAsync();

        var logger = new TestLogger<SystemSettingService>();
        var service = new SystemSettingService(context, logger);

        var result = await service.GetAllSettingsAsync();

        result.Should().HaveCount(2);
        result["FREE_AI_QUOTA"].Should().Be("3");
        result["AI_QUOTA_UNIT_PRICE"].Should().Be("10000");
    }

    [Fact]
    public async Task UpdateSettingsAsync_ShouldUpdateExistingAndInsertNew_AndWriteInfoLog()
    {
        await using var context = CreateContext();
        await context.SystemSettings.AddAsync(new SystemSetting("FREE_AI_QUOTA", "3"));
        await context.SaveChangesAsync();

        var logger = new TestLogger<SystemSettingService>();
        var service = new SystemSettingService(context, logger);

        await service.UpdateSettingsAsync(new Dictionary<string, string>
        {
            ["FREE_AI_QUOTA"] = "5",
            ["AI_QUOTA_UNIT_PRICE"] = "12000"
        });

        var free = await context.SystemSettings.FirstAsync(x => x.Key == "FREE_AI_QUOTA");
        var price = await context.SystemSettings.FirstAsync(x => x.Key == "AI_QUOTA_UNIT_PRICE");
        free.Value.Should().Be("5");
        price.Value.Should().Be("12000");
        logger.Entries.Should().Contain(e =>
            e.Level == LogLevel.Information &&
            e.Message.Contains("System settings updated for keys"));
    }

    [Fact]
    public async Task UpdateSettingsAsync_WhenInputEmpty_ShouldDoNothingAndNotLog()
    {
        await using var context = CreateContext();
        var logger = new TestLogger<SystemSettingService>();
        var service = new SystemSettingService(context, logger);

        await service.UpdateSettingsAsync(new Dictionary<string, string>());

        context.SystemSettings.Count().Should().Be(0);
        logger.Entries.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateSettingsAsync_WhenInputNull_ShouldDoNothing()
    {
        await using var context = CreateContext();
        var logger = new TestLogger<SystemSettingService>();
        var service = new SystemSettingService(context, logger);

        await service.UpdateSettingsAsync(null!);

        context.SystemSettings.Should().BeEmpty();
        logger.Entries.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateSettingsAsync_ShouldOverwriteExistingValue()
    {
        await using var context = CreateContext();
        await context.SystemSettings.AddAsync(new SystemSetting("A", "1"));
        await context.SaveChangesAsync();

        var logger = new TestLogger<SystemSettingService>();
        var service = new SystemSettingService(context, logger);

        await service.UpdateSettingsAsync(new Dictionary<string, string> { ["A"] = "2" });

        var updated = await context.SystemSettings.FirstAsync(x => x.Key == "A");
        updated.Value.Should().Be("2");
    }

    [Theory]
    [InlineData("FREE_AI_QUOTA", "3")]
    [InlineData("AI_QUOTA_UNIT_PRICE", "10000")]
    [InlineData("UNKNOWN", null)]
    public async Task GetSettingAsync_ShouldReturnExpectedValueForDifferentKeys(string key, string? expectedValue)
    {
        await using var context = CreateContext();
        await context.SystemSettings.AddRangeAsync(
            new SystemSetting("FREE_AI_QUOTA", "3"),
            new SystemSetting("AI_QUOTA_UNIT_PRICE", "10000"));
        await context.SaveChangesAsync();

        var service = new SystemSettingService(context, new TestLogger<SystemSettingService>());

        var actual = await service.GetSettingAsync(key);

        actual.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("A", "1", "2")]
    [InlineData("B", "old", "new")]
    [InlineData("C", "x", "y")]
    public async Task UpdateSettingsAsync_ShouldUpdateSingleExistingKey(string key, string initial, string updated)
    {
        await using var context = CreateContext();
        await context.SystemSettings.AddAsync(new SystemSetting(key, initial));
        await context.SaveChangesAsync();

        var service = new SystemSettingService(context, new TestLogger<SystemSettingService>());

        await service.UpdateSettingsAsync(new Dictionary<string, string> { [key] = updated });

        var saved = await context.SystemSettings.FirstAsync(x => x.Key == key);
        saved.Value.Should().Be(updated);
    }

    [Fact]
    public async Task GetAllSettingsAsync_WhenNoData_ShouldReturnEmptyDictionary()
    {
        await using var context = CreateContext();
        var service = new SystemSettingService(context, new TestLogger<SystemSettingService>());

        var result = await service.GetAllSettingsAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateSettingsAsync_WithMixedExistingAndNewKeys_ShouldPersistAll()
    {
        await using var context = CreateContext();
        await context.SystemSettings.AddRangeAsync(
            new SystemSetting("A", "1"),
            new SystemSetting("B", "2"));
        await context.SaveChangesAsync();
        var service = new SystemSettingService(context, new TestLogger<SystemSettingService>());

        await service.UpdateSettingsAsync(new Dictionary<string, string>
        {
            ["A"] = "10",
            ["C"] = "30"
        });

        var all = await context.SystemSettings.OrderBy(x => x.Key).ToListAsync();
        all.Should().HaveCount(3);
        all.Should().ContainSingle(x => x.Key == "A" && x.Value == "10");
        all.Should().ContainSingle(x => x.Key == "B" && x.Value == "2");
        all.Should().ContainSingle(x => x.Key == "C" && x.Value == "30");
    }

    [Fact]
    public async Task UpdateSettingsAsync_WithSingleNewKey_ShouldInsertNewSetting()
    {
        await using var context = CreateContext();
        var service = new SystemSettingService(context, new TestLogger<SystemSettingService>());

        await service.UpdateSettingsAsync(new Dictionary<string, string> { ["NEW_KEY"] = "NEW_VALUE" });

        var saved = await context.SystemSettings.SingleAsync();
        saved.Key.Should().Be("NEW_KEY");
        saved.Value.Should().Be("NEW_VALUE");
    }

    [Theory]
    [InlineData("K1", "V1")]
    [InlineData("K2", "V2")]
    [InlineData("K3", "V3")]
    [InlineData("FEATURE_X", "true")]
    [InlineData("FEATURE_Y", "false")]
    [InlineData("TIMEOUT_MS", "5000")]
    [InlineData("RETRY_COUNT", "3")]
    [InlineData("SMTP_FROM", "noreply@test.local")]
    [InlineData("UI_THEME", "dark")]
    [InlineData("QUOTA_PRICE", "12000")]
    public async Task UpdateSettingsAsync_WithSinglePair_ShouldInsertExactKeyValue(string key, string value)
    {
        await using var context = CreateContext();
        var service = new SystemSettingService(context, new TestLogger<SystemSettingService>());

        await service.UpdateSettingsAsync(new Dictionary<string, string> { [key] = value });

        var saved = await context.SystemSettings.SingleAsync();
        saved.Key.Should().Be(key);
        saved.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("FREE_AI_QUOTA", "3")]
    [InlineData("AI_QUOTA_UNIT_PRICE", "10000")]
    [InlineData("SMTP_HOST", "smtp.test.local")]
    [InlineData("SMTP_PORT", "587")]
    [InlineData("EMAIL_FROM", "noreply@test.local")]
    [InlineData("JWT_ISSUER", "AuraEyes.Tests")]
    [InlineData("JWT_AUDIENCE", "AuraEyes.Client")]
    [InlineData("MAX_RETRY", "5")]
    [InlineData("FEATURE_ALPHA", "true")]
    [InlineData("CACHE_TTL", "600")]
    public async Task GetSettingAsync_WithManyKnownKeys_ShouldReturnExactValue(string key, string expected)
    {
        await using var context = CreateContext();
        await context.SystemSettings.AddRangeAsync(
            new SystemSetting("FREE_AI_QUOTA", "3"),
            new SystemSetting("AI_QUOTA_UNIT_PRICE", "10000"),
            new SystemSetting("SMTP_HOST", "smtp.test.local"),
            new SystemSetting("SMTP_PORT", "587"),
            new SystemSetting("EMAIL_FROM", "noreply@test.local"),
            new SystemSetting("JWT_ISSUER", "AuraEyes.Tests"),
            new SystemSetting("JWT_AUDIENCE", "AuraEyes.Client"),
            new SystemSetting("MAX_RETRY", "5"),
            new SystemSetting("FEATURE_ALPHA", "true"),
            new SystemSetting("CACHE_TTL", "600"));
        await context.SaveChangesAsync();
        var service = new SystemSettingService(context, new TestLogger<SystemSettingService>());

        var actual = await service.GetSettingAsync(key);

        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("MISSING_1")]
    [InlineData("MISSING_2")]
    [InlineData("MISSING_3")]
    [InlineData("MISSING_4")]
    [InlineData("MISSING_5")]
    [InlineData("MISSING_6")]
    [InlineData("MISSING_7")]
    [InlineData("MISSING_8")]
    [InlineData("MISSING_9")]
    [InlineData("MISSING_10")]
    public async Task GetSettingAsync_WithUnknownKeys_ShouldReturnNull(string key)
    {
        await using var context = CreateContext();
        await context.SystemSettings.AddAsync(new SystemSetting("EXISTING_KEY", "EXISTING_VALUE"));
        await context.SaveChangesAsync();
        var service = new SystemSettingService(context, new TestLogger<SystemSettingService>());

        var actual = await service.GetSettingAsync(key);

        actual.Should().BeNull();
    }

    [Theory]
    [InlineData("K_A", "V_A")]
    [InlineData("K_B", "V_B")]
    [InlineData("FEATURE_FLAG", "true")]
    [InlineData("MAX_ITEMS", "100")]
    [InlineData("SMTP_TIMEOUT", "30")]
    [InlineData("JWT_CLOCK_SKEW", "60")]
    [InlineData("UI_LOCALE", "vi-VN")]
    [InlineData("DEFAULT_ROLE", "Patient")]
    [InlineData("CACHE_POLICY", "Sliding")]
    [InlineData("ALERT_EMAIL", "alerts@test.local")]
    public async Task UpdateSettingsAsync_WithSinglePair_ShouldWriteLogContainingUpdatedKey(string key, string value)
    {
        await using var context = CreateContext();
        var logger = new TestLogger<SystemSettingService>();
        var service = new SystemSettingService(context, logger);

        await service.UpdateSettingsAsync(new Dictionary<string, string> { [key] = value });

        logger.Entries.Should().Contain(entry =>
            entry.Level == LogLevel.Information &&
            entry.Message.Contains("System settings updated for keys") &&
            entry.Message.Contains(key));
    }

    [Theory]
    [InlineData("A1", "B1", 2)]
    [InlineData("A2", "B2", 2)]
    [InlineData("A3", "B3", 2)]
    [InlineData("A4", "B4", 2)]
    [InlineData("A5", "B5", 2)]
    [InlineData("A6", "B6", 2)]
    [InlineData("A7", "B7", 2)]
    [InlineData("A8", "B8", 2)]
    [InlineData("A9", "B9", 2)]
    [InlineData("A10", "B10", 2)]
    public async Task UpdateSettingsAsync_WithTwoNewKeys_ShouldPersistBoth(string key1, string key2, int expectedCount)
    {
        await using var context = CreateContext();
        var service = new SystemSettingService(context, new TestLogger<SystemSettingService>());

        await service.UpdateSettingsAsync(new Dictionary<string, string>
        {
            [key1] = $"V_{key1}",
            [key2] = $"V_{key2}"
        });

        var all = await context.SystemSettings.ToListAsync();
        all.Should().HaveCount(expectedCount);
        all.Should().Contain(x => x.Key == key1 && x.Value == $"V_{key1}");
        all.Should().Contain(x => x.Key == key2 && x.Value == $"V_{key2}");
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
