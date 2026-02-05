using Application.Common.Interfaces;
using Application.Wallets.Common;

namespace Application.Wallets.Queries.GetDepositRequest;

/// <summary>
/// Query to get a single deposit request by ID.
/// </summary>
public record GetDepositRequestQuery(Guid Id) : IQuery<DepositRequestDto>;
