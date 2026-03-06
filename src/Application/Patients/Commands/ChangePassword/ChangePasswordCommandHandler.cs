using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace Application.Patients.Commands.ChangePassword;

public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IIdentityService identityService,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, errors) = await _identityService.ChangePasswordAsync(
            request.UserId,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        if (!succeeded)
            return Result.Failure(errors);

        _logger.LogInformation("Password changed for user {UserId}", request.UserId);
        return Result.Success();
    }
}
