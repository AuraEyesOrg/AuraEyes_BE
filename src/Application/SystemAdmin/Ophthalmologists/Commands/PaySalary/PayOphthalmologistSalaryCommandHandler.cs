using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;

namespace Application.SystemAdmin.Ophthalmologists.Commands.PaySalary;

public class PayOphthalmologistSalaryCommandHandler : ICommandHandler<PayOphthalmologistSalaryCommand, string>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PayOphthalmologistSalaryCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(PayOphthalmologistSalaryCommand request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(
            request.OphthalmologistId,
            cancellationToken);

        if (ophthalmologist is null)
        {
            return Result<string>.NotFound($"Ophthalmologist '{request.OphthalmologistId}' not found.");
        }

        var payoutAmount = request.Amount ?? ophthalmologist.ActualMonthlySalary;
        if (!payoutAmount.HasValue || payoutAmount.Value <= 0)
        {
            return Result<string>.Failure("Salary amount must be provided and greater than 0.");
        }

        var wallet = await _walletRepository.GetByUserIdAsync(ophthalmologist.UserId, cancellationToken);
        if (wallet is null)
        {
            wallet = new Wallet(ophthalmologist.UserId, "Ophthalmologist", 0);
            await _walletRepository.AddAsync(wallet, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var noteText = string.IsNullOrWhiteSpace(request.Note)
            ? $"Salary payout ({DateTime.UtcNow:yyyy-MM})"
            : request.Note!.Trim();

        wallet.Deposit(payoutAmount.Value, noteText);

        var transaction = new WalletTransaction(
            wallet.Id,
            payoutAmount.Value,
            TransactionType.Bonus,
            noteText,
            referenceType: "Salary",
            referenceId: ophthalmologist.Id);

        wallet.AddTransaction(transaction);
        await _walletRepository.AddTransactionAsync(transaction, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(
            $"Paid {payoutAmount.Value:N0} VND to ophthalmologist wallet successfully.");
    }
}
