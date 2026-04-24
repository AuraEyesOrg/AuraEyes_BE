using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network.InternalChat;
using Microsoft.EntityFrameworkCore;

namespace Application.Network.InternalChat.Queries.GetGroups;

public class InternalGroupChatDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public InternalGroupType Type { get; set; }
    public Guid? ConsultationSessionId { get; set; }
    public string? MeetingLink { get; set; }
    public string? CalendarEventId { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Guid> MemberIds { get; set; } = new();
}

public class GetInternalGroupChatsQuery : IQuery<List<InternalGroupChatDto>>
{
}

public class GetInternalGroupChatsQueryHandler : IQueryHandler<GetInternalGroupChatsQuery, List<InternalGroupChatDto>>
{
    private readonly IRepository<InternalGroupChat> _groupChatRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetInternalGroupChatsQueryHandler(IRepository<InternalGroupChat> groupChatRepository, ICurrentUserService currentUserService)
    {
        _groupChatRepository = groupChatRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<InternalGroupChatDto>>> Handle(GetInternalGroupChatsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var groups = await _groupChatRepository.Query()
            .AsNoTracking()
            .Where(g => g.Members.Any(m => m.MemberId == currentUserId))
            .Select(g => new InternalGroupChatDto
            {
                Id = g.Id,
                Name = g.Name,
                Type = g.Type,
                ConsultationSessionId = g.ConsultationSessionId,
                MeetingLink = g.MeetingLink,
                CalendarEventId = g.CalendarEventId,
                CreatorId = g.CreatorId,
                CreatedAt = g.CreatedAt,
                MemberIds = g.Members.Select(m => m.MemberId).ToList()
            })
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result<List<InternalGroupChatDto>>.Success(groups);
    }
}

