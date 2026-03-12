using Application.AiQuota.Common;
using Application.Common.Interfaces;

namespace Application.AiQuota.Queries.GetQuotaBalance;

public record GetQuotaBalanceQuery : IQuery<QuotaBalanceDto>;
