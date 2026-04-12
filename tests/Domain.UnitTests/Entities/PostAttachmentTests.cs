using Domain.Entities.Network;
using Domain.Enums.Network;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class PostAttachmentTests
{
    private readonly Guid _postId = Guid.NewGuid();

    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateAttachment()
    {
        var attachment = new PostAttachment(
            _postId,
            AttachmentType.Image,
            "retina.png",
            "https://cdn/retina.png",
            "image/png",
            204800,
            1);

        attachment.PostId.Should().Be(_postId);
        attachment.Type.Should().Be(AttachmentType.Image);
        attachment.FileName.Should().Be("retina.png");
        attachment.FileUrl.Should().Be("https://cdn/retina.png");
        attachment.MimeType.Should().Be("image/png");
        attachment.FileSize.Should().Be(204800);
        attachment.DisplayOrder.Should().Be(1);
    }

    [Fact]
    public void Constructor_MinimalInput_ShouldUseDefaults()
    {
        var attachment = new PostAttachment(_postId, AttachmentType.Document, "doc.pdf", "https://cdn/doc.pdf");

        attachment.MimeType.Should().BeNull();
        attachment.FileSize.Should().BeNull();
        attachment.DisplayOrder.Should().Be(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyFileName_ShouldThrow(string? fileName)
    {
        var act = () => new PostAttachment(_postId, AttachmentType.Image, fileName!, "https://cdn/img.png");

        act.Should().Throw<ArgumentException>().WithParameterName("fileName");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyFileUrl_ShouldThrow(string? fileUrl)
    {
        var act = () => new PostAttachment(_postId, AttachmentType.Image, "img.png", fileUrl!);

        act.Should().Throw<ArgumentException>().WithParameterName("fileUrl");
    }

    #endregion

    #region UpdateDisplayOrder

    [Fact]
    public void UpdateDisplayOrder_ShouldUpdateOrder()
    {
        var attachment = new PostAttachment(_postId, AttachmentType.Research, "paper.pdf", "https://cdn/paper.pdf");

        attachment.UpdateDisplayOrder(5);

        attachment.DisplayOrder.Should().Be(5);
        attachment.UpdatedAt.Should().NotBeNull();
    }

    #endregion
}
