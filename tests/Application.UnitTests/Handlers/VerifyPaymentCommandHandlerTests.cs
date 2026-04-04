using Application.Common.Interfaces;
using Application.Wallets.Commands.VerifyPayment;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Application.UnitTests.Handlers;

public class VerifyPaymentCommandHandlerTests
{
    private readonly IDepositRequestRepository _depositRequestRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IPayOSService _payOSService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VerifyPaymentCommandHandler> _logger;
    private readonly VerifyPaymentCommandHandler _handler;

    public VerifyPaymentCommandHandlerTests()
    {
        _depositRequestRepository = Substitute.For<IDepositRequestRepository>();
        _walletRepository = Substitute.For<IWalletRepository>();
        _payOSService = Substitute.For<IPayOSService>();
        _notificationService = Substitute.For<INotificationService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _logger = Substitute.For<ILogger<VerifyPaymentCommandHandler>>();

        _handler = new VerifyPaymentCommandHandler(
            _depositRequestRepository,
            _walletRepository,
            _payOSService,
            _notificationService,
            _unitOfWork,
            _logger);
    }

    [Fact]
    public async Task Handle_WhenDepositRequestNotFound_ShouldReturnNotFound()
    {
        _depositRequestRepository.GetByOrderCodeAsync("ORD-1", Arg.Any<CancellationToken>())
            .Returns((DepositRequest?)null);

        var result = await _handler.Handle(new VerifyPaymentCommand { OrderCode = "ORD-1" }, CancellationToken.None);

        result.IsNotFound.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Contains("Deposit request not found"));
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotOwnDeposit_ShouldReturnForbidden()
    {
        var ownerId = Guid.NewGuid();
        var otherUser = Guid.NewGuid();
        var deposit = BuildPendingDeposit(ownerId, orderCode: "ORD-2");

        _depositRequestRepository.GetByOrderCodeAsync("ORD-2", Arg.Any<CancellationToken>()).Returns(deposit);

        var result = await _handler.Handle(
            new VerifyPaymentCommand { OrderCode = "ORD-2", UserId = otherUser },
            CancellationToken.None);

        result.IsForbidden.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenAlreadyCompleted_ShouldReturnCurrentBalance()
    {
        var ownerId = Guid.NewGuid();
        var deposit = BuildPendingDeposit(ownerId, orderCode: "ORD-3");
        deposit.Complete("TXN-1");
        var wallet = new Wallet(ownerId, "Patient", 1_000_000m);

        _depositRequestRepository.GetByOrderCodeAsync("ORD-3", Arg.Any<CancellationToken>()).Returns(deposit);
        _walletRepository.GetByIdAsync(deposit.WalletId, Arg.Any<CancellationToken>()).Returns(wallet);

        var result = await _handler.Handle(
            new VerifyPaymentCommand { OrderCode = "ORD-3", UserId = ownerId },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.IsSuccess.Should().BeTrue();
        result.Data.Status.Should().Be(PaymentStatus.Completed.ToString());
        result.Data.NewBalance.Should().Be(1_000_000m);
    }

    [Fact]
    public async Task Handle_WhenPayosCancelled_ShouldCancelDepositAndReturnUnsuccessful()
    {
        var ownerId = Guid.NewGuid();
        var deposit = BuildPendingDeposit(ownerId, orderCode: "ORD-4");

        _depositRequestRepository.GetByOrderCodeAsync("ORD-4", Arg.Any<CancellationToken>()).Returns(deposit);
        _payOSService.GetPaymentStatusAsync("ORD-4").Returns(("CANCELLED", deposit.Amount, ""));

        var result = await _handler.Handle(new VerifyPaymentCommand { OrderCode = "ORD-4" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.IsSuccess.Should().BeFalse();
        result.Data.Status.Should().Be("Cancelled");
        deposit.Status.Should().Be(PaymentStatus.Cancelled);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPayosPaidAndWalletFound_ShouldCreditWalletAndPersistTransaction()
    {
        var ownerId = Guid.NewGuid();
        var deposit = BuildPendingDeposit(ownerId, amount: 250_000m, orderCode: "ORD-5");
        var wallet = new Wallet(ownerId, "Patient", 100_000m);

        _depositRequestRepository.GetByOrderCodeAsync("ORD-5", Arg.Any<CancellationToken>()).Returns(deposit);
        _payOSService.GetPaymentStatusAsync("ORD-5").Returns(("PAID", 250_000m, "TXN-5"));
        _walletRepository.GetByIdAsync(deposit.WalletId, Arg.Any<CancellationToken>()).Returns(wallet);

        var result = await _handler.Handle(new VerifyPaymentCommand { OrderCode = "ORD-5" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.IsSuccess.Should().BeTrue();
        result.Data.Status.Should().Be("Completed");
        result.Data.NewBalance.Should().Be(350_000m);
        wallet.Balance.Should().Be(350_000m);
        deposit.Status.Should().Be(PaymentStatus.Completed);

        await _walletRepository.Received(1).AddTransactionAsync(
            Arg.Is<WalletTransaction>(tx =>
                tx.WalletId == wallet.Id &&
                tx.Amount == 250_000m &&
                tx.TransactionType == TransactionType.Deposit),
            Arg.Any<CancellationToken>());

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _notificationService.Received(1).SendAsync(
            ownerId,
            "Nạp tiền thành công",
            Arg.Any<string>(),
            NotificationType.WalletDepositSuccess,
            Arg.Any<object>(),
            Arg.Any<CancellationToken>(),
            Arg.Any<Guid?>());
    }

    [Fact]
    public async Task Handle_WhenPayosPaidButWalletMissing_ShouldReturnFailure()
    {
        var ownerId = Guid.NewGuid();
        var deposit = BuildPendingDeposit(ownerId, amount: 250_000m, orderCode: "ORD-6");

        _depositRequestRepository.GetByOrderCodeAsync("ORD-6", Arg.Any<CancellationToken>()).Returns(deposit);
        _payOSService.GetPaymentStatusAsync("ORD-6").Returns(("PAID", 250_000m, "TXN-6"));
        _walletRepository.GetByIdAsync(deposit.WalletId, Arg.Any<CancellationToken>()).Returns((Wallet?)null);

        var result = await _handler.Handle(new VerifyPaymentCommand { OrderCode = "ORD-6" }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("Wallet not found"));

        await _walletRepository.DidNotReceive().AddTransactionAsync(Arg.Any<WalletTransaction>(), Arg.Any<CancellationToken>());
    }

    private static DepositRequest BuildPendingDeposit(Guid userId, decimal amount = 100_000m, string orderCode = "ORDER-1")
    {
        var walletId = Guid.NewGuid();
        var deposit = new DepositRequest(
            userId,
            walletId,
            amount,
            PaymentMethod.PayOS,
            "https://return.local",
            "https://cancel.local",
            "Top up");
        deposit.SetPaymentLink("https://pay.local", orderCode);

        return deposit;
    }
}
