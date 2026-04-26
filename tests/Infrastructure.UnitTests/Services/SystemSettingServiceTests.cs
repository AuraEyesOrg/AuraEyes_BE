using Domain.Entities.Platform;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Infrastructure.UnitTests.Services;

public class SystemSettingServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<SystemSettingService>> _loggerMock;
    private readonly SystemSettingService _service;

    public SystemSettingServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<SystemSettingService>>();
        _service = new SystemSettingService(_context, _loggerMock.Object);
    }

    [Fact]
    public async Task GetSettingAsync_WhenFound_ShouldReturnValue()
    {
        // Arrange
        var key = "TEST_KEY";
        var value = "TEST_VALUE";
        await _context.SystemSettings.AddAsync(new SystemSetting(key, value));
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetSettingAsync(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task GetAllSettingsAsync_ShouldReturnAllAsDictionary()
    {
        // Arrange
        await _context.SystemSettings.AddRangeAsync(
            new SystemSetting("K1", "V1"),
            new SystemSetting("K2", "V2")
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllSettingsAsync();

        // Assert
        result.Should().HaveCount(2);
        result["K1"].Should().Be("V1");
        result["K2"].Should().Be("V2");
    }

    [Fact]
    public async Task UpdateSettingsAsync_ShouldUpsertValues()
    {
        // Arrange
        await _context.SystemSettings.AddAsync(new SystemSetting("EXISTING", "OLD"));
        await _context.SaveChangesAsync();

        var updates = new Dictionary<string, string>
        {
            { "EXISTING", "NEW" },
            { "NEW_KEY", "NEW_VALUE" }
        };

        // Act
        await _service.UpdateSettingsAsync(updates);

        // Assert
        var existing = await _service.GetSettingAsync("EXISTING");
        var added = await _service.GetSettingAsync("NEW_KEY");

        existing.Should().Be("NEW");
        added.Should().Be("NEW_VALUE");
    }
}
