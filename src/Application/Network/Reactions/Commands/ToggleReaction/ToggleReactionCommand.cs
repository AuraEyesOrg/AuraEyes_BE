using Application.Common.Interfaces;
using Domain.Enums.Network;

namespace Application.Network.Reactions.Commands.ToggleReaction;

/// <summary>
/// Command to toggle a reaction on a post (add/remove/change)
/// </summary>
public record ToggleReactionCommand : ICommand
{
    public Guid PostId { get; init; }
    public Guid UserId { get; init; }
    public ReactionType Type { get; init; }
}
