using Application.Common.Interfaces;
using MediatR;

namespace Application.Network.InternalChat.Queries.GetInternalUsers;

public class GetInternalUsersQuery : IRequest<List<UserAdminDto>>
{
    public string? SearchTerm { get; set; }
}

public class GetInternalUsersQueryHandler : IRequestHandler<GetInternalUsersQuery, List<UserAdminDto>>
{
    private readonly IIdentityService _identityService;

    public GetInternalUsersQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<List<UserAdminDto>> Handle(GetInternalUsersQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetInternalUsersAsync(request.SearchTerm, cancellationToken);
    }
}
