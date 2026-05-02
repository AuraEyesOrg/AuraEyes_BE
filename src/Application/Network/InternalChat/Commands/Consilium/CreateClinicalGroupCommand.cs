using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network.InternalChat;
using Domain.Enums;
using Domain.Enums.Network;
using Microsoft.Extensions.Logging;

namespace Application.Network.InternalChat.Commands.Consilium;

public class CreateClinicalGroupCommand : ICommand<Guid>
{
    public string? Name { get; set; }
    public Guid? ConsultationSessionId { get; set; }
    public List<Guid> InvitedDoctorIds { get; set; } = new();
    public string? Reason { get; set; }
}

public class CreateClinicalGroupCommandHandler : ICommandHandler<CreateClinicalGroupCommand, Guid>
{
    private readonly IRepository<InternalGroupChat> _groupChatRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInternalChatHubService _chatHubService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CreateClinicalGroupCommandHandler> _logger;

    public CreateClinicalGroupCommandHandler(
        IRepository<InternalGroupChat> groupChatRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IInternalChatHubService chatHubService,
        INotificationService notificationService,
        ILogger<CreateClinicalGroupCommandHandler> logger)
    {
        _groupChatRepository = groupChatRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _chatHubService = chatHubService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateClinicalGroupCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        
        // Ensure name is set
        var groupName = string.IsNullOrWhiteSpace(request.Name) 
            ? $"Hội chẩn: {DateTime.Now:dd/MM HH:mm}" 
            : request.Name;

        var group = new InternalGroupChat(
            groupName, 
            InternalGroupType.ClinicalCase, 
            currentUserId, 
            AuthorType.Ophthalmologist, 
            request.ConsultationSessionId);
        
        // Add creator
        group.AddMember(currentUserId, AuthorType.Ophthalmologist);
        
        // Add invited doctors
        foreach (var doctorId in request.InvitedDoctorIds)
        {
            if (doctorId != currentUserId)
            {
                group.AddMember(doctorId, AuthorType.Ophthalmologist);
            }
        }

        // Add initial system message if reason is provided
        if (!string.IsNullOrWhiteSpace(request.Reason))
        {
            var systemMessage = new InternalGroupMessage(
                group.Id,
                currentUserId,
                AuthorType.Ophthalmologist,
                $"[HỘI CHẨN LÂM SÀNG]\nLý do: {request.Reason}\nXem chi tiết tại Hồ sơ bệnh án của phiên khám."
            );
            group.AddMessage(systemMessage);
        }

        await _groupChatRepository.AddAsync(group, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify invited doctors via SignalR
        foreach (var doctorId in request.InvitedDoctorIds)
        {
            if (doctorId == currentUserId) continue;

            await _notificationService.SendAsync(
                doctorId,
                "Mời hội chẩn lâm sàng khẩn cấp",
                $"Bác sĩ {_currentUserService.UserName} mời bạn hội chẩn ca bệnh: {groupName}",
                NotificationType.ConsiliumInvitation,
                new { 
                    GroupId = group.Id, 
                    SessionId = request.ConsultationSessionId,
                    InviterName = _currentUserService.UserName,
                    GroupName = groupName
                },
                cancellationToken,
                group.Id);
        }

        await _chatHubService.BroadcastGroupUpdateAsync(group.Id, "GroupCreated", cancellationToken);

        return Result<Guid>.Success(group.Id);
    }
}
