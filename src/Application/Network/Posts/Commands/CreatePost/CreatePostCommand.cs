using Application.Common.Interfaces;
using Domain.Enums.Network;
using Microsoft.AspNetCore.Http;

namespace Application.Network.Posts.Commands.CreatePost;

/// <summary>
/// Command to create a new professional post
/// </summary>
public class CreatePostCommand : ICommand<Guid>
{
    public Guid AuthorId { get; set; }
    public AuthorType AuthorType { get; set; }
    public string Content { get; set; } = string.Empty;
    public PostCategory Category { get; set; }
    public Guid? OrganisationId { get; set; }
    public bool AllowComments { get; set; } = true;
    public List<IFormFile>? Attachments { get; set; }
    public bool IsAnonymizationConfirmed { get; set; }
    public bool IsInternalCase { get; set; }
    public Guid? ConsultationSessionId { get; set; }
    public int? PatientAge { get; set; }
    public string? PatientGender { get; set; }
}
