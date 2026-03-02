using Application.Common.Interfaces;
using Application.Wallets.Common;
using Domain.Enums;

namespace Application.Wallets.Commands.CreateDeposit;

/// <summary>
/// Command to create a deposit request and get PayOS payment link.
/// </summary>
public record CreateDepositCommand : ICommand<CreateDepositResponse>
{
    public Guid UserId { get; init; }
    public decimal AmountVnd { get; init; }
    public PaymentMethod PaymentMethod { get; init; } = PaymentMethod.PayOS;
    public string? Description { get; init; }
    public string? ReturnUrl { get; init; }
    public string? CancelUrl { get; init; }
}
