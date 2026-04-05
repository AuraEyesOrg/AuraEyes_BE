using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Wallets.Commands.CreateWithdrawalRequest;
using Domain.Common;
using Domain.Entities.Contracts;
using Domain.Entities.Financial;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Handlers;

public class CreateWithdrawalRequestCommandHandlerTests
{
    private readonly IWalletRepository _walletRepository;
    private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateWithdrawalRequestCommandHandler _handler;

    public CreateWithdrawalRequestCommandHandlerTests()
    {
        _walletRepository = Substitute.For<IWalletRepository>();
        _withdrawalRequestRepository = Substitute.For<IWithdrawalRequestRepository>();
        _contractRepository = Substitute.For<IContractRepository>();
        _identityService = Substitute.For<IIdentityService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _handler = new CreateWithdrawalRequestCommandHandler(
            _walletRepository,
            _withdrawalRequestRepository,
            _contractRepository,
            _identityService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenUserNotOphthalmologist_ShouldReturnForbidden()
    {
        var userId = Guid.NewGuid();
        var command = BuildValidCommand(userId);

        _identityService.IsInRoleAsync(userId, Roles.Ophthalmologist).Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsForbidden.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Contains("Only ophthalmologists"));
    }

    [Fact]
    public async Task Handle_WhenPendingRequestExists_ShouldReturnConflict()
    {
        var userId = Guid.NewGuid();
        var command = BuildValidCommand(userId);

        _identityService.IsInRoleAsync(userId, Roles.Ophthalmologist).Returns(true);
        _withdrawalRequestRepository.HasPendingRequestAsync(userId, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsConflict.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Contains("pending withdrawal request"));
    }

    [Fact]
    public async Task Handle_WhenWalletNotFound_ShouldReturnNotFound()
    {
        var userId = Guid.NewGuid();
        var command = BuildValidCommand(userId);

        _identityService.IsInRoleAsync(userId, Roles.Ophthalmologist).Returns(true);
        _withdrawalRequestRepository.HasPendingRequestAsync(userId, Arg.Any<CancellationToken>()).Returns(false);
        _walletRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns((Wallet?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsNotFound.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Contains("Wallet not found"));
    }

    [Fact]
    public async Task Handle_WhenInsufficientBalance_ShouldReturnFailure()
    {
        var userId = Guid.NewGuid();
        var command = BuildValidCommand(userId, amount: 500_000m);
        var wallet = new Wallet(userId, "Ophthalmologist", 100_000m);

        _identityService.IsInRoleAsync(userId, Roles.Ophthalmologist).Returns(true);
        _withdrawalRequestRepository.HasPendingRequestAsync(userId, Arg.Any<CancellationToken>()).Returns(false);
        _walletRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(wallet);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("Insufficient wallet balance"));
    }

    [Fact]
    public async Task Handle_WhenLegacyOwnerType_ShouldMigrateAndCreateRequest()
    {
        var userId = Guid.NewGuid();
        var command = BuildValidCommand(userId, amount: 80_000m);
        var wallet = new Wallet(userId, "Patient", 200_000m);

        _identityService.IsInRoleAsync(userId, Roles.Ophthalmologist).Returns(true);
        _withdrawalRequestRepository.HasPendingRequestAsync(userId, Arg.Any<CancellationToken>()).Returns(false);
        _walletRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(wallet);
        _withdrawalRequestRepository.AddAsync(Arg.Any<WithdrawalRequest>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<WithdrawalRequest>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Amount.Should().Be(80_000m);
        result.Data.Status.Should().Be(Domain.Enums.PaymentStatus.Pending);
        wallet.OwnerType.Should().Be("Ophthalmologist");

        await _unitOfWork.Received(2).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenContractNumberNotProvided_ShouldUseActiveContractNumber()
    {
        var userId = Guid.NewGuid();
        var command = BuildValidCommand(userId, contractNumber: null);
        var wallet = new Wallet(userId, "Ophthalmologist", 500_000m);
        var contract = BuildContract(userId, "CTR-2026-001");

        _identityService.IsInRoleAsync(userId, Roles.Ophthalmologist).Returns(true);
        _withdrawalRequestRepository.HasPendingRequestAsync(userId, Arg.Any<CancellationToken>()).Returns(false);
        _walletRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(wallet);
        _contractRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(contract);
        _withdrawalRequestRepository.AddAsync(Arg.Any<WithdrawalRequest>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<WithdrawalRequest>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ContractNumber.Should().Be("CTR-2026-001");
    }

    private static CreateWithdrawalRequestCommand BuildValidCommand(
        Guid userId,
        decimal amount = 100_000m,
        string? contractNumber = "HD-001") =>
        new()
        {
            UserId = userId,
            AmountVnd = amount,
            BankName = "Vietcombank",
            BankAccountNumber = "1234567890",
            AccountHolderName = "Nguyen Van A",
            ContractNumber = contractNumber,
            Note = "Monthly payout"
        };

    private static Contract BuildContract(Guid userId, string contractNumber)
    {
        return new Contract(
            userId,
            Guid.NewGuid(),
            contractNumber,
            aiQuotaLimit: 0,
            platformCommissionRate: 0m,
            signedContent: "Test contract");
    }
}
