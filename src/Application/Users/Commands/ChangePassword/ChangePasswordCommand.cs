using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Users.Commands.ChangePassword;

public record ChangePasswordCommand : ICommand<bool>
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(IIdentityService identityService, ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) return Result<bool>.Failure("Unauthorized.");

        var result = await _identityService.ChangePasswordAsync(userId.Value, request.CurrentPassword, request.NewPassword, cancellationToken);
        
        return result.Succeeded 
            ? Result<bool>.Success(true) 
            : Result<bool>.Failure(result.Errors);
    }
}
