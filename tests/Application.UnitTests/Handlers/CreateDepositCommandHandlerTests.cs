using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Wallets.Commands.CreateDeposit;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Application.UnitTests.Handlers;

public class CreateDepositCommandHandlerTests
{
    private readonly IWalletRepository _walletRepository;
    private readonly IDepositRequestRepository _depositRequestRepository;
    private readonly IPayOSService _payOSService;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDepositCommandHandler> _logger;
    private readonly CreateDepositCommandHandler _handler;

    public CreateDepositCommandHandlerTests()
    {
        _walletRepository = Substitute.For<IWalletRepository>();
        _depositRequestRepository = Substitute.For<IDepositRequestRepository>();
        _payOSService = Substitute.For<IPayOSService>();
        _identityService = Substitute.For<IIdentityService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _logger = Substitute.For<ILogger<CreateDepositCommandHandler>>();

        _handler = new CreateDepositCommandHandler(
            _walletRepository,
            _depositRequestRepository,
            _payOSService,
            _identityService,
            _unitOfWork,
            _logger);
    }

    [Fact]
    public async Task Handle_WhenWalletExists_ShouldCreateDepositRequestAndReturnPaymentLink()
    {
        var userId = Guid.NewGuid();
        var wallet = new Wallet(userId, "Patient", 100_000m);

        var command = new CreateDepositCommand
        {
            UserId = userId,
            AmountVnd = 300_000m,
            PaymentMethod = PaymentMethod.PayOS,
            Description = "Top up",
            ReturnUrl = "https://return.local",
            CancelUrl = "https://cancel.local"
        };

        _walletRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(wallet);
        _depositRequestRepository.AddAsync(Arg.Any<DepositRequest>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<DepositRequest>());
        _payOSService.CreatePaymentLinkAsync(Arg.Any<Guid>(), 300_000m, "Top up", "https://return.local", "https://cancel.local")
            .Returns(("https://pay.local/checkout", "ORDER-123"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Amount.Should().Be(300_000m);
        result.Data.PaymentUrl.Should().Be("https://pay.local/checkout");
        result.Data.OrderCode.Should().Be("ORDER-123");
        result.Data.Status.Should().Be(PaymentStatus.Pending.ToString());

        await _walletRepository.DidNotReceive().AddAsync(Arg.Any<Wallet>(), Arg.Any<CancellationToken>());
        await _depositRequestRepository.Received(1).AddAsync(
            Arg.Is<DepositRequest>(d =>
                d.UserId == userId &&
                d.Amount == 300_000m &&
                d.PaymentMethod == PaymentMethod.PayOS),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(2).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenWalletMissing_ShouldCreateWalletWithRoleBasedOwnerType()
    {
        var userId = Guid.NewGuid();
        var command = new CreateDepositCommand
        {
            UserId = userId,
            AmountVnd = 150_000m,
            PaymentMethod = PaymentMethod.PayOS
        };

        _walletRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((Wallet?)null);
        _identityService.GetUserRolesAsync(userId)
            .Returns(new List<string> { Roles.Ophthalmologist });
        _walletRepository.AddAsync(Arg.Any<Wallet>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<Wallet>());
        _depositRequestRepository.AddAsync(Arg.Any<DepositRequest>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<DepositRequest>());
        _payOSService.CreatePaymentLinkAsync(Arg.Any<Guid>(), Arg.Any<decimal>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(("https://pay.local/checkout", "ORDER-234"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        await _walletRepository.Received(1).AddAsync(
            Arg.Is<Wallet>(w => w.UserId == userId && w.OwnerType == "Ophthalmologist"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDescriptionNull_ShouldUseFallbackDescription()
    {
        var userId = Guid.NewGuid();
        var wallet = new Wallet(userId, "Patient", 0m);
        var command = new CreateDepositCommand
        {
            UserId = userId,
            AmountVnd = 200_000m,
            PaymentMethod = PaymentMethod.PayOS,
            Description = null,
            ReturnUrl = null,
            CancelUrl = null
        };

        _walletRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(wallet);
        _depositRequestRepository.AddAsync(Arg.Any<DepositRequest>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<DepositRequest>());
        _payOSService.CreatePaymentLinkAsync(
                Arg.Any<Guid>(),
                200_000m,
                Arg.Is<string>(d => d.Contains("Nap tien AuraEyes")),
                string.Empty,
                string.Empty)
            .Returns(("https://pay.local/checkout", "ORDER-345"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _payOSService.Received(1).CreatePaymentLinkAsync(
            Arg.Any<Guid>(),
            200_000m,
            Arg.Is<string>(d => d.Contains("Nap tien AuraEyes")),
            string.Empty,
            string.Empty);
    }

    [Fact]
    public async Task Handle_WhenPaymentProviderThrows_ShouldReturnFailure()
    {
        var userId = Guid.NewGuid();
        var wallet = new Wallet(userId, "Patient", 10_000m);
        var command = new CreateDepositCommand
        {
            UserId = userId,
            AmountVnd = 100_000m,
            PaymentMethod = PaymentMethod.PayOS,
            Description = "Top up"
        };

        _walletRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(wallet);
        _depositRequestRepository.AddAsync(Arg.Any<DepositRequest>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<DepositRequest>());
        _payOSService.CreatePaymentLinkAsync(Arg.Any<Guid>(), Arg.Any<decimal>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns<Task<(string PaymentUrl, string OrderCode)>>(x => throw new InvalidOperationException("gateway down"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Failed to create deposit"));
    }
}
