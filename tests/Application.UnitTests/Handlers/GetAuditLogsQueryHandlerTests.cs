using Application.Common.Models;
using Application.SystemAdmin.AuditLogs.Queries.GetAuditLogs;
using Application.SystemAdmin.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Handlers;

public class GetAuditLogsQueryHandlerTests
{
    private readonly IAdminQueryService _adminQueryService;
    private readonly GetAuditLogsQueryHandler _handler;

    public GetAuditLogsQueryHandlerTests()
    {
        _adminQueryService = Substitute.For<IAdminQueryService>();
        _handler = new GetAuditLogsQueryHandler(_adminQueryService);
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllLogs()
    {
        // Arrange
        var logs = new List<AuditLogDto>
        {
            new() { Id = Guid.NewGuid(), Action = "Insert", EntityName = "Patient", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Action = "Update", EntityName = "Wallet", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Action = "Insert", EntityName = "WalletTransaction", CreatedAt = DateTime.UtcNow }
        };
        var pagedResult = new PagedResult<AuditLogDto>(logs, 3, 1, 20);

        _adminQueryService.GetAuditLogsAsync(
            Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(),
            Arg.Any<Guid?>(), Arg.Any<DateTime?>(), Arg.Any<DateTime?>(),
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var query = new GetAuditLogsQuery { PageNumber = 1, PageSize = 20 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(3);
        result.Data.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Handle_FilterByEntityName_ReturnsFilteredLogs()
    {
        // Arrange
        var filteredLogs = new List<AuditLogDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Action = "Insert",
                EntityName = "WalletTransaction",
                OldValue = null,
                NewValue = "{\"amount\":50000,\"type\":\"Deposit\"}",
                CreatedAt = DateTime.UtcNow
            }
        };
        var pagedResult = new PagedResult<AuditLogDto>(filteredLogs, 1, 1, 20);

        _adminQueryService.GetAuditLogsAsync(
            null, null, "WalletTransaction",
            null, null, null,
            1, 20, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var query = new GetAuditLogsQuery
        {
            EntityName = "WalletTransaction",
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().EntityName.Should().Be("WalletTransaction");
        result.Data.TotalCount.Should().Be(1);

        await _adminQueryService.Received(1).GetAuditLogsAsync(
            null, null, "WalletTransaction",
            null, null, null,
            1, 20, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FilterByAction_ReturnsOnlyMatchingActions()
    {
        // Arrange
        var filteredLogs = new List<AuditLogDto>
        {
            new() { Id = Guid.NewGuid(), Action = "Update", EntityName = "Patient", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Action = "Update", EntityName = "Ophthalmologist", CreatedAt = DateTime.UtcNow }
        };
        var pagedResult = new PagedResult<AuditLogDto>(filteredLogs, 2, 1, 20);

        _adminQueryService.GetAuditLogsAsync(
            null, "Update", null,
            null, null, null,
            1, 20, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var query = new GetAuditLogsQuery
        {
            Action = "Update",
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.Items.Should().AllSatisfy(x => x.Action.Should().Be("Update"));
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPagedResult()
    {
        // Arrange
        var pagedResult = new PagedResult<AuditLogDto>(new List<AuditLogDto>(), 0, 1, 20);

        _adminQueryService.GetAuditLogsAsync(
            Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(),
            Arg.Any<Guid?>(), Arg.Any<DateTime?>(), Arg.Any<DateTime?>(),
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var query = new GetAuditLogsQuery
        {
            EntityName = "NonExistentEntity",
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_PaginationParameters_PassedCorrectly()
    {
        // Arrange
        var pagedResult = new PagedResult<AuditLogDto>(new List<AuditLogDto>(), 100, 3, 10);

        _adminQueryService.GetAuditLogsAsync(
            Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(),
            Arg.Any<Guid?>(), Arg.Any<DateTime?>(), Arg.Any<DateTime?>(),
            3, 10, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var query = new GetAuditLogsQuery { PageNumber = 3, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.PageNumber.Should().Be(3);
        result.Data.PageSize.Should().Be(10);
        result.Data.TotalCount.Should().Be(100);
        result.Data.TotalPages.Should().Be(10);
    }
}
