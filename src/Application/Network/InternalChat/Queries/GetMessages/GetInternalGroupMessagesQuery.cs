using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network.InternalChat;
using Domain.Enums.Network;
using Microsoft.EntityFrameworkCore;

namespace Application.Network.InternalChat.Queries.GetMessages;

public class InternalGroupMessageDto
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid SenderId { get; set; }
    public AuthorType SenderType { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? SenderName { get; set; }
    public string? SenderAvatar { get; set; }
}

public class GetInternalGroupMessagesQuery : IQuery<List<InternalGroupMessageDto>>
{
    public Guid GroupId { get; set; }
}

public class GetInternalGroupMessagesQueryHandler : IQueryHandler<GetInternalGroupMessagesQuery, List<InternalGroupMessageDto>>
{
    private readonly IRepository<InternalGroupChat> _groupChatRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly IRepository<InternalGroupMessage> _messageRepository;

    public GetInternalGroupMessagesQueryHandler(
        IRepository<InternalGroupMessage> messageRepository,
        IRepository<InternalGroupChat> groupChatRepository,
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _messageRepository = messageRepository;
        _groupChatRepository = groupChatRepository;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<Result<List<InternalGroupMessageDto>>> Handle(GetInternalGroupMessagesQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();

        // Verify membership
        var isMember = await _groupChatRepository.Query()
            .AsNoTracking()
            .AnyAsync(g => g.Id == request.GroupId && g.Members.Any(m => m.MemberId == currentUserId), cancellationToken);

        if (!isMember)
        {
            return Result<List<InternalGroupMessageDto>>.Forbidden("You are not a member of this group");
        }

        var messages = await _messageRepository.Query()
            .AsNoTracking()
            .Where(m => m.GroupId == request.GroupId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new InternalGroupMessageDto
            {
                Id = m.Id,
                GroupId = m.GroupId,
                SenderId = m.SenderId,
                SenderType = m.SenderType,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var senderIds = messages.Select(m => m.SenderId).Distinct();
        var users = await _identityService.GetUsersByIdsAsync(senderIds, cancellationToken);
        var userDict = users.ToDictionary(u => u.Id);

        foreach (var message in messages)
        {
            if (userDict.TryGetValue(message.SenderId, out var user))
            {
                message.SenderName = user.FullName;
                message.SenderAvatar = user.AvatarUrl;
            }
        }

        return Result<List<InternalGroupMessageDto>>.Success(messages);
    }
}
