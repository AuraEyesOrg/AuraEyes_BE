using Application.AiQuota.Common;
using Application.AiQuota.Interfaces;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.AiQuota.Queries.GetAiQuota;

/// <summary>
/// Handler for GetAiQuotaQuery — delegates to IAiQuotaService
/// for cross-layer quota calculation.
/// </summary>
public class GetAiQuotaQueryHandler : IQueryHandler<GetAiQuotaQuery, AiQuotaDto>
{
    private readonly IAiQuotaService _quotaService;
    private readonly ICurrentUserService _currentUser;

    public GetAiQuotaQueryHandler(IAiQuotaService quotaService, ICurrentUserService currentUser)
    {
        _quotaService = quotaService;
        _currentUser = currentUser;
    }

    public async Task<Result<AiQuotaDto>> Handle(GetAiQuotaQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Result<AiQuotaDto>.Failure("User is not authenticated.");

        var role = _currentUser.Roles.FirstOrDefault() ?? "Patient";
        var quota = await _quotaService.GetQuotaAsync(_currentUser.UserId.Value, role, cancellationToken);

        return Result<AiQuotaDto>.Success(quota);
    }
}
