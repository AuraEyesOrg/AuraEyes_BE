using FluentAssertions;
using Infrastructure.Services;
using Infrastructure.Settings;
using Infrastructure.UnitTests.Common;
using Microsoft.Extensions.Options;

namespace Infrastructure.UnitTests.Services;

public class SupabaseStorageServiceTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\r\n")]
    [InlineData("\n")]
    [InlineData("\r")]
    [InlineData(" \t ")]
    [InlineData("    \r\n")]
    [InlineData("   \n")]
    public void DeleteFile_WithBlankPath_ShouldReturnFalse(string path)
    {
        var service = CreateService();

        var result = service.DeleteFile(path);

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\r\n")]
    [InlineData("\n")]
    [InlineData("\r")]
    [InlineData(" \t ")]
    [InlineData("    \r\n")]
    [InlineData("   \n")]
    public void FileExists_WithBlankPath_ShouldReturnFalse(string path)
    {
        var service = CreateService();

        var result = service.FileExists(path);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task SaveFileAsync_WithEmptyStream_ShouldThrowInvalidOperationException()
    {
        var service = CreateService();
        await using var stream = new MemoryStream(Array.Empty<byte>());

        var act = async () => await service.SaveFileAsync(stream, "test.png", "folder");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*File stream is empty*");
    }

    private static SupabaseStorageService CreateService()
    {
        var settings = new SupabaseStorageSettings
        {
            Url = "https://example.supabase.co",
            ServiceKey = "service-key",
            BucketName = "files"
        };
        return new SupabaseStorageService(Options.Create(settings), new TestLogger<SupabaseStorageService>());
    }
}
