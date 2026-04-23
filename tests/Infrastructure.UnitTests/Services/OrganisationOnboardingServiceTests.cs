using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models.Auth;
using Domain.Entities.Contracts;
using Domain.Entities.Users;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.Settings;
using Infrastructure.UnitTests.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Infrastructure.UnitTests.Services;

public class OrganisationOnboardingServiceTests
{
    [Fact]
    public async Task SubmitRequestAsync_ShouldPersistPendingRequest_AndNotifyConfiguredAdminEmail()
    {
        await using var context = CreateContext();
        var email = new FakeEmailService();
        var service = CreateService(context, email, "ops-admin@test.local");

        var result = await service.SubmitRequestAsync(CreateRequest("org1@test.local"));

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        var request = await context.OrganisationOnboardingRequests.FirstAsync();
        request.Status.Should().Be(OrganisationOnboardingStatus.Pending);
        request.ContactEmail.Should().Be("org1@test.local");
        email.Sent.Should().ContainSingle(x => x.To == "ops-admin@test.local");
    }

    [Fact]
    public async Task SubmitRequestAsync_WhenPendingAlreadyExists_ShouldReturnConflict()
    {
        await using var context = CreateContext();
        var email = new FakeEmailService();
        var service = CreateService(context, email);
        await service.SubmitRequestAsync(CreateRequest("dup@test.local"));

        var result = await service.SubmitRequestAsync(CreateRequest("dup@test.local"));

        result.IsConflict.Should().BeTrue();
        (await context.OrganisationOnboardingRequests.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task SubmitRequestAsync_WhenNoConfiguredAdminEmail_ShouldNotifyDistinctSystemAdmins()
    {
        await using var context = CreateContext();
        var email = new FakeEmailService();
        var userManager = CreateUserManager(context);
        await EnsureRoleAsync(context, Roles.SystemAdmin);

        var adminA = new ApplicationUser { Id = Guid.NewGuid(), UserName = "a@test.local", Email = "same@test.local", FullName = "A" };
        var adminB = new ApplicationUser { Id = Guid.NewGuid(), UserName = "b@test.local", Email = "SAME@test.local", FullName = "B" };
        await userManager.CreateAsync(adminA, "Admin@12345");
        await userManager.CreateAsync(adminB, "Admin@12345");
        var role = await context.Roles.FirstAsync(r => r.Name == Roles.SystemAdmin);
        await context.UserRoles.AddRangeAsync(
            new ApplicationUserRole { UserId = adminA.Id, RoleId = role.Id },
            new ApplicationUserRole { UserId = adminB.Id, RoleId = role.Id });
        await context.SaveChangesAsync();

        var service = CreateService(context, email, null, userManager);
        await service.SubmitRequestAsync(CreateRequest("notify@test.local"));

        email.Sent.Should().ContainSingle(x => x.To.Equals("same@test.local", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SubmitRequestAsync_WhenNoConfiguredEmailAndNoSystemAdmins_ShouldStillSucceed()
    {
        await using var context = CreateContext();
        var email = new FakeEmailService();
        var service = CreateService(context, email, null);

        var result = await service.SubmitRequestAsync(CreateRequest("no-admins@test.local"));

        result.IsSuccess.Should().BeTrue();
        email.Sent.Should().BeEmpty();
    }

    [Fact]
    public async Task SubmitRequestAsync_ShouldNormalizeTrimmedInputFields()
    {
        await using var context = CreateContext();
        var email = new FakeEmailService();
        var service = CreateService(context, email);

        var result = await service.SubmitRequestAsync(new RegisterOrganisationRequest
        {
            ContactEmail = " trim@test.local ",
            ContactFullName = "  Trim Contact  ",
            OrganisationName = "  Trim Org  ",
            OrgType = (int)OrgType.Clinic,
            Address = "  Address  ",
            ContactPhone = "  0909  ",
            LicenseNumber = "  LIC-TRIM  ",
            Notes = "  note  "
        });

        result.IsSuccess.Should().BeTrue();
        var saved = await context.OrganisationOnboardingRequests.FirstAsync();
        saved.ContactEmail.Should().Be("trim@test.local");
        saved.ContactFullName.Should().Be("Trim Contact");
        saved.OrganisationName.Should().Be("Trim Org");
        saved.Address.Should().Be("Address");
    }

    [Fact]
    public async Task SubmitRequestAsync_WhenExistingRequestIsApproved_ShouldAllowNewPendingRequest()
    {
        await using var context = CreateContext();
        var approved = new OrganisationOnboardingRequest("Old Org", OrgType.Clinic, "Old Contact", "history@test.local");
        approved.Approve(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        await context.OrganisationOnboardingRequests.AddAsync(approved);
        await context.SaveChangesAsync();

        var service = CreateService(context, new FakeEmailService());
        var result = await service.SubmitRequestAsync(CreateRequest("history@test.local"));

        result.IsSuccess.Should().BeTrue();
        (await context.OrganisationOnboardingRequests.CountAsync()).Should().Be(2);
        (await context.OrganisationOnboardingRequests.CountAsync(x => x.Status == OrganisationOnboardingStatus.Pending)).Should().Be(1);
    }

    [Fact]
    public async Task GetRequestsAsync_ShouldReturnAllRequests()
    {
        await using var context = CreateContext();
        var service = CreateService(context, new FakeEmailService());
        await service.SubmitRequestAsync(CreateRequest("r1@test.local"));
        await service.SubmitRequestAsync(CreateRequest("r2@test.local"));

        var result = await service.GetRequestsAsync();

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(2);
    }

    [Fact]
    public async Task ApproveRequestAsync_WhenRequestNotFound_ShouldReturnNotFound()
    {
        await using var context = CreateContext();
        var service = CreateService(context, new FakeEmailService());

        var result = await service.ApproveRequestAsync(Guid.NewGuid(), Guid.NewGuid());

        result.IsNotFound.Should().BeTrue();
    }

    [Fact]
    public async Task ApproveRequestAsync_WhenRequestAlreadyProcessed_ShouldReturnFailure()
    {
        await using var context = CreateContext();
        var approvedBy = Guid.NewGuid();
        var request = new OrganisationOnboardingRequest("Org A", OrgType.Clinic, "Contact", "done@test.local");
        request.Approve(approvedBy, Guid.NewGuid(), Guid.NewGuid());
        await context.OrganisationOnboardingRequests.AddAsync(request);
        await context.SaveChangesAsync();
        var service = CreateService(context, new FakeEmailService());

        var result = await service.ApproveRequestAsync(request.Id, approvedBy);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("already been processed"));
    }

    [Fact]
    public async Task ApproveRequestAsync_WhenUserWithContactEmailExists_ShouldReturnConflict()
    {
        await using var context = CreateContext();
        var request = new OrganisationOnboardingRequest("Org B", OrgType.Hospital, "Contact", "exists@test.local");
        await context.OrganisationOnboardingRequests.AddAsync(request);
        var userManager = CreateUserManager(context);
        await userManager.CreateAsync(new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "exists@test.local",
            Email = "exists@test.local",
            FullName = "Existing User"
        }, "Existing@12345");
        await context.SaveChangesAsync();

        var service = CreateService(context, new FakeEmailService(), userManager: userManager);
        var result = await service.ApproveRequestAsync(request.Id, Guid.NewGuid());

        result.IsConflict.Should().BeTrue();
    }

    [Fact]
    public async Task ApproveRequestAsync_ShouldCreateOrganisationAdmin_AndApproveRequest()
    {
        await using var context = CreateContext();
        await EnsureRoleAsync(context, Roles.SystemAdmin);
        var request = new OrganisationOnboardingRequest("Org C", OrgType.Hospital, "Owner C", "ownerc@test.local", "0909", "Addr", "LIC-1");
        await context.OrganisationOnboardingRequests.AddAsync(request);
        await context.ContractTemplates.AddAsync(new ContractTemplate(
            "Medical Org Contract",
            ContractType.MedicalOrganizationContract,
            "v1",
            "<html>template</html>"));
        await context.SaveChangesAsync();

        var email = new FakeEmailService();
        var service = CreateService(context, email);
        var result = await service.ApproveRequestAsync(request.Id, Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        (await context.Organisations.CountAsync()).Should().Be(1);
        (await context.Contracts.CountAsync()).Should().Be(1);
        var refreshed = await context.OrganisationOnboardingRequests.FirstAsync();
        refreshed.Status.Should().Be(OrganisationOnboardingStatus.Approved);
        email.Sent.Should().Contain(x => x.To == "ownerc@test.local");
    }

    [Fact]
    public async Task ApproveRequestAsync_WhenNoActiveTemplate_ShouldStillSucceedWithoutContract()
    {
        await using var context = CreateContext();
        await EnsureRoleAsync(context, Roles.SystemAdmin);
        var request = new OrganisationOnboardingRequest("Org D", OrgType.Clinic, "Owner D", "ownerd@test.local");
        await context.OrganisationOnboardingRequests.AddAsync(request);
        var inactiveTemplate = new ContractTemplate("Inactive", ContractType.MedicalOrganizationContract, "v1", "<html/>");
        inactiveTemplate.Deactivate();
        await context.ContractTemplates.AddAsync(inactiveTemplate);
        await context.SaveChangesAsync();

        var service = CreateService(context, new FakeEmailService());
        var result = await service.ApproveRequestAsync(request.Id, Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        (await context.Contracts.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task ApproveRequestAsync_ShouldAssignOrgAdminRoleToCreatedUser()
    {
        await using var context = CreateContext();
        await EnsureRoleAsync(context, Roles.SystemAdmin);
        var request = new OrganisationOnboardingRequest("Org Role", OrgType.Clinic, "Role Owner", "role-owner@test.local");
        await context.OrganisationOnboardingRequests.AddAsync(request);
        await context.SaveChangesAsync();

        var service = CreateService(context, new FakeEmailService());
        var result = await service.ApproveRequestAsync(request.Id, Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        var createdUserId = result.Data!.OrgAdminUserId;
        var role = await context.Roles.FirstAsync(r => r.Name == Roles.SystemAdmin);
        var hasRole = await context.UserRoles.AnyAsync(ur => ur.UserId == createdUserId && ur.RoleId == role.Id);
        hasRole.Should().BeTrue();
    }

    [Fact]
    public async Task ApproveRequestAsync_WhenEmailSendingFails_ShouldReturnFailure()
    {
        await using var context = CreateContext();
        await EnsureRoleAsync(context, Roles.SystemAdmin);
        var request = new OrganisationOnboardingRequest("Org E", OrgType.Clinic, "Owner E", "ownere@test.local");
        await context.OrganisationOnboardingRequests.AddAsync(request);
        await context.SaveChangesAsync();
        var service = CreateService(context, new ThrowingEmailService());

        var result = await service.ApproveRequestAsync(request.Id, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Failed to approve"));
    }

    private static OrganisationOnboardingService CreateService(
        ApplicationDbContext context,
        IEmailService emailService,
        string? adminEmail = "admin@test.local",
        UserManager<ApplicationUser>? userManager = null)
    {
        userManager ??= CreateUserManager(context);
        return new OrganisationOnboardingService(
            new Repository<OrganisationOnboardingRequest>(context),
            new Repository<Organisation>(context),
            new Repository<ContractTemplate>(context),
            new ContractRepository(context),
            emailService,
            new FakeNotificationService(),
            context,
            userManager,
            Options.Create(new AdminNotificationSettings { OrganisationOnboardingEmail = adminEmail }),
            new TestLogger<OrganisationOnboardingService>());
    }

    private static RegisterOrganisationRequest CreateRequest(string email) => new()
    {
        ContactEmail = email,
        ContactFullName = "Contact Person",
        OrganisationName = "Aura Clinic",
        OrgType = (int)OrgType.Clinic,
        Address = "Address A",
        ContactPhone = "0123",
        LicenseNumber = "L-001",
        Notes = "note"
    };

    private static UserManager<ApplicationUser> CreateUserManager(ApplicationDbContext context)
    {
        var store = new UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken, ApplicationRoleClaim>(context);
        var options = Options.Create(new IdentityOptions());
        return new UserManager<ApplicationUser>(
            store,
            options,
            new PasswordHasher<ApplicationUser>(),
            [new UserValidator<ApplicationUser>()],
            [new PasswordValidator<ApplicationUser>()],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null,
            NullLogger<UserManager<ApplicationUser>>.Instance);
    }

    private static async Task EnsureRoleAsync(ApplicationDbContext context, string roleName)
    {
        if (await context.Roles.AnyAsync(r => r.Name == roleName))
        {
            return;
        }

        await context.Roles.AddAsync(new ApplicationRole { Name = roleName, NormalizedName = roleName.ToUpperInvariant() });
        await context.SaveChangesAsync();
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new ApplicationDbContext(options);
    }

    private sealed class FakeEmailService : IEmailService
    {
        public List<(string To, string Subject, string Body, bool IsHtml)> Sent { get; } = [];
        public Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendPasswordResetAsync(string email, string resetLink, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendWelcomeEmailAsync(string email, string fullName, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
        {
            Sent.Add((to, subject, body, isHtml));
            return Task.CompletedTask;
        }
        public Task SendClinicAppointmentConfirmationAsync(string email, ClinicAppointmentConfirmationEmailPayload payload, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendOrganisationScreeningResultShareAsync(string email, OrganisationScreeningResultShareEmailPayload payload, IReadOnlyCollection<EmailAttachment> attachments, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendWithAttachmentsAsync(string to, string subject, string body, IReadOnlyCollection<EmailAttachment> attachments, bool isHtml = true, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class ThrowingEmailService : IEmailService
    {
        public Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendPasswordResetAsync(string email, string resetLink, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendWelcomeEmailAsync(string email, string fullName, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("smtp-down");
        public Task SendClinicAppointmentConfirmationAsync(string email, ClinicAppointmentConfirmationEmailPayload payload, CancellationToken cancellationToken = default) => throw new InvalidOperationException("smtp-down");
        public Task SendOrganisationScreeningResultShareAsync(string email, OrganisationScreeningResultShareEmailPayload payload, IReadOnlyCollection<EmailAttachment> attachments, CancellationToken cancellationToken = default) => throw new InvalidOperationException("smtp-down");
        public Task SendWithAttachmentsAsync(string to, string subject, string body, IReadOnlyCollection<EmailAttachment> attachments, bool isHtml = true, CancellationToken cancellationToken = default) => throw new InvalidOperationException("smtp-down");
    }

    private sealed class FakeNotificationService : INotificationService
    {
        public Task SendAsync(Guid userId, string title, string message, NotificationType type, object? payload = null, CancellationToken cancellationToken = default, Guid? referenceId = null) => Task.CompletedTask;
        public Task SendToRoleAsync(string roleName, string title, string message, NotificationType type = NotificationType.SystemAlert, object? payload = null, CancellationToken cancellationToken = default, Guid? referenceId = null) => Task.CompletedTask;
        public Task SendAsync(Guid userId, string message, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
