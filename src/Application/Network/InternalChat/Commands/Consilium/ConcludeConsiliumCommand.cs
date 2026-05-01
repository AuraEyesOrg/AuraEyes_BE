using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Network.InternalChat;
using Domain.Enums.Network;

namespace Application.Network.InternalChat.Commands.Consilium;

public class ConcludeConsiliumCommand : ICommand<bool>
{
    public Guid GroupId { get; set; }
}

public class ConcludeConsiliumCommandHandler : ICommandHandler<ConcludeConsiliumCommand, bool>
{
    private readonly IRepository<InternalGroupChat> _groupChatRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IInternalChatHubService _chatHubService;

    public ConcludeConsiliumCommandHandler(
        IRepository<InternalGroupChat> groupChatRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IInternalChatHubService chatHubService)
    {
        _groupChatRepository = groupChatRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _chatHubService = chatHubService;
    }

    public async Task<Result<bool>> Handle(ConcludeConsiliumCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupChatRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group == null) return Result<bool>.Failure("Group not found");

        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();

        try
        {
            group.ConcludeConsilium(currentUserId);
            
            // Add a system message about conclusion
            var systemMessage = new InternalGroupMessage(
                group.Id,
                currentUserId,
                AuthorType.SystemAdmin,
                "Hội chẩn đã kết thúc. Nhóm chat hiện chỉ ở chế độ xem."
            );
            group.AddMessage(systemMessage);

            await _groupChatRepository.UpdateAsync(group, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _chatHubService.BroadcastGroupUpdateAsync(group.Id, "ConsiliumConcluded", cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
