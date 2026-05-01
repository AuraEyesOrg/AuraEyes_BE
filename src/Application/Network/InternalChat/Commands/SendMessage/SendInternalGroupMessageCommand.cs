using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network.InternalChat;
using Domain.Enums.Network;
using Microsoft.EntityFrameworkCore;

namespace Application.Network.InternalChat.Commands.SendMessage;

public class SendInternalGroupMessageCommand : ICommand<Guid>
{
    public Guid GroupId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class SendInternalGroupMessageCommandHandler : ICommandHandler<SendInternalGroupMessageCommand, Guid>
{
    private readonly IRepository<InternalGroupChat> _groupChatRepository;
    private readonly IRepository<InternalGroupMessage> _messageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IInternalChatHubService _chatHubService;
    private readonly IIdentityService _identityService;

    public SendInternalGroupMessageCommandHandler(
        IRepository<InternalGroupChat> groupChatRepository,
        IRepository<InternalGroupMessage> messageRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IInternalChatHubService chatHubService,
        IIdentityService identityService)
    {
        _groupChatRepository = groupChatRepository;
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _chatHubService = chatHubService;
        _identityService = identityService;
    }

    public async Task<Result<Guid>> Handle(SendInternalGroupMessageCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var currentUserRole = _currentUserService.Roles.FirstOrDefault();

        var senderType = currentUserRole switch
        {
            "Ophthalmologist" => AuthorType.Ophthalmologist,
            "ClinicStaff" => AuthorType.ClinicStaff,
            "SystemAdmin" => AuthorType.SystemAdmin,
            _ => throw new UnauthorizedAccessException("Invalid role for sending internal chat")
        };

        var group = await _groupChatRepository.Query()
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

        if (group == null)
        {
            return Result<Guid>.Failure("Group not found");
        }

        if (!group.Members.Any(m => m.MemberId == currentUserId))
        {
            return Result<Guid>.Failure("You are not a member of this group");
        }

        try
        {
            var message = new InternalGroupMessage(group.Id, currentUserId, senderType, request.Content);
            group.AddMessage(message);

            await _messageRepository.AddAsync(message, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Broadcast realtime message
            var sender = await _identityService.GetUserByIdAsync(currentUserId, cancellationToken);

            var messageDto = new Application.Network.InternalChat.Queries.GetMessages.InternalGroupMessageDto
            {
                Id = message.Id,
                GroupId = message.GroupId,
                SenderId = message.SenderId,
                SenderType = message.SenderType,
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                SenderName = sender?.FullName,
                SenderAvatar = sender?.AvatarUrl
            };

            await _chatHubService.BroadcastMessageAsync(group.Id, messageDto, cancellationToken);

            return Result<Guid>.Success(message.Id);
        }
        catch (InvalidOperationException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
