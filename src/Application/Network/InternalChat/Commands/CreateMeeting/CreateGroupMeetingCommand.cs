using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network.InternalChat;
using Microsoft.EntityFrameworkCore;

namespace Application.Network.InternalChat.Commands.CreateMeeting;

public class CreateGroupMeetingCommand : ICommand<MeetingInfo>
{
    public Guid GroupId { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class CreateGroupMeetingCommandHandler : ICommandHandler<CreateGroupMeetingCommand, MeetingInfo>
{
    private readonly IRepository<InternalGroupChat> _groupChatRepository;
    private readonly IGoogleMeetService _googleMeetService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly MediatR.IMediator _mediator;

    public CreateGroupMeetingCommandHandler(
        IRepository<InternalGroupChat> groupChatRepository,
        IGoogleMeetService googleMeetService,
        ICurrentUserService currentUserService,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        MediatR.IMediator mediator)
    {
        _groupChatRepository = groupChatRepository;
        _googleMeetService = googleMeetService;
        _currentUserService = currentUserService;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result<MeetingInfo>> Handle(CreateGroupMeetingCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        
        var group = await _groupChatRepository.Query()
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

        if (group == null)
        {
            return Result<MeetingInfo>.Failure("Group not found");
        }

        if (!group.Members.Any(m => m.MemberId == currentUserId))
        {
            return Result<MeetingInfo>.Forbidden("You are not a member of this group");
        }

        // Get emails of all members to invite them to the Google Meet
        var memberIds = group.Members.Select(m => m.MemberId).ToList();
        var attendeeEmails = new List<string>();
        
        foreach (var memberId in memberIds)
        {
            var user = await _identityService.GetUserByIdAsync(memberId);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                attendeeEmails.Add(user.Email);
            }
        }

        var meetingTitle = string.IsNullOrWhiteSpace(request.Title) 
            ? $"AURA Collaboration: {group.Name}" 
            : request.Title;

        var meetingInfo = await _googleMeetService.CreateMeetingAsync(
            meetingTitle,
            DateTime.UtcNow.AddMinutes(2), // Start in 2 minutes
            attendeeEmails,
            60,
            cancellationToken);

        // Persist meeting info to the group
        group.SetMeetingInfo(meetingInfo.MeetingLink, meetingInfo.CalendarEventId);
        await _groupChatRepository.UpdateAsync(group, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Automatically send a message to the group chat with the meeting link
        var messageContent = $"[SYSTEM] A new consultation meeting has been created: {meetingInfo.MeetingLink}";
        await _mediator.Send(new Application.Network.InternalChat.Commands.SendMessage.SendInternalGroupMessageCommand
        {
            GroupId = group.Id,
            Content = messageContent
        }, cancellationToken);

        return Result<MeetingInfo>.Success(meetingInfo);
    }
}
