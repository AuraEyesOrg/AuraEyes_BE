using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network.InternalChat;
using Domain.Enums.Network;

namespace Application.Network.InternalChat.Commands.CreateGroupChat;

public class CreateInternalGroupChatCommand : ICommand<Guid>
{
    public string? Name { get; set; }
    public InternalGroupType Type { get; set; }
    public Guid? ConsultationSessionId { get; set; }
    public List<Guid> MemberIds { get; set; } = new();
}

public class CreateInternalGroupChatCommandHandler : ICommandHandler<CreateInternalGroupChatCommand, Guid>
{
    private readonly IRepository<InternalGroupChat> _groupChatRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateInternalGroupChatCommandHandler(IRepository<InternalGroupChat> groupChatRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _groupChatRepository = groupChatRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(CreateInternalGroupChatCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var currentUserRole = _currentUserService.Roles.FirstOrDefault();
        
        var creatorType = currentUserRole switch
        {
            "Ophthalmologist" => AuthorType.Ophthalmologist,
            "ClinicStaff" => AuthorType.ClinicStaff,
            "SystemAdmin" => AuthorType.SystemAdmin,
            _ => throw new UnauthorizedAccessException("Invalid role for creating internal chat")
        };

        var group = new InternalGroupChat(request.Name, request.Type, currentUserId, creatorType, request.ConsultationSessionId);
        
        group.AddMember(currentUserId, creatorType);
        
        foreach (var memberId in request.MemberIds)
        {
            if (memberId != currentUserId)
            {
                // In a real scenario, we should look up the member's role to determine AuthorType
                group.AddMember(memberId, AuthorType.ClinicStaff); 
            }
        }

        await _groupChatRepository.AddAsync(group, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(group.Id);
    }
}

