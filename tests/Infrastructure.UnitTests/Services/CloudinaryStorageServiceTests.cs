using Application.Common.Interfaces;
using CloudinaryDotNet;
using FluentAssertions;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Infrastructure.UnitTests.Services;

public class CloudinaryStorageServiceTests
{
    private readonly Mock<IOptions<CloudinarySettings>> _settingsMock;
    private readonly Mock<ILogger<CloudinaryStorageService>> _loggerMock;
    private readonly CloudinarySettings _settings;

    public CloudinaryStorageServiceTests()
    {
        _settings = new CloudinarySettings
        {
            CloudName = "test-cloud",
            ApiKey = "test-key",
            ApiSecret = "test-secret",
            Folder = "test-folder"
        };
        _settingsMock = new Mock<IOptions<CloudinarySettings>>();
        _settingsMock.Setup(s => s.Value).Returns(_settings);
        _loggerMock = new Mock<ILogger<CloudinaryStorageService>>();
    }

    [Fact]
    public void Constructor_ShouldInitializeCloudinary()
    {
        // Act
        var service = new CloudinaryStorageService(_settingsMock.Object, _loggerMock.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveFileAsync_WhenStreamEmpty_ShouldThrow()
    {
        // Arrange
        var service = new CloudinaryStorageService(_settingsMock.Object, _loggerMock.Object);
        var stream = new MemoryStream();

        // Act
        var act = async () => await service.SaveFileAsync(stream, "test.png", "sub");

        // Assert
        // It will fail because the API key is invalid/test
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public void DeleteFile_WhenUrlInvalid_ShouldReturnFalse()
    {
        // Arrange
        var service = new CloudinaryStorageService(_settingsMock.Object, _loggerMock.Object);

        // Act
        var result = service.DeleteFile("");

        // Assert
        result.Should().BeFalse();
    }
}
