using Application.Common.Interfaces;
using Application.SystemAdmin.Ophthalmologists.Commands.PaySalary;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Hangfire job to automate monthly salary payouts for ophthalmologists.
/// Runs on a scheduled basis (e.g., 5th of every month).
/// Deducts 10% for Tax and BHXH.
/// </summary>
public class MonthlySalaryJob
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IPayOSPayoutService _payOSPayoutService;
    private readonly IMediator _mediator;
    private readonly IWalletRepository _walletRepository;
    private readonly ILogger<MonthlySalaryJob> _logger;
    private readonly INotificationService _notificationService;

    public MonthlySalaryJob(
        IOphthalmologistRepository ophthalmologistRepository,
        IPayOSPayoutService payOSPayoutService,
        IMediator mediator,
        IWalletRepository walletRepository,
        ILogger<MonthlySalaryJob> logger,
        INotificationService notificationService)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _payOSPayoutService = payOSPayoutService;
        _mediator = mediator;
        _walletRepository = walletRepository;
        _logger = logger;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Executes the monthly salary payout logic.
    /// </summary>
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting automated monthly salary payout job at {Time}", DateTime.UtcNow);

        // 1. Fetch all verified ophthalmologists who have a monthly salary configured.
        var doctors = await _ophthalmologistRepository.GetVerifiedAsync(cancellationToken);
        var eligibleDoctors = doctors.Where(d => d.ActualMonthlySalary.HasValue && d.ActualMonthlySalary.Value > 0).ToList();

        if (!eligibleDoctors.Any())
        {
            _logger.LogInformation("No eligible ophthalmologists found for salary payout this month.");
            return;
        }

        // 2. Pre-check: Check PayOS payout account balance (optional safeguard if we were doing auto-payouts).
        // For now, we deposit to internal wallets as per the initial workflow, but check balance anyway to alert admin.
        try
        {
            var balanceInfo = await _payOSPayoutService.GetPayoutAccountBalanceAsync(cancellationToken);
            _logger.LogInformation("Current PayOS Payout Account Balance: {Balance} {Currency}", balanceInfo.Balance, balanceInfo.Currency);
            
            // Calculate total net salary to be paid
            decimal totalRequired = eligibleDoctors.Sum(d => d.ActualMonthlySalary!.Value * 0.9m);
            
            if (balanceInfo.Balance < (long)totalRequired)
            {
                var msg = $"[WARNING] Total required salary ({totalRequired:N0} VND) exceeds PayOS Payout Balance ({balanceInfo.Balance:N0} VND). Please top up.";
                _logger.LogWarning(msg);
                await _notificationService.SendToRoleAsync(
                    Application.Common.Constants.Roles.SystemAdmin,
                    "Insufficient Funds for Salary Payout", 
                    msg, 
                    cancellationToken: cancellationToken);
                // because doctors will still need to request withdrawal later.
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check PayOS account balance during salary job.");
        }

        int successCount = 0;
        int failedCount = 0;

        foreach (var doctor in eligibleDoctors)
        {
            try
            {
                // Safety check: Don't pay if already paid this month
                bool alreadyPaid = await _walletRepository.HasSalaryBeenPaidAsync(
                    doctor.UserId, 
                    DateTime.UtcNow.Year, 
                    DateTime.UtcNow.Month, 
                    cancellationToken);

                if (alreadyPaid)
                {
                    _logger.LogInformation("Salary for doctor {DoctorId} already paid this month. Skipping.", doctor.Id);
                    continue;
                }

                // Net salary = 90% of ActualMonthlySalary (10% deduction for Tax/BHXH)
                decimal netSalary = doctor.ActualMonthlySalary!.Value * 0.9m;
                string note = $"Automated Monthly Salary ({DateTime.UtcNow:MM/yyyy}) - 10% Tax/BHXH deducted.";

                var command = new PayOphthalmologistSalaryCommand
                {
                    OphthalmologistId = doctor.Id,
                    Amount = netSalary,
                    Note = note
                };

                var result = await _mediator.Send(command, cancellationToken);

                if (result.IsSuccess)
                {
                    successCount++;
                    _logger.LogInformation("Successfully processed salary for doctor {DoctorId}: {Amount} VND", doctor.Id, netSalary);
                }
                else
                {
                    failedCount++;
                    _logger.LogError("Failed to process salary for doctor {DoctorId}: {Error}", doctor.Id, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                failedCount++;
                _logger.LogError(ex, "Unexpected error processing salary for doctor {DoctorId}", doctor.Id);
            }
        }

        _logger.LogInformation("Monthly salary payout job finished. Success: {Success}, Failed: {Failed}", successCount, failedCount);
        
        if (failedCount > 0)
        {
            await _notificationService.SendToRoleAsync(
                Application.Common.Constants.Roles.SystemAdmin,
                "Salary Payout Job Completed with Errors", 
                $"Processed {successCount} successes and {failedCount} failures.", 
                cancellationToken: cancellationToken);
        }
    }
}
