using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.SystemAdmin.Permissions.Commands.SynchronizeRolePermissions;

public record SynchronizeRolePermissionsCommand : IRequest<Result<Unit>>;

public class SynchronizeRolePermissionsCommandHandler : IRequestHandler<SynchronizeRolePermissionsCommand, Result<Unit>>
{
    private readonly IIdentityService _identityService;

    public SynchronizeRolePermissionsCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<Unit>> Handle(SynchronizeRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        await _identityService.SynchronizeRolesWithDefaultsAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
