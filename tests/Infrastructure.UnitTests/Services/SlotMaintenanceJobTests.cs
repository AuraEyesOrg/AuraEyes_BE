using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Infrastructure.UnitTests.Services;

public class SlotMaintenanceJobTests
{
    private readonly Mock<IBetterStackHeartbeatService> _heartbeatMock;
    private readonly Mock<ILogger<SlotMaintenanceJob>> _loggerMock;

    public SlotMaintenanceJobTests()
    {
        _heartbeatMock = new Mock<IBetterStackHeartbeatService>();
        _loggerMock = new Mock<ILogger<SlotMaintenanceJob>>();
    }

    [Fact]
    public async Task ExpireUnusedSlotsAsync_ShouldInvokeHeartbeat()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);
        
        var job = new SlotMaintenanceJob(context, _heartbeatMock.Object, _loggerMock.Object);

        try { await job.ExpireUnusedSlotsAsync(); } catch { }

        _heartbeatMock.Verify(x => x.NotifyStartedAsync(BetterStackMonitor.SlotMaintenance, It.IsAny<CancellationToken>()), Times.Once);
    }
}
