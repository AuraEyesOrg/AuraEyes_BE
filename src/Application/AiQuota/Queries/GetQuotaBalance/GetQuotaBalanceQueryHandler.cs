using Application.AiQuota.Common;
using Application.AiQuota.Interfaces;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.AiQuota.Queries.GetQuotaBalance;

public class GetQuotaBalanceQueryHandler : IQueryHandler<GetQuotaBalanceQuery, QuotaBalanceDto>
{
    private readonly IAiQuotaService _quotaService;
    private readonly ICurrentUserService _currentUser;

    public GetQuotaBalanceQueryHandler(IAiQuotaService quotaService, ICurrentUserService currentUser)
    {
        _quotaService = quotaService;
        _currentUser = currentUser;
    }

    public async Task<Result<QuotaBalanceDto>> Handle(GetQuotaBalanceQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Result<QuotaBalanceDto>.Unauthorized("User is not authenticated.");

        var role = _currentUser.Roles.FirstOrDefault() ?? "Patient";
        var quota = await _quotaService.GetQuotaAsync(_currentUser.UserId.Value, role, cancellationToken);

        return Result<QuotaBalanceDto>.Success(new QuotaBalanceDto
        {
            TotalAiQuota = quota.TotalQuota,
            UsedAiQuota = quota.UsedQuota,
            RemainingQuota = quota.RemainingQuota,
            QuotaSource = quota.QuotaSource,
            UnitPrice = quota.UnitPrice
        });
    }
}
