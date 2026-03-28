using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;
using Domain.Repositories;

namespace Application.SystemAdmin.Ophthalmologists.Queries.GetWithdrawalRequests;

public class GetWithdrawalRequestsQueryHandler : IQueryHandler<GetWithdrawalRequestsQuery, PagedResult<AdminWithdrawalRequestDto>>
{
    private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;
    private readonly IIdentityService _identityService;

    public GetWithdrawalRequestsQueryHandler(
        IWithdrawalRequestRepository withdrawalRequestRepository,
        IIdentityService identityService)
    {
        _withdrawalRequestRepository = withdrawalRequestRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<AdminWithdrawalRequestDto>>> Handle(
        GetWithdrawalRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _withdrawalRequestRepository.GetPagedAsync(
            request.Status,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = new List<AdminWithdrawalRequestDto>(items.Count);

        foreach (var item in items)
        {
            var user = await _identityService.GetUserByIdAsync(item.UserId, cancellationToken);
            dtos.Add(new AdminWithdrawalRequestDto
            {
                Id = item.Id,
                UserId = item.UserId,
                WalletId = item.WalletId,
                Amount = item.Amount,
                Status = item.Status,
                BankName = item.BankName,
                BankAccountNumber = item.BankAccountNumber,
                AccountHolderName = item.AccountHolderName,
                ContractNumber = item.ContractNumber,
                Note = item.Note,
                AdminNote = item.AdminNote,
                TransferReference = item.TransferReference,
                ProcessedByAdminId = item.ProcessedByAdminId,
                ProcessedAt = item.ProcessedAt,
                CreatedAt = item.CreatedAt,
                DoctorFullName = user?.FullName ?? "Unknown",
                DoctorEmail = user?.Email ?? string.Empty
            });
        }

        var pagedResult = new PagedResult<AdminWithdrawalRequestDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<AdminWithdrawalRequestDto>>.Success(pagedResult);
    }
}
