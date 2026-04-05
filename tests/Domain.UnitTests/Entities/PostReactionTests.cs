using Domain.Entities.Network;
using Domain.Enums.Network;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class PostReactionTests
{
    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateReaction()
    {
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var reaction = new PostReaction(postId, userId, ReactionType.Insightful);

        reaction.PostId.Should().Be(postId);
        reaction.UserId.Should().Be(userId);
        reaction.Type.Should().Be(ReactionType.Insightful);
    }

    #endregion

    #region ChangeType

    [Fact]
    public void ChangeType_ShouldUpdateType()
    {
        var reaction = new PostReaction(Guid.NewGuid(), Guid.NewGuid(), ReactionType.Agree);

        reaction.ChangeType(ReactionType.Celebrate);

        reaction.Type.Should().Be(ReactionType.Celebrate);
        reaction.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void ChangeType_SameType_ShouldKeepTypeAndUpdateTimestamp()
    {
        var reaction = new PostReaction(Guid.NewGuid(), Guid.NewGuid(), ReactionType.Agree);
        var before = reaction.UpdatedAt;

        reaction.ChangeType(ReactionType.Agree);

        reaction.Type.Should().Be(ReactionType.Agree);
        reaction.UpdatedAt.Should().NotBeNull();
        if (before.HasValue)
            reaction.UpdatedAt.Should().BeOnOrAfter(before.Value);
    }

    [Theory]
    [InlineData(ReactionType.Insightful)]
    [InlineData(ReactionType.Helpful)]
    [InlineData(ReactionType.Question)]
    public void ChangeType_ToDifferentTypes_ShouldWork(ReactionType newType)
    {
        var reaction = new PostReaction(Guid.NewGuid(), Guid.NewGuid(), ReactionType.Agree);

        reaction.ChangeType(newType);

        reaction.Type.Should().Be(newType);
    }

    #endregion
}
