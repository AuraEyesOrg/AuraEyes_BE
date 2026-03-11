using Application.AiQuota.Common;
using Application.Common.Interfaces;

namespace Application.AiQuota.Queries.GetAiQuota;

/// <summary>
/// Query to get AI quota information for the current user.
/// </summary>
public record GetAiQuotaQuery : IQuery<AiQuotaDto>;
