using Domain.Entities.Platform;
using Domain.Entities.Users;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Services;

public class AdminQueryServiceTests
{
    [Fact]
    public async Task GetOphthalmologistsAsync_WhenNoData_ShouldReturnEmptyPagedResult()
    {
        await using var context = CreateContext();
        var service = new AdminQueryService(context);

        var result = await service.GetOphthalmologistsAsync(null, null, 1, 10);

        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOphthalmologistsAsync_WithVerificationFilter_ShouldReturnOnlyMatchingStatus()
    {
        await using var context = CreateContext();

        var approvedUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "approved-doc@test.local",
            Email = "approved-doc@test.local",
            FullName = "Approved Doc"
        };
        var rejectedUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "rejected-doc@test.local",
            Email = "rejected-doc@test.local",
            FullName = "Rejected Doc"
        };
        await context.Users.AddRangeAsync(approvedUser, rejectedUser);

        var approved = new Ophthalmologist(approvedUser.Id, yearsOfExperience: 5);
        approved.Verify();
        var rejected = new Ophthalmologist(rejectedUser.Id, yearsOfExperience: 4);
        rejected.Reject("Missing credential");
        await context.Ophthalmologists.AddRangeAsync(approved, rejected);
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetOphthalmologistsAsync(null, "Approved", 1, 10);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle();
        result.Items[0].Email.Should().Be("approved-doc@test.local");
    }

    [Fact]
    public async Task GetPatientsAsync_WithActiveStatus_ShouldReturnOnlyActivePatients()
    {
        await using var context = CreateContext();

        var activeUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "active@test.local",
            Email = "active@test.local",
            FullName = "Active User",
            IsActive = true,
            EmailConfirmed = true
        };
        var pendingUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "pending@test.local",
            Email = "pending@test.local",
            FullName = "Pending User",
            IsActive = true,
            EmailConfirmed = false
        };

        await context.Users.AddRangeAsync(activeUser, pendingUser);
        await context.Patients.AddRangeAsync(
            new Patient(activeUser.Id),
            new Patient(pendingUser.Id));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetPatientsAsync(null, "active", 1, 10);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle();
        result.Items[0].Email.Should().Be("active@test.local");
    }

    [Fact]
    public async Task GetPatientsAsync_WithPendingStatus_ShouldReturnOnlyPendingPatients()
    {
        await using var context = CreateContext();

        var activeUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "active2@test.local",
            Email = "active2@test.local",
            FullName = "Active User 2",
            IsActive = true,
            EmailConfirmed = true
        };
        var pendingUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "pending2@test.local",
            Email = "pending2@test.local",
            FullName = "Pending User 2",
            IsActive = true,
            EmailConfirmed = false
        };

        await context.Users.AddRangeAsync(activeUser, pendingUser);
        await context.Patients.AddRangeAsync(new Patient(activeUser.Id), new Patient(pendingUser.Id));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetPatientsAsync(null, "pending", 1, 10);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(i => i.Email == "pending2@test.local");
    }

    [Fact]
    public async Task GetPatientsAsync_WithSuspendedStatus_ShouldReturnOnlyInactivePatients()
    {
        await using var context = CreateContext();
        var suspended = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "suspended@test.local",
            Email = "suspended@test.local",
            FullName = "Suspended User",
            IsActive = false,
            EmailConfirmed = true
        };
        var active = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "active3@test.local",
            Email = "active3@test.local",
            FullName = "Active User 3",
            IsActive = true,
            EmailConfirmed = true
        };
        await context.Users.AddRangeAsync(suspended, active);
        await context.Patients.AddRangeAsync(new Patient(suspended.Id), new Patient(active.Id));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetPatientsAsync(null, "suspended", 1, 10);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(i => i.Email == "suspended@test.local");
    }

    [Fact]
    public async Task GetAuditLogsAsync_WithActionFilter_ShouldReturnMatchingLogs()
    {
        await using var context = CreateContext();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "audit@test.local",
            Email = "audit@test.local",
            FullName = "Audit User"
        };
        await context.Users.AddAsync(user);
        await context.AuditLogs.AddRangeAsync(
            new AuditLog("Create", "Patient", "1", user.Id, null, "{name:'A'}", "127.0.0.1"),
            new AuditLog("Update", "Patient", "1", user.Id, "{name:'A'}", "{name:'B'}", "127.0.0.1"));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetAuditLogsAsync(null, "Create", null, null, null, null, 1, 10);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(i => i.Action == "Create");
    }

    [Fact]
    public async Task GetAuditLogsAsync_WithDateRange_ShouldReturnOnlyInRange()
    {
        await using var context = CreateContext();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "audit2@test.local",
            Email = "audit2@test.local",
            FullName = "Audit User 2"
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var oldLog = new AuditLog("Create", "EntityA", "1", user.Id);
        await context.AuditLogs.AddAsync(oldLog);
        await context.SaveChangesAsync();

        await Task.Delay(20);
        var newLog = new AuditLog("Update", "EntityA", "2", user.Id);
        await context.AuditLogs.AddAsync(newLog);
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var fromDate = oldLog.CreatedAt.AddMilliseconds(1);
        var result = await service.GetAuditLogsAsync(null, null, null, null, fromDate, null, 1, 10);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(i => i.Action == "Update");
    }

    [Fact]
    public async Task GetAuditLogsAsync_WithPaging_ShouldReturnExpectedPage()
    {
        await using var context = CreateContext();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "audit3@test.local",
            Email = "audit3@test.local",
            FullName = "Audit User 3"
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        await context.AuditLogs.AddRangeAsync(
            new AuditLog("A1", "Entity", "1", user.Id),
            new AuditLog("A2", "Entity", "2", user.Id),
            new AuditLog("A3", "Entity", "3", user.Id));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetAuditLogsAsync(null, null, null, null, null, null, 2, 1);

        result.TotalCount.Should().Be(3);
        result.Items.Should().HaveCount(1);
    }

    [Theory]
    [InlineData("active", 1)]
    [InlineData("pending", 1)]
    [InlineData("suspended", 1)]
    public async Task GetPatientsAsync_WithDifferentStatusFilters_ShouldReturnExpectedCount(string status, int expectedCount)
    {
        await using var context = CreateContext();
        var activeUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "mix-active@test.local",
            Email = "mix-active@test.local",
            FullName = "Mix Active",
            IsActive = true,
            EmailConfirmed = true
        };
        var pendingUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "mix-pending@test.local",
            Email = "mix-pending@test.local",
            FullName = "Mix Pending",
            IsActive = true,
            EmailConfirmed = false
        };
        var suspendedUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "mix-suspended@test.local",
            Email = "mix-suspended@test.local",
            FullName = "Mix Suspended",
            IsActive = false,
            EmailConfirmed = true
        };
        await context.Users.AddRangeAsync(activeUser, pendingUser, suspendedUser);
        await context.Patients.AddRangeAsync(
            new Patient(activeUser.Id),
            new Patient(pendingUser.Id),
            new Patient(suspendedUser.Id));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetPatientsAsync(null, status, 1, 20);

        result.TotalCount.Should().Be(expectedCount);
        result.Items.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task GetPatientsAsync_WithUnknownStatus_ShouldReturnAllPatients()
    {
        await using var context = CreateContext();
        var u1 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "all-1@test.local", Email = "all-1@test.local", FullName = "All One", IsActive = true, EmailConfirmed = true };
        var u2 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "all-2@test.local", Email = "all-2@test.local", FullName = "All Two", IsActive = false, EmailConfirmed = false };
        await context.Users.AddRangeAsync(u1, u2);
        await context.Patients.AddRangeAsync(new Patient(u1.Id), new Patient(u2.Id));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetPatientsAsync(null, "unknown-status", 1, 20);

        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPatientsAsync_ShouldExcludeDeletedUsers()
    {
        await using var context = CreateContext();
        var alive = new ApplicationUser { Id = Guid.NewGuid(), UserName = "alive@test.local", Email = "alive@test.local", FullName = "Alive", IsDeleted = false };
        var deleted = new ApplicationUser { Id = Guid.NewGuid(), UserName = "deleted@test.local", Email = "deleted@test.local", FullName = "Deleted", IsDeleted = true };
        await context.Users.AddRangeAsync(alive, deleted);
        await context.Patients.AddRangeAsync(new Patient(alive.Id), new Patient(deleted.Id));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetPatientsAsync(null, null, 1, 20);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(i => i.Email == "alive@test.local");
    }

    [Fact]
    public async Task GetOphthalmologistsAsync_WithMultipleVerificationStatuses_ShouldReturnUnion()
    {
        await using var context = CreateContext();
        var uPending = new ApplicationUser { Id = Guid.NewGuid(), UserName = "doc-p@test.local", Email = "doc-p@test.local", FullName = "Doc Pending" };
        var uApproved = new ApplicationUser { Id = Guid.NewGuid(), UserName = "doc-a@test.local", Email = "doc-a@test.local", FullName = "Doc Approved" };
        var uRejected = new ApplicationUser { Id = Guid.NewGuid(), UserName = "doc-r@test.local", Email = "doc-r@test.local", FullName = "Doc Rejected" };
        await context.Users.AddRangeAsync(uPending, uApproved, uRejected);

        var pending = new Ophthalmologist(uPending.Id, yearsOfExperience: 2);
        var approved = new Ophthalmologist(uApproved.Id, yearsOfExperience: 3);
        approved.Verify();
        var rejected = new Ophthalmologist(uRejected.Id, yearsOfExperience: 4);
        rejected.Reject("missing docs");
        await context.Ophthalmologists.AddRangeAsync(pending, approved, rejected);
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetOphthalmologistsAsync(null, "Approved,Rejected", 1, 20);

        result.TotalCount.Should().Be(2);
        result.Items.Should().OnlyContain(i => i.VerificationStatus == "Approved" || i.VerificationStatus == "Rejected");
    }

    [Fact]
    public async Task GetAuditLogsAsync_WithUserFilter_ShouldReturnOnlyThatUserLogs()
    {
        await using var context = CreateContext();
        var u1 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "log-u1@test.local", Email = "log-u1@test.local", FullName = "Log U1" };
        var u2 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "log-u2@test.local", Email = "log-u2@test.local", FullName = "Log U2" };
        await context.Users.AddRangeAsync(u1, u2);
        await context.AuditLogs.AddRangeAsync(
            new AuditLog("Create", "Patient", "1", u1.Id),
            new AuditLog("Update", "Patient", "2", u2.Id),
            new AuditLog("Delete", "Patient", "3", u1.Id));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetAuditLogsAsync(null, null, null, u1.Id, null, null, 1, 20);

        result.TotalCount.Should().Be(2);
        result.Items.Should().OnlyContain(i => i.UserId == u1.Id);
    }

    [Fact]
    public async Task GetAuditLogsAsync_WithToDate_ShouldExcludeNewerLogs()
    {
        await using var context = CreateContext();
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "todate@test.local", Email = "todate@test.local", FullName = "To Date" };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var oldLog = new AuditLog("A1", "Entity", "1", user.Id);
        await context.AuditLogs.AddAsync(oldLog);
        await context.SaveChangesAsync();
        await Task.Delay(20);
        var newLog = new AuditLog("A2", "Entity", "2", user.Id);
        await context.AuditLogs.AddAsync(newLog);
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetAuditLogsAsync(null, null, null, null, null, oldLog.CreatedAt.AddMilliseconds(1), 1, 20);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(i => i.Action == "A1");
    }

    [Fact]
    public async Task GetAuditLogsAsync_WithEntityFilter_ShouldReturnOnlyMatchingEntity()
    {
        await using var context = CreateContext();
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "entity@test.local", Email = "entity@test.local", FullName = "Entity User" };
        await context.Users.AddAsync(user);
        await context.AuditLogs.AddRangeAsync(
            new AuditLog("Create", "Patient", "1", user.Id),
            new AuditLog("Create", "Appointment", "2", user.Id));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetAuditLogsAsync(null, null, "Patient", null, null, null, 1, 20);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(i => i.EntityName == "Patient");
    }

    [Fact]
    public async Task GetOphthalmologistsAsync_WithPaging_ShouldReturnRequestedSlice()
    {
        await using var context = CreateContext();
        var u1 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "doc-page-1@test.local", Email = "doc-page-1@test.local", FullName = "Doc Page 1" };
        var u2 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "doc-page-2@test.local", Email = "doc-page-2@test.local", FullName = "Doc Page 2" };
        var u3 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "doc-page-3@test.local", Email = "doc-page-3@test.local", FullName = "Doc Page 3" };
        await context.Users.AddRangeAsync(u1, u2, u3);
        await context.Ophthalmologists.AddRangeAsync(
            new Ophthalmologist(u1.Id, yearsOfExperience: 2),
            new Ophthalmologist(u2.Id, yearsOfExperience: 3),
            new Ophthalmologist(u3.Id, yearsOfExperience: 4));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetOphthalmologistsAsync(null, null, 2, 1);

        result.TotalCount.Should().Be(3);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAuditLogsAsync_WithUnknownUser_ShouldMapUserNameAsNull()
    {
        await using var context = CreateContext();
        await context.AuditLogs.AddAsync(new AuditLog("Create", "Patient", "x1", Guid.NewGuid()));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetAuditLogsAsync(null, null, null, null, null, null, 1, 20);

        result.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(i => i.UserName == null);
    }

    [Theory]
    [InlineData("active", 1)]
    [InlineData("ACTIVE", 1)]
    [InlineData("Active", 1)]
    [InlineData("pending", 1)]
    [InlineData("PENDING", 1)]
    [InlineData("Pending", 1)]
    [InlineData("suspended", 1)]
    [InlineData("SUSPENDED", 1)]
    [InlineData("Suspended", 1)]
    [InlineData("unknown", 3)]
    public async Task GetPatientsAsync_StatusFilter_ShouldBeCaseInsensitiveAndFallbackToAll(string status, int expectedCount)
    {
        await using var context = CreateContext();
        var activeUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "status-active@test.local",
            Email = "status-active@test.local",
            FullName = "Status Active",
            IsActive = true,
            EmailConfirmed = true
        };
        var pendingUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "status-pending@test.local",
            Email = "status-pending@test.local",
            FullName = "Status Pending",
            IsActive = true,
            EmailConfirmed = false
        };
        var suspendedUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "status-suspended@test.local",
            Email = "status-suspended@test.local",
            FullName = "Status Suspended",
            IsActive = false,
            EmailConfirmed = true
        };
        await context.Users.AddRangeAsync(activeUser, pendingUser, suspendedUser);
        await context.Patients.AddRangeAsync(
            new Patient(activeUser.Id),
            new Patient(pendingUser.Id),
            new Patient(suspendedUser.Id));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetPatientsAsync(null, status, 1, 20);

        result.TotalCount.Should().Be(expectedCount);
        result.Items.Should().HaveCount(expectedCount);
    }

    [Theory]
    [InlineData("Approved", 1)]
    [InlineData("approved", 1)]
    [InlineData("APPROVED", 1)]
    [InlineData("Rejected", 1)]
    [InlineData("rejected", 1)]
    [InlineData("REJECTED", 1)]
    [InlineData("PendingVerification", 1)]
    [InlineData("pendingverification", 1)]
    [InlineData("Approved,Rejected", 2)]
    [InlineData("Approved,Rejected,PendingVerification", 3)]
    public async Task GetOphthalmologistsAsync_VerificationFilter_ShouldHandleCaseAndMultiValues(string filter, int expectedCount)
    {
        await using var context = CreateContext();
        var uPending = new ApplicationUser { Id = Guid.NewGuid(), UserName = "vf-p@test.local", Email = "vf-p@test.local", FullName = "VF Pending" };
        var uApproved = new ApplicationUser { Id = Guid.NewGuid(), UserName = "vf-a@test.local", Email = "vf-a@test.local", FullName = "VF Approved" };
        var uRejected = new ApplicationUser { Id = Guid.NewGuid(), UserName = "vf-r@test.local", Email = "vf-r@test.local", FullName = "VF Rejected" };
        await context.Users.AddRangeAsync(uPending, uApproved, uRejected);

        var pending = new Ophthalmologist(uPending.Id, yearsOfExperience: 2);
        var approved = new Ophthalmologist(uApproved.Id, yearsOfExperience: 3);
        approved.Verify();
        var rejected = new Ophthalmologist(uRejected.Id, yearsOfExperience: 4);
        rejected.Reject("missing docs");
        await context.Ophthalmologists.AddRangeAsync(pending, approved, rejected);
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetOphthalmologistsAsync(null, filter, 1, 20);

        result.TotalCount.Should().Be(expectedCount);
        result.Items.Should().HaveCount(expectedCount);
    }

    [Theory]
    [InlineData(null, 3)]
    [InlineData("", 3)]
    [InlineData(" ", 3)]
    [InlineData("UnknownStatus", 3)]
    [InlineData("Unknown1,Unknown2", 3)]
    [InlineData("Approved,Unknown", 1)]
    [InlineData("Rejected,Unknown", 1)]
    [InlineData("PendingVerification,Unknown", 1)]
    [InlineData("Approved,Approved", 1)]
    [InlineData("Approved,Rejected,Unknown", 2)]
    public async Task GetOphthalmologistsAsync_VerificationFilter_WithInvalidOrMixedValues_ShouldBehaveAsExpected(
        string? filter,
        int expectedCount)
    {
        await using var context = CreateContext();
        var uPending = new ApplicationUser { Id = Guid.NewGuid(), UserName = "mix-vf-p@test.local", Email = "mix-vf-p@test.local", FullName = "Mix VF Pending" };
        var uApproved = new ApplicationUser { Id = Guid.NewGuid(), UserName = "mix-vf-a@test.local", Email = "mix-vf-a@test.local", FullName = "Mix VF Approved" };
        var uRejected = new ApplicationUser { Id = Guid.NewGuid(), UserName = "mix-vf-r@test.local", Email = "mix-vf-r@test.local", FullName = "Mix VF Rejected" };
        await context.Users.AddRangeAsync(uPending, uApproved, uRejected);

        var pending = new Ophthalmologist(uPending.Id, yearsOfExperience: 2);
        var approved = new Ophthalmologist(uApproved.Id, yearsOfExperience: 3);
        approved.Verify();
        var rejected = new Ophthalmologist(uRejected.Id, yearsOfExperience: 4);
        rejected.Reject("docs");
        await context.Ophthalmologists.AddRangeAsync(pending, approved, rejected);
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetOphthalmologistsAsync(null, filter, 1, 20);

        result.TotalCount.Should().Be(expectedCount);
        result.Items.Should().HaveCount(expectedCount);
    }

    [Theory]
    [InlineData(null, 3)]
    [InlineData("", 3)]
    [InlineData(" ", 3)]
    [InlineData("active", 1)]
    [InlineData("pending", 1)]
    [InlineData("suspended", 1)]
    [InlineData("ACTIVE", 1)]
    [InlineData("PENDING", 1)]
    [InlineData("SUSPENDED", 1)]
    [InlineData(" active ", 3)]
    public async Task GetPatientsAsync_StatusFilter_WithWhitespaceAndNull_ShouldMatchServiceBehavior(string? status, int expectedCount)
    {
        await using var context = CreateContext();
        var activeUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "ws-active@test.local",
            Email = "ws-active@test.local",
            FullName = "WS Active",
            IsActive = true,
            EmailConfirmed = true
        };
        var pendingUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "ws-pending@test.local",
            Email = "ws-pending@test.local",
            FullName = "WS Pending",
            IsActive = true,
            EmailConfirmed = false
        };
        var suspendedUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "ws-suspended@test.local",
            Email = "ws-suspended@test.local",
            FullName = "WS Suspended",
            IsActive = false,
            EmailConfirmed = true
        };
        await context.Users.AddRangeAsync(activeUser, pendingUser, suspendedUser);
        await context.Patients.AddRangeAsync(
            new Patient(activeUser.Id),
            new Patient(pendingUser.Id),
            new Patient(suspendedUser.Id));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetPatientsAsync(null, status, 1, 20);

        result.TotalCount.Should().Be(expectedCount);
        result.Items.Should().HaveCount(expectedCount);
    }

    [Theory]
    [InlineData("Create", 1)]
    [InlineData("Update", 1)]
    [InlineData("Delete", 1)]
    [InlineData("create", 0)]
    [InlineData("UPDATE", 0)]
    [InlineData("Unknown", 0)]
    [InlineData("Create ", 0)]
    [InlineData(" Update", 0)]
    [InlineData("", 3)]
    [InlineData(" ", 3)]
    public async Task GetAuditLogsAsync_ActionFilter_ShouldMatchExactActionString(string actionFilter, int expectedCount)
    {
        await using var context = CreateContext();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "action-filter@test.local",
            Email = "action-filter@test.local",
            FullName = "Action Filter"
        };
        await context.Users.AddAsync(user);
        await context.AuditLogs.AddRangeAsync(
            new AuditLog("Create", "Patient", "1", user.Id),
            new AuditLog("Update", "Patient", "2", user.Id),
            new AuditLog("Delete", "Patient", "3", user.Id));
        await context.SaveChangesAsync();

        var service = new AdminQueryService(context);
        var result = await service.GetAuditLogsAsync(null, actionFilter, null, null, null, null, 1, 20);

        result.TotalCount.Should().Be(expectedCount);
        result.Items.Should().HaveCount(expectedCount);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
}
