using Application.Common.Interfaces;
using Application.Common.Models;
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

    #endregion
}
