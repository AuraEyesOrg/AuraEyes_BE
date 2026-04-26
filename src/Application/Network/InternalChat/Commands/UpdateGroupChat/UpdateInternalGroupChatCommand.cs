using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network.InternalChat;

namespace Application.Network.InternalChat.Commands.UpdateGroupChat;

public class UpdateInternalGroupChatCommand : ICommand
{
    public Guid GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class UpdateInternalGroupChatCommandHandler : ICommandHandler<UpdateInternalGroupChatCommand>
{
    private readonly IRepository<InternalGroupChat> _groupChatRepository;
    private readonly IInternalChatHubService _chatHubService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInternalGroupChatCommandHandler(
        IRepository<InternalGroupChat> groupChatRepository, 
        IUnitOfWork unitOfWork,
        IInternalChatHubService chatHubService)
    {
        _groupChatRepository = groupChatRepository;
        _unitOfWork = unitOfWork;
        _chatHubService = chatHubService;
    }

    public async Task<Result> Handle(UpdateInternalGroupChatCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupChatRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group == null)
            return Result.Failure("Group not found");

        group.Rename(request.Name);
        
        await _groupChatRepository.UpdateAsync(group, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _chatHubService.BroadcastGroupUpdateAsync(group.Id, "GroupRenamed", cancellationToken);

        return Result.Success();
    }
}
