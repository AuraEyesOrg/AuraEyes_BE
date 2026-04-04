using Domain.Entities.Network;
using Domain.Enums.Network;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class PostCommentTests
{
    private readonly Guid _postId = Guid.NewGuid();
    private readonly Guid _authorId = Guid.NewGuid();

    private PostComment CreateValidComment(string content = "Test comment", Guid? parentCommentId = null)
    {
        return new PostComment(_postId, _authorId, AuthorType.Ophthalmologist, content, parentCommentId);
    }

    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateComment()
    {
        var parentId = Guid.NewGuid();

        var comment = new PostComment(_postId, _authorId, AuthorType.Organisation, "Great post!", parentId);

        comment.PostId.Should().Be(_postId);
        comment.AuthorId.Should().Be(_authorId);
        comment.AuthorType.Should().Be(AuthorType.Organisation);
        comment.Content.Should().Be("Great post!");
        comment.ParentCommentId.Should().Be(parentId);
        comment.ReplyCount.Should().Be(0);
        comment.LikeCount.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithoutParent_ShouldHaveNullParentId()
    {
        var comment = new PostComment(_postId, _authorId, AuthorType.Ophthalmologist, "Top-level comment");

        comment.ParentCommentId.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyContent_ShouldThrow(string? content)
    {
        var act = () => new PostComment(_postId, _authorId, AuthorType.Ophthalmologist, content!);

        act.Should().Throw<ArgumentException>().WithParameterName("content");
    }

    #endregion

    #region UpdateContent

    [Fact]
    public void UpdateContent_ValidContent_ShouldUpdate()
    {
        var comment = CreateValidComment();

        comment.UpdateContent("Updated comment");

        comment.Content.Should().Be("Updated comment");
        comment.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateContent_EmptyContent_ShouldThrow(string? content)
    {
        var comment = CreateValidComment();

        var act = () => comment.UpdateContent(content!);

        act.Should().Throw<ArgumentException>().WithParameterName("content");
    }

    #endregion

    #region Reply Count

    [Fact]
    public void IncrementReplyCount_ShouldIncrease()
    {
        var comment = CreateValidComment();

        comment.IncrementReplyCount();
        comment.IncrementReplyCount();

        comment.ReplyCount.Should().Be(2);
    }

    [Fact]
    public void DecrementReplyCount_ShouldDecrease()
    {
        var comment = CreateValidComment();
        comment.IncrementReplyCount();
        comment.IncrementReplyCount();

        comment.DecrementReplyCount();

        comment.ReplyCount.Should().Be(1);
    }

    [Fact]
    public void DecrementReplyCount_WhenZero_ShouldRemainZero()
    {
        var comment = CreateValidComment();

        comment.DecrementReplyCount();

        comment.ReplyCount.Should().Be(0);
    }

    #endregion

    #region Like Count

    [Fact]
    public void IncrementLikeCount_ShouldIncrease()
    {
        var comment = CreateValidComment();

        comment.IncrementLikeCount();
        comment.IncrementLikeCount();

        comment.LikeCount.Should().Be(2);
    }

    [Fact]
    public void DecrementLikeCount_ShouldDecrease()
    {
        var comment = CreateValidComment();
        comment.IncrementLikeCount();
        comment.IncrementLikeCount();

        comment.DecrementLikeCount();

        comment.LikeCount.Should().Be(1);
    }

    [Fact]
    public void DecrementLikeCount_WhenZero_ShouldRemainZero()
    {
        var comment = CreateValidComment();

        comment.DecrementLikeCount();

        comment.LikeCount.Should().Be(0);
    }

    #endregion
}
