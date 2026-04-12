using Domain.Entities.Network;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class SavedPostTests
{
    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateSavedPost()
    {
        var userId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        var saved = new SavedPost(userId, postId, "Favorites");

        saved.UserId.Should().Be(userId);
        saved.PostId.Should().Be(postId);
        saved.CollectionName.Should().Be("Favorites");
    }

    [Fact]
    public void Constructor_WithoutCollection_ShouldHaveNullCollection()
    {
        var saved = new SavedPost(Guid.NewGuid(), Guid.NewGuid());

        saved.CollectionName.Should().BeNull();
    }

    #endregion

    #region MoveToCollection

    [Fact]
    public void MoveToCollection_ShouldUpdateCollectionName()
    {
        var saved = new SavedPost(Guid.NewGuid(), Guid.NewGuid(), "Old Collection");

        saved.MoveToCollection("New Collection");

        saved.CollectionName.Should().Be("New Collection");
        saved.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MoveToCollection_Null_ShouldRemoveFromCollection()
    {
        var saved = new SavedPost(Guid.NewGuid(), Guid.NewGuid(), "Favorites");

        saved.MoveToCollection(null);

        saved.CollectionName.Should().BeNull();
        saved.UpdatedAt.Should().NotBeNull();
    }

    #endregion
}
