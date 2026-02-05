using Application.Common.Interfaces;
using Application.Wallets.Common;

namespace Application.Wallets.Commands.VerifyPayment;

/// <summary>
/// Command to verify payment status and credit wallet if successful.
/// </summary>
public record VerifyPaymentCommand : ICommand<VerifyPaymentResponse>
{
    public string OrderCode { get; init; } = string.Empty;

    /// <summary>
    /// Optional: User ID for validation (null for webhook/public status check).
    /// </summary>
    public Guid? UserId { get; init; }
}
