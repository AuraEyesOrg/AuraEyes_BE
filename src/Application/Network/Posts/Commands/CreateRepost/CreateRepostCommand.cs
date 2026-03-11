using Application.Common.Interfaces;
using Domain.Enums.Network;

namespace Application.Network.Posts.Commands.CreateRepost;

/// <summary>
/// Command to repost (share) an existing professional post.
/// Creates a new ProfessionalPost record with IsRepost = true.
/// </summary>
public class CreateRepostCommand : ICommand<Guid>
{
    /// <summary>The ID of the author who is reposting (injected from JWT token in handler).</summary>
    public Guid AuthorId { get; set; }

    /// <summary>The type of the author (Ophthalmologist or Organisation).</summary>
    public AuthorType AuthorType { get; set; }

    /// <summary>The ID of the original post to repost.</summary>
    public Guid OriginalPostId { get; set; }

    /// <summary>Optional comment added by the reposter (quote post text).</summary>
    public string? RepostComment { get; set; }
}
