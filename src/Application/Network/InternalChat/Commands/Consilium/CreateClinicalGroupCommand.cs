using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Network.InternalChat;
using Domain.Enums;
using Domain.Enums.Network;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Network.InternalChat.Commands.Consilium;

public class CreateClinicalGroupCommand : ICommand<Guid>
{
    public string? Name { get; set; }
    public Guid? ConsultationSessionId { get; set; }
    public List<Guid> InvitedDoctorIds { get; set; } = new();
    public string? Reason { get; set; }

    /// <summary>
    /// Marks this consilium as a high-priority emergency case.
    /// Affects the SignalR notification title and the system first-message prefix.
    /// </summary>
    public bool IsEmergency { get; set; } = false;

    /// <summary>
    /// ID of the Medical Record associated with this consilium.
    /// Injected as a deep-link in the first system message so invited doctors
    /// can navigate directly to the patient's record without extra clicks.
    /// </summary>
    public Guid? MedicalRecordId { get; set; }
}

public class CreateClinicalGroupCommandHandler : ICommandHandler<CreateClinicalGroupCommand, Guid>
{
    private readonly IRepository<InternalGroupChat> _groupChatRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInternalChatHubService _chatHubService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IRepository<ConsultationSession> _sessionRepository;
    private readonly ILogger<CreateClinicalGroupCommandHandler> _logger;

    public CreateClinicalGroupCommandHandler(
        IRepository<InternalGroupChat> groupChatRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IInternalChatHubService chatHubService,
        INotificationService notificationService,
        IOphthalmologistRepository ophthalmologistRepository,
        IRepository<ConsultationSession> sessionRepository,
        ILogger<CreateClinicalGroupCommandHandler> logger)
    {
        _groupChatRepository = groupChatRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _chatHubService = chatHubService;
        _notificationService = notificationService;
        _ophthalmologistRepository = ophthalmologistRepository;
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateClinicalGroupCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        
        // Ensure name is set
        var groupName = string.IsNullOrWhiteSpace(request.Name) 
            ? $"Hội chẩn: {DateTime.Now:dd/MM HH:mm}" 
            : request.Name;

        // ─── CRITICAL FIX: InvitedDoctorIds are Ophthalmologist entity IDs.
        // MemberId in GroupMembers must be ApplicationUser.Id (UserId).
        // Resolve ophthalmologist ID → userId mapping in one batch query.
        var invitedOphthalmologistIds = request.InvitedDoctorIds.Distinct().ToList();
        
        var ophthalmologists = await _ophthalmologistRepository.Query()
            .Where(o => invitedOphthalmologistIds.Contains(o.Id))
            .Select(o => new { o.Id, o.UserId })
            .ToListAsync(cancellationToken);
        
        // Map: ophthalmologistId -> userId
        var ophthalToUserIdMap = ophthalmologists.ToDictionary(o => o.Id, o => o.UserId);

        var group = new InternalGroupChat(
            groupName, 
            InternalGroupType.ClinicalCase, 
            currentUserId, 
            AuthorType.Ophthalmologist, 
            request.ConsultationSessionId);
        
        // Add creator (currentUserId is the ApplicationUser.Id — already correct)
        group.AddMember(currentUserId, AuthorType.Ophthalmologist);
        
        // Add invited doctors using their UserId (not ophthalmologistId)
        foreach (var ophthalmologistId in invitedOphthalmologistIds)
        {
            if (!ophthalToUserIdMap.TryGetValue(ophthalmologistId, out var invitedUserId))
            {
                _logger.LogWarning("Could not resolve UserId for OphthalmologistId {OphthalmologistId}. Skipping.", ophthalmologistId);
                continue;
            }
            
            if (invitedUserId != currentUserId)
            {
                group.AddMember(invitedUserId, AuthorType.Ophthalmologist);
            }
        }

        // Always inject a first system message with:
        //   - Emergency tag prefix when IsEmergency = true
        //   - Consilium reason (if provided)
        //   - Deep-link to the Medical Record (enables one-click navigation for invited doctors)
        var emergencyPrefix = request.IsEmergency ? "🚨 [CA KHẨN CẤP] " : "";

        var reasonSection = !string.IsNullOrWhiteSpace(request.Reason)
            ? $"\nLý do: {request.Reason}"
            : string.Empty;

        var medicalRecordSection = request.MedicalRecordId.HasValue
            ? $"\n🔗 Xem Bệnh án: /medical-records/{request.MedicalRecordId}"
            : string.Empty;

        var screeningId = Guid.Empty;
        if (request.ConsultationSessionId.HasValue)
        {
            var session = await _sessionRepository.GetByIdAsync(request.ConsultationSessionId.Value, cancellationToken);
            if (session != null && session.AiScreeningId.HasValue)
            {
                screeningId = session.AiScreeningId.Value;
            }
        }

        var screeningSection = screeningId != Guid.Empty
            ? $"\n🔬 Xem Review Hội chẩn: /screening-review/{screeningId}"
            : string.Empty;

        var systemMessageContent =
            $"{emergencyPrefix}[HỘI CHẨN LÂM SÀNG]{reasonSection}{medicalRecordSection}{screeningSection}\nPhiên hội chẩn này sẽ tự động kết thúc sau 20 phút.";

        var systemMessage = new InternalGroupMessage(
            group.Id,
            currentUserId,
            AuthorType.Ophthalmologist,
            systemMessageContent
        );
        group.AddMessage(systemMessage);

        await _groupChatRepository.AddAsync(group, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify invited doctors via SignalR — send to their UserId (not ophthalmologistId)
        foreach (var ophthalmologistId in invitedOphthalmologistIds)
        {
            if (!ophthalToUserIdMap.TryGetValue(ophthalmologistId, out var invitedUserId))
                continue;
            if (invitedUserId == currentUserId) continue;

            var notificationTitle = request.IsEmergency
                ? "🚨 Mời hội chẩn KHẨN CẤP"
                : "Mời hội chẩn lâm sàng";

            await _notificationService.SendAsync(
                invitedUserId,
                notificationTitle,
                $"Bác sĩ {_currentUserService.UserName} mời bạn hội chẩn ca bệnh: {groupName}",
                NotificationType.ConsiliumInvitation,
                new {
                    GroupId = group.Id,
                    SessionId = request.ConsultationSessionId,
                    InviterName = _currentUserService.UserName,
                    GroupName = groupName,
                    IsEmergency = request.IsEmergency,
                    MedicalRecordId = request.MedicalRecordId
                },
                cancellationToken,
                group.Id);
        }

        await _chatHubService.BroadcastGroupUpdateAsync(group.Id, "GroupCreated", cancellationToken);

        return Result<Guid>.Success(group.Id);
    }
}
