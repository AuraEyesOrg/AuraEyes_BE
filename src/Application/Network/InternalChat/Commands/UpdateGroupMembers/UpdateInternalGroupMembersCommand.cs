using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network.InternalChat;
using Domain.Enums.Network;

namespace Application.Network.InternalChat.Commands.UpdateGroupMembers;

public class UpdateInternalGroupMembersCommand : ICommand
{
    public Guid GroupId { get; set; }
    public List<Guid> MemberIds { get; set; } = new();
}

public class UpdateInternalGroupMembersCommandHandler : ICommandHandler<UpdateInternalGroupMembersCommand>
{
    private readonly IRepository<InternalGroupChat> _groupChatRepository;
    private readonly IInternalChatHubService _chatHubService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInternalGroupMembersCommandHandler(
        IRepository<InternalGroupChat> groupChatRepository, 
        IUnitOfWork unitOfWork,
        IInternalChatHubService chatHubService)
    {
        _groupChatRepository = groupChatRepository;
        _unitOfWork = unitOfWork;
        _chatHubService = chatHubService;
    }

    public async Task<Result> Handle(UpdateInternalGroupMembersCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupChatRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group == null)
            return Result.Failure("Group not found");

        // Simple mapping: default all to ClinicStaff for now to match CreateInternalGroupChatCommand pattern.
        // In a more complex system, we'd query the DB for each user's primary role.
        var membersToSync = request.MemberIds
            .Select(id => (id, AuthorType.ClinicStaff))
            .ToList();

        group.UpdateMembers(membersToSync);
        
        await _groupChatRepository.UpdateAsync(group, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _chatHubService.BroadcastGroupUpdateAsync(group.Id, "MembersUpdated", cancellationToken);

        return Result.Success();
    }
}
