using Application.Common.Interfaces;
using Application.Wallets.Common;

namespace Application.Wallets.Queries.GetWallet;

/// <summary>
/// Query to get wallet by user ID.
/// </summary>
public record GetWalletQuery(Guid UserId) : IQuery<WalletDto>;
