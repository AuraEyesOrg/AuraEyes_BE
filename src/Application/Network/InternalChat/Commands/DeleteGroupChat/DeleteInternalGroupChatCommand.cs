using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network.InternalChat;

namespace Application.Network.InternalChat.Commands.DeleteGroupChat;

public class DeleteInternalGroupChatCommand : ICommand
{
    public Guid GroupId { get; set; }
}

public class DeleteInternalGroupChatCommandHandler : ICommandHandler<DeleteInternalGroupChatCommand>
{
    private readonly IRepository<InternalGroupChat> _groupChatRepository;
    private readonly IInternalChatHubService _chatHubService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInternalGroupChatCommandHandler(
        IRepository<InternalGroupChat> groupChatRepository, 
        IUnitOfWork unitOfWork,
        IInternalChatHubService chatHubService)
    {
        _groupChatRepository = groupChatRepository;
        _unitOfWork = unitOfWork;
        _chatHubService = chatHubService;
    }

    public async Task<Result> Handle(DeleteInternalGroupChatCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupChatRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group == null)
            return Result.Failure("Group not found");

        await _groupChatRepository.DeleteAsync(group, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _chatHubService.BroadcastGroupUpdateAsync(group.Id, "GroupDeleted", cancellationToken);

        return Result.Success();
    }
}
