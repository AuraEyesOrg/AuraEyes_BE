using Domain.Entities.Network;
using Domain.Enums.Network;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class ProfessionalPostTests
{
    private readonly Guid _authorId = Guid.NewGuid();

    private ProfessionalPost CreateValidPost(
        string content = "Test post content",
        PostCategory category = PostCategory.CasePresentation,
        Guid? organisationId = null,
        bool allowComments = true)
    {
        return new ProfessionalPost(_authorId, AuthorType.Ophthalmologist, content, category, organisationId, allowComments);
    }

    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreatePost()
    {
        var orgId = Guid.NewGuid();

        var post = new ProfessionalPost(
            _authorId,
            AuthorType.Organisation,
            "Valid content",
            PostCategory.Announcement,
            orgId,
            false);

        post.AuthorId.Should().Be(_authorId);
        post.AuthorType.Should().Be(AuthorType.Organisation);
        post.Content.Should().Be("Valid content");
        post.Category.Should().Be(PostCategory.Announcement);
        post.OrganisationId.Should().Be(orgId);
        post.AllowComments.Should().BeFalse();
        post.IsRepost.Should().BeFalse();
        post.ReactionCount.Should().Be(0);
        post.CommentCount.Should().Be(0);
        post.RepostCount.Should().Be(0);
        post.ViewCount.Should().Be(0);
        post.IsHidden.Should().BeFalse();
        post.IsInternalCase.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyContent_ShouldThrow(string? content)
    {
        var act = () => new ProfessionalPost(_authorId, AuthorType.Ophthalmologist, content!, PostCategory.CasePresentation);

        act.Should().Throw<ArgumentException>().WithParameterName("content");
    }

    #endregion

    #region CreateRepost

    [Fact]
    public void CreateRepost_ShouldCopyOriginalPostData()
    {
        var original = CreateValidPost("Original content", PostCategory.KnowledgeShare);
        var repostAuthorId = Guid.NewGuid();

        var repost = ProfessionalPost.CreateRepost(repostAuthorId, AuthorType.Organisation, original, "My comment");

        repost.AuthorId.Should().Be(repostAuthorId);
        repost.AuthorType.Should().Be(AuthorType.Organisation);
        repost.Content.Should().Be("Original content");
        repost.Category.Should().Be(PostCategory.KnowledgeShare);
        repost.OriginalPostId.Should().Be(original.Id);
        repost.IsRepost.Should().BeTrue();
        repost.RepostComment.Should().Be("My comment");
        repost.AllowComments.Should().BeTrue();
    }

    [Fact]
    public void CreateRepost_WithoutComment_ShouldHaveNullComment()
    {
        var original = CreateValidPost();

        var repost = ProfessionalPost.CreateRepost(Guid.NewGuid(), AuthorType.Ophthalmologist, original);

        repost.RepostComment.Should().BeNull();
    }

    #endregion

    #region UpdateContent

    [Fact]
    public void UpdateContent_ValidContent_ShouldUpdate()
    {
        var post = CreateValidPost();

        post.UpdateContent("Updated content");

        post.Content.Should().Be("Updated content");
        post.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateContent_EmptyContent_ShouldThrow(string? content)
    {
        var post = CreateValidPost();

        var act = () => post.UpdateContent(content!);

        act.Should().Throw<ArgumentException>().WithParameterName("content");
    }

    #endregion

    #region UpdateAllowComments

    [Fact]
    public void UpdateAllowComments_ShouldToggle()
    {
        var post = CreateValidPost(allowComments: true);

        post.UpdateAllowComments(false);

        post.AllowComments.Should().BeFalse();
        post.UpdatedAt.Should().NotBeNull();
    }

    #endregion

    #region Reaction Count

    [Fact]
    public void IncrementReactionCount_ShouldIncrease()
    {
        var post = CreateValidPost();

        post.IncrementReactionCount();
        post.IncrementReactionCount();

        post.ReactionCount.Should().Be(2);
    }

    [Fact]
    public void DecrementReactionCount_ShouldDecrease()
    {
        var post = CreateValidPost();
        post.IncrementReactionCount();
        post.IncrementReactionCount();

        post.DecrementReactionCount();

        post.ReactionCount.Should().Be(1);
    }

    [Fact]
    public void DecrementReactionCount_WhenZero_ShouldRemainZero()
    {
        var post = CreateValidPost();

        post.DecrementReactionCount();

        post.ReactionCount.Should().Be(0);
    }

    #endregion

    #region Comment Count

    [Fact]
    public void IncrementCommentCount_ShouldIncrease()
    {
        var post = CreateValidPost();

        post.IncrementCommentCount();

        post.CommentCount.Should().Be(1);
    }

    [Fact]
    public void DecrementCommentCount_ShouldDecrease()
    {
        var post = CreateValidPost();
        post.IncrementCommentCount();
        post.IncrementCommentCount();

        post.DecrementCommentCount();

        post.CommentCount.Should().Be(1);
    }

    [Fact]
    public void DecrementCommentCount_WhenZero_ShouldRemainZero()
    {
        var post = CreateValidPost();

        post.DecrementCommentCount();

        post.CommentCount.Should().Be(0);
    }

    #endregion

    #region Repost Count

    [Fact]
    public void IncrementRepostCount_ShouldIncrease()
    {
        var post = CreateValidPost();

        post.IncrementRepostCount();
        post.IncrementRepostCount();

        post.RepostCount.Should().Be(2);
    }

    #endregion

    #region View Count

    [Fact]
    public void IncrementViewCount_ShouldIncrease()
    {
        var post = CreateValidPost();

        post.IncrementViewCount();
        post.IncrementViewCount();
        post.IncrementViewCount();

        post.ViewCount.Should().Be(3);
    }

    #endregion

    #region AddAttachment

    [Fact]
    public void AddAttachment_ShouldAddToCollection()
    {
        var post = CreateValidPost();
        var attachment = new PostAttachment(post.Id, AttachmentType.Image, "img.png", "https://cdn/img.png");

        post.AddAttachment(attachment);

        post.Attachments.Should().ContainSingle().Which.Should().Be(attachment);
        post.UpdatedAt.Should().NotBeNull();
    }

    #endregion

    #region Hide

    [Fact]
    public void Hide_WithReason_ShouldSetFields()
    {
        var post = CreateValidPost();

        post.Hide("Violates policy");

        post.IsHidden.Should().BeTrue();
        post.HideReason.Should().Be("Violates policy");
        post.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Hide_WithNullReason_ShouldSetHideReasonNull()
    {
        var post = CreateValidPost();

        post.Hide(null);

        post.IsHidden.Should().BeTrue();
        post.HideReason.Should().BeNull();
    }

    [Fact]
    public void Hide_WithWhitespaceReason_ShouldSetHideReasonNull()
    {
        var post = CreateValidPost();

        post.Hide("   ");

        post.IsHidden.Should().BeTrue();
        post.HideReason.Should().BeNull();
    }

    #endregion

    #region SetClinicalCaseMetadata

    [Fact]
    public void SetClinicalCaseMetadata_ValidInput_ShouldSetFields()
    {
        var post = CreateValidPost();
        var sessionId = Guid.NewGuid();

        post.SetClinicalCaseMetadata(true, sessionId, 55, "Male");

        post.IsInternalCase.Should().BeTrue();
        post.ConsultationSessionId.Should().Be(sessionId);
        post.PatientAge.Should().Be(55);
        post.PatientGender.Should().Be("Male");
        post.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void SetClinicalCaseMetadata_NegativeAge_ShouldThrow()
    {
        var post = CreateValidPost();

        var act = () => post.SetClinicalCaseMetadata(false, null, -1, null);

        act.Should().Throw<ArgumentException>().WithParameterName("patientAge");
    }

    [Fact]
    public void SetClinicalCaseMetadata_InternalCaseWithoutSessionId_ShouldThrow()
    {
        var post = CreateValidPost();

        var act = () => post.SetClinicalCaseMetadata(true, null, 30, "Female");

        act.Should().Throw<ArgumentException>().WithParameterName("consultationSessionId");
    }

    [Fact]
    public void SetClinicalCaseMetadata_NotInternalCase_SessionIdOptional()
    {
        var post = CreateValidPost();

        post.SetClinicalCaseMetadata(false, null, 40, "Female");

        post.IsInternalCase.Should().BeFalse();
        post.ConsultationSessionId.Should().BeNull();
    }

    #endregion
}
