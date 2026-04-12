using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.SystemAdmin.Cashflow.Queries.GetCashflowTransactions;

public class GetCashflowTransactionsQueryHandler : IQueryHandler<GetCashflowTransactionsQuery, PagedResult<CashflowTransactionDto>>
{
    private static readonly HashSet<string> SupportedActorRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Patient",
        "Ophthalmologist",
        "Organisation",
        "System"
    };

    private readonly IWalletRepository _walletRepository;
    private readonly IIdentityService _identityService;

    public GetCashflowTransactionsQueryHandler(
        IWalletRepository walletRepository,
        IIdentityService identityService)
    {
        _walletRepository = walletRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<CashflowTransactionDto>>> Handle(
        GetCashflowTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 200);

        var statusFilter = request.Status?.Trim();
        if (!string.IsNullOrWhiteSpace(statusFilter)
            && !statusFilter.Equals("Completed", StringComparison.OrdinalIgnoreCase))
        {
            var empty = new PagedResult<CashflowTransactionDto>(
                new List<CashflowTransactionDto>(),
                0,
                pageNumber,
                pageSize);

            return Result<PagedResult<CashflowTransactionDto>>.Success(empty);
        }

        var normalizedActorRole = NormalizeActorRole(request.ActorRole);

        var (items, totalCount) = await _walletRepository.GetCashflowTransactionsPagedAsync(
            normalizedActorRole,
            request.SearchTerm,
            pageNumber,
            pageSize,
            cancellationToken);

        var userCache = new Dictionary<Guid, UserDto?>();
        var rows = new List<CashflowTransactionDto>(items.Count);

        foreach (var (transaction, wallet) in items)
        {
            if (!userCache.TryGetValue(wallet.UserId, out var user))
            {
                user = await _identityService.GetUserByIdAsync(wallet.UserId, cancellationToken);
                userCache[wallet.UserId] = user;
            }

            var actorName = user?.FullName;
            if (string.IsNullOrWhiteSpace(actorName))
            {
                actorName = wallet.OwnerType.Equals("System", StringComparison.OrdinalIgnoreCase)
                    ? "System Wallet"
                    : "Unknown";
            }

            rows.Add(new CashflowTransactionDto
            {
                Id = transaction.Id,
                ActorName = actorName,
                ActorEmail = user?.Email,
                ActorRole = wallet.OwnerType,
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType.ToString(),
                ReferenceType = transaction.ReferenceType,
                ReferenceId = transaction.ReferenceId,
                BookingCode = transaction.ReferenceType?.Equals("Booking", StringComparison.OrdinalIgnoreCase) == true
                    ? transaction.ReferenceId?.ToString()
                    : null,
                Status = "Completed",
                Description = transaction.Description,
                CreatedAt = transaction.CreatedAt,
            });
        }

        var pagedResult = new PagedResult<CashflowTransactionDto>(
            rows,
            totalCount,
            pageNumber,
            pageSize);

        return Result<PagedResult<CashflowTransactionDto>>.Success(pagedResult);
    }

    private static string? NormalizeActorRole(string? actorRole)
    {
        if (string.IsNullOrWhiteSpace(actorRole))
        {
            return null;
        }

        var trimmed = actorRole.Trim();
        if (!SupportedActorRoles.Contains(trimmed))
        {
            return null;
        }

        return SupportedActorRoles.First(x => x.Equals(trimmed, StringComparison.OrdinalIgnoreCase));
    }
}
