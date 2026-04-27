using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Identity;

public class IdentityServiceTests
{
    [Fact]
    public async Task GetUserByEmailAsync_WhenExists_ShouldReturnDto()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        var email = "exists@test.local";
        await service.CreateUserAsync(email, "Pass@123", "User Name");

        var result = await service.GetUserByEmailAsync(email);

        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenMissing_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.GetUserByEmailAsync("missing@test.local");

        result.Should().BeNull();
    }

    [Fact]
    public async Task IsEmailConfirmedAsync_WhenConfirmed_ShouldReturnTrue()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        var email = "confirmed@test.local";
        await service.CreateUserAsync(email, "Pass@123", "User Name");
        var user = await context.Users.FirstAsync(u => u.Email == email);
        user.EmailConfirmed = true;
        await context.SaveChangesAsync();

        var result = await service.IsEmailConfirmedAsync(user.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserRolesAsync_WithMultipleRoles_ShouldReturnAll()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        var email = "multi@test.local";
        
        // Setup roles in DB
        await context.Roles.AddAsync(new ApplicationRole { Name = "Role1", NormalizedName = "ROLE1" });
        await context.Roles.AddAsync(new ApplicationRole { Name = "Role2", NormalizedName = "ROLE2" });
        await context.SaveChangesAsync();

        await service.CreateUserAsync(email, "Pass@123", "User Name");
        var user = await context.Users.FirstAsync(u => u.Email == email);
        
        var userManager = CreateUserManager(context);
        await userManager.AddToRolesAsync(user, new[] { "Role1", "Role2" });

        var roles = await service.GetUserRolesAsync(user.Id);

        roles.Should().HaveCount(2);
        roles.Should().Contain(new[] { "Role1", "Role2" });
    }

    [Fact]
    public async Task DeactivateUserAsync_WhenUserExists_ShouldToggleIsActive()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        var email = "deactivate@test.local";
        await service.CreateUserAsync(email, "Pass@123", "User Name");
        var user = await context.Users.FirstAsync(u => u.Email == email);

        var result = await service.DeactivateUserAsync(user.Id);
        var updated = await context.Users.FirstAsync(u => u.Id == user.Id);

        result.Succeeded.Should().BeTrue();
        updated.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task SoftDeleteUserAsync_WhenUserExists_ShouldSetFlags()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        var email = "delete@test.local";
        await service.CreateUserAsync(email, "Pass@123", "User Name");
        var user = await context.Users.FirstAsync(u => u.Email == email);

        var result = await service.SoftDeleteUserAsync(user.Id);
        var updated = await context.Users.IgnoreQueryFilters().FirstAsync(u => u.Id == user.Id);

        result.Succeeded.Should().BeTrue();
        updated.IsDeleted.Should().BeTrue();
        updated.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserMetricsAsync_ShouldReturnCorrectCounts()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        var userManager = CreateUserManager(context);

        // Seed roles
        await context.Roles.AddRangeAsync(
            new ApplicationRole { Name = "Patient", NormalizedName = "PATIENT" },
            new ApplicationRole { Name = "Ophthalmologist", NormalizedName = "OPHTHALMOLOGIST" }
        );
        await context.SaveChangesAsync();

        // Add 3 users
        for (int i = 0; i < 3; i++)
        {
            var user = new ApplicationUser { Email = $"u{i}@t.l", UserName = $"u{i}@t.l", IsActive = true };
            await userManager.CreateAsync(user);
            if (i == 0) await userManager.AddToRoleAsync(user, "Patient");
            if (i == 1) await userManager.AddToRoleAsync(user, "Ophthalmologist");
        }

        var metrics = await service.GetUserMetricsAsync();

        metrics.TotalUsers.Should().Be(3);
        metrics.ActiveDoctors.Should().Be(1);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidCredentials_ShouldSucceed()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        var email = "pass-change@test.local";
        var oldPass = "OldPass@123";
        var newPass = "NewPass@123";
        
        await service.CreateUserAsync(email, oldPass, "User Name");
        var user = await context.Users.FirstAsync(u => u.Email == email);

        var result = await service.ChangePasswordAsync(user.Id, oldPass, newPass);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task SetStaffOnboardingStatusAsync_ShouldUpdateProfileFlags()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        await service.CreateUserAsync("staff@t.l", "Staff@123!", "Staff User");
        var user = await context.Users.FirstAsync(u => u.Email == "staff@t.l");

        await service.SetStaffOnboardingStatusAsync(user.Id);
        var updated = await context.Users.FirstAsync(u => u.Id == user.Id);

        updated.EmailConfirmed.Should().BeTrue();
        updated.MustUpdateProfile.Should().BeTrue();
    }

    #region Helpers

    private static UserManager<ApplicationUser> CreateUserManager(ApplicationDbContext context)
    {
        var userStore = new UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken, ApplicationRoleClaim>(context);
        return new UserManager<ApplicationUser>(
            userStore, null, new PasswordHasher<ApplicationUser>(),
            [new UserValidator<ApplicationUser>()],
            [new PasswordValidator<ApplicationUser>()],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null, null);
    }

    private static IdentityService CreateService(ApplicationDbContext context)
    {
        var userManager = CreateUserManager(context);
        var roleStore = new RoleStore<ApplicationRole, ApplicationDbContext, Guid, ApplicationUserRole, ApplicationRoleClaim>(context);
        var roleManager = new RoleManager<ApplicationRole>(
            roleStore, [new RoleValidator<ApplicationRole>()],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null);

        return new IdentityService(userManager, roleManager, context);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    #endregion
}
