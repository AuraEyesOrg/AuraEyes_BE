using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Commands.CreateDeposit;
using Application.Wallets.Commands.CreateWithdrawalRequest;
using Application.Wallets.Commands.VerifyPayment;
using Application.Wallets.Queries.GetDepositHistory;
using Application.Wallets.Queries.GetDepositRequest;
using Application.Wallets.Queries.GetWithdrawalHistory;
using Application.Wallets.Queries.GetWallet;
using Application.Wallets.Queries.GetWalletTransactions;
using Application.Wallets.Common;
using API.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace API.UnitTests.Controllers;

public class WalletsControllerTests
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<WalletsController> _logger;
    private readonly WalletsController _controller;

    public WalletsControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _logger = Substitute.For<ILogger<WalletsController>>();
        _controller = new WalletsController(_mediator, _currentUserService, _logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    #region GetWallet Tests

    [Fact]
    public async Task GetWallet_WhenUserNotAuthenticated_ShouldReturn401()
    {
        // Arrange
        _currentUserService.UserId.Returns((Guid?)null);

        // Act
        var result = await _controller.GetWallet();

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetWallet_WhenAuthenticated_ShouldSendQuery()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<GetWalletQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<WalletDto>.Success(new WalletDto
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = 100000m
            }));

        // Act
        var result = await _controller.GetWallet();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(Arg.Is<GetWalletQuery>(q => q.UserId == userId), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetWallet_WhenNotFound_ShouldReturn404()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<GetWalletQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<WalletDto>.NotFound("Wallet not found"));

        // Act
        var result = await _controller.GetWallet();

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetTransactions Tests

    [Fact]
    public async Task GetTransactions_WhenUserNotAuthenticated_ShouldReturn401()
    {
        // Arrange
        _currentUserService.UserId.Returns((Guid?)null);

        // Act
        var result = await _controller.GetTransactions();

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetTransactions_WhenAuthenticated_ShouldSendQueryWithPagination()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<GetWalletTransactionsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<PagedResult<WalletTransactionDto>>.Success(
                new PagedResult<WalletTransactionDto>(new List<WalletTransactionDto>(), 0, 1, 20)));

        // Act
        var result = await _controller.GetTransactions(2, 10);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(
            Arg.Is<GetWalletTransactionsQuery>(q =>
                q.UserId == userId && q.PageNumber == 2 && q.PageSize == 10),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetDepositHistory Tests

    [Fact]
    public async Task GetDepositHistory_WhenUserNotAuthenticated_ShouldReturn401()
    {
        // Arrange
        _currentUserService.UserId.Returns((Guid?)null);

        // Act
        var result = await _controller.GetDepositHistory();

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetDepositHistory_WhenAuthenticated_ShouldSendQueryWithPagination()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<GetDepositHistoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<PagedResult<DepositRequestDto>>.Success(
                new PagedResult<DepositRequestDto>(new List<DepositRequestDto>(), 0, 1, 20)));

        // Act
        var result = await _controller.GetDepositHistory(3, 15);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(
            Arg.Is<GetDepositHistoryQuery>(q => q.UserId == userId && q.PageNumber == 3 && q.PageSize == 15),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetDepositRequest Tests

    [Fact]
    public async Task GetDepositRequest_WhenUserNotAuthenticated_ShouldReturn401()
    {
        // Arrange
        _currentUserService.UserId.Returns((Guid?)null);

        // Act
        var result = await _controller.GetDepositRequest(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetDepositRequest_WhenAuthenticatedAndOwner_ShouldReturnOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<GetDepositRequestQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<DepositRequestDto>.Success(new DepositRequestDto
            {
                Id = requestId,
                UserId = userId,
                WalletId = Guid.NewGuid(),
                Amount = 100_000m,
                PaymentMethod = Domain.Enums.PaymentMethod.PayOS,
                Status = Domain.Enums.PaymentStatus.Pending
            }));

        // Act
        var result = await _controller.GetDepositRequest(requestId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(
            Arg.Is<GetDepositRequestQuery>(q => q.Id == requestId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetDepositRequest_WhenAuthenticatedButNotOwner_ShouldReturn403()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<GetDepositRequestQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<DepositRequestDto>.Success(new DepositRequestDto
            {
                Id = requestId,
                UserId = Guid.NewGuid(),
                WalletId = Guid.NewGuid(),
                Amount = 100_000m,
                PaymentMethod = Domain.Enums.PaymentMethod.PayOS,
                Status = Domain.Enums.PaymentStatus.Pending
            }));

        // Act
        var result = await _controller.GetDepositRequest(requestId);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(403);
    }

    #endregion

    #region CreateDeposit Tests

    [Fact]
    public async Task CreateDeposit_WhenUserNotAuthenticated_ShouldReturn401()
    {
        // Arrange
        _currentUserService.UserId.Returns((Guid?)null);
        var request = new CreateDepositRequest { AmountVnd = 50000 };

        // Act
        var result = await _controller.CreateDeposit(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task CreateDeposit_WhenAuthenticated_ShouldSendCommandWithPayOSDefault()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<CreateDepositCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CreateDepositResponse>.Success(new CreateDepositResponse
            {
                DepositRequestId = Guid.NewGuid(),
                PaymentUrl = "https://pay.local",
                OrderCode = "ORDER-001",
                Amount = 100_000m,
                Status = "Pending"
            }));

        var request = new CreateDepositRequest
        {
            AmountVnd = 100_000m,
            Description = "Top up",
            ReturnUrl = "https://return.local",
            CancelUrl = "https://cancel.local"
        };

        // Act
        var result = await _controller.CreateDeposit(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(
            Arg.Is<CreateDepositCommand>(cmd =>
                cmd.UserId == userId &&
                cmd.AmountVnd == 100_000m &&
                cmd.PaymentMethod == Domain.Enums.PaymentMethod.PayOS &&
                cmd.Description == "Top up"),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region VerifyPayment Tests

    [Fact]
    public async Task VerifyPayment_WhenUserNotAuthenticated_ShouldReturn401()
    {
        // Arrange
        _currentUserService.UserId.Returns((Guid?)null);
        var request = new VerifyPaymentRequest { OrderCode = "123456" };

        // Act
        var result = await _controller.VerifyPayment(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task VerifyPayment_WhenAuthenticated_ShouldSendCommand()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<VerifyPaymentCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<VerifyPaymentResponse>.Success(new VerifyPaymentResponse
            {
                DepositRequestId = Guid.NewGuid(),
                OrderCode = "ORDER-123",
                Status = "Completed",
                Amount = 100_000m,
                IsSuccess = true
            }));

        var request = new VerifyPaymentRequest { OrderCode = "ORDER-123" };

        // Act
        var result = await _controller.VerifyPayment(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(
            Arg.Is<VerifyPaymentCommand>(cmd => cmd.OrderCode == "ORDER-123" && cmd.UserId == userId),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region PayOSWebhook Tests

    [Fact]
    public async Task PayOSWebhook_WhenNotSuccessful_ShouldReturnOk()
    {
        // Arrange
        var request = new PayOSWebhookRequest
        {
            Code = "00",
            Success = false,
            Data = null
        };

        // Act
        var result = await _controller.PayOSWebhook(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PayOSWebhook_WhenDataIsNull_ShouldReturnOk()
    {
        // Arrange
        var request = new PayOSWebhookRequest
        {
            Code = "00",
            Success = true,
            Data = null
        };

        // Act
        var result = await _controller.PayOSWebhook(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PayOSWebhook_WhenSuccessfulAndPaymentVerified_ShouldReturnOk()
    {
        // Arrange
        _mediator.Send(Arg.Any<VerifyPaymentCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<VerifyPaymentResponse>.Success(new VerifyPaymentResponse
            {
                DepositRequestId = Guid.NewGuid(),
                OrderCode = "123456",
                Status = "Completed",
                Amount = 100_000m,
                IsSuccess = true
            }));

        var request = new PayOSWebhookRequest
        {
            Code = "00",
            Success = true,
            Data = new PayOSWebhookData { OrderCode = 123456, Amount = 100000, Description = "Deposit" }
        };

        // Act
        var result = await _controller.PayOSWebhook(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(
            Arg.Is<VerifyPaymentCommand>(cmd => cmd.OrderCode == "123456"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetWithdrawalRequests_WhenAuthenticated_ShouldSendQuery()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<GetWithdrawalHistoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<PagedResult<WithdrawalRequestDto>>.Success(
                new PagedResult<WithdrawalRequestDto>(new List<WithdrawalRequestDto>(), 0, 1, 20)));

        // Act
        var result = await _controller.GetWithdrawalRequests(2, 25);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(
            Arg.Is<GetWithdrawalHistoryQuery>(q => q.UserId == userId && q.PageNumber == 2 && q.PageSize == 25),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateWithdrawalRequest_WhenAuthenticated_ShouldSendCommand()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<CreateWithdrawalRequestCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<WithdrawalRequestDto>.Success(new WithdrawalRequestDto
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                WalletId = Guid.NewGuid(),
                Amount = 100_000m,
                Status = Domain.Enums.PaymentStatus.Pending,
                BankName = "VCB",
                BankAccountNumber = "1234567890",
                AccountHolderName = "Nguyen Van A"
            }));

        var request = new CreateWithdrawalRequest
        {
            AmountVnd = 100_000m,
            BankName = "VCB",
            BankAccountNumber = "1234567890",
            AccountHolderName = "Nguyen Van A",
            ContractNumber = "CTR-001",
            Note = "Monthly payout"
        };

        // Act
        var result = await _controller.CreateWithdrawalRequest(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(
            Arg.Is<CreateWithdrawalRequestCommand>(cmd =>
                cmd.UserId == userId &&
                cmd.AmountVnd == 100_000m &&
                cmd.BankName == "VCB" &&
                cmd.ContractNumber == "CTR-001"),
            Arg.Any<CancellationToken>());
    }

    #endregion
}
