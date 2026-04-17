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
    public async Task GetUserByEmailAsync_WhenMissing_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.GetUserByEmailAsync("missing@test.local");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenMissing_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.GetUserByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task IsEmailConfirmedAsync_WhenMissing_ShouldReturnFalse()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.IsEmailConfirmedAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsUserActiveAsync_WhenMissing_ShouldReturnFalse()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.IsUserActiveAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserRolesAsync_WhenMissing_ShouldReturnEmpty()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var roles = await service.GetUserRolesAsync(Guid.NewGuid());

        roles.Should().BeEmpty();
    }

    [Fact]
    public async Task IsInRoleAsync_WhenMissing_ShouldReturnFalse()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.IsInRoleAsync(Guid.NewGuid(), "AnyRole");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyTwoFactorCodeAsync_WhenMissing_ShouldReturnFalse()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.VerifyTwoFactorCodeAsync(Guid.NewGuid(), "123456");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyRecoveryCodeAsync_WhenMissing_ShouldReturnNotSucceeded()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.VerifyRecoveryCodeAsync(Guid.NewGuid(), "rc-1");

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("User not found");
    }

    [Fact]
    public async Task GetRecoveryCodesCountAsync_WhenMissing_ShouldReturnZero()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var count = await service.GetRecoveryCodesCountAsync(Guid.NewGuid());

        count.Should().Be(0);
    }

    [Fact]
    public async Task GenerateEmailConfirmationTokenAsync_WhenMissing_ShouldThrow()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var act = async () => await service.GenerateEmailConfirmationTokenAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*User not found*");
    }

    [Fact]
    public async Task GeneratePasswordResetTokenAsync_WhenMissing_ShouldThrow()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var act = async () => await service.GeneratePasswordResetTokenAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*User not found*");
    }

    [Theory]
    [InlineData("alice@example.com", "ALICEKEY123456")]
    [InlineData("bob+tag@example.com", "B0BKEY987654")]
    [InlineData("user.name@domain.local", "ZXCVBNM1234")]
    [InlineData("a@b.co", "ABCD1234")]
    [InlineData("x@y.z", "XYZ98765")]
    public void GenerateAuthenticatorUri_ShouldContainExpectedPayload(string email, string key)
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var uri = service.GenerateAuthenticatorUri(email, key);

        uri.Should().StartWith("otpauth://totp/");
        uri.Should().Contain("secret=");
        uri.Should().Contain(key);
        uri.Should().Contain("issuer=");
    }

    [Theory]
    [InlineData("abcd", "ABCD")]
    [InlineData("abcdefgh", "ABCD EFGH")]
    [InlineData("abcdefghijk", "ABCD EFGH IJK")]
    [InlineData("a1b2c3d4e5f6", "A1B2 C3D4 E5F6")]
    [InlineData("zzzzyyyyxxxx", "ZZZZ YYYY XXXX")]
    public void FormatAuthenticatorKey_ShouldGroupAndUppercase(string input, string expected)
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var formatted = service.FormatAuthenticatorKey(input);

        formatted.Should().Be(expected);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidInput_ShouldSucceed()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.CreateUserAsync("new-user@test.local", "Password@123", "New User");

        result.Succeeded.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateUserWithRoleAsync_WithValidInput_ShouldSucceedAndAttachRole()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.CreateUserWithRoleAsync("role-user@test.local", "Password@123", "Role User", "Patient");
        var createdUser = await service.GetUserByEmailAsync("role-user@test.local");
        var roles = await service.GetUserRolesAsync(createdUser!.Id);

        result.Succeeded.Should().BeTrue();
        roles.Should().Contain("Patient");
    }

    [Fact]
    public async Task IsPhoneNumberInUseByOrganizationAsync_WithNormalizedPhone_ShouldMatch()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        var organizationId = Guid.NewGuid();
        var userResult = await service.CreateUserAsync("phone-org@test.local", "Password@123", "Phone User");
        userResult.Succeeded.Should().BeTrue();
        var user = await context.Users.FirstAsync(x => x.Email == "phone-org@test.local");
        user.OrganizationId = organizationId;
        user.PhoneNumber = "+84 909-123-456";
        await context.SaveChangesAsync();

        var exists = await service.IsPhoneNumberInUseByOrganizationAsync(organizationId, "+84 909-123-456");

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ConfirmEmailAsync_WhenMissingUser_ShouldReturnFailure()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.ConfirmEmailAsync(Guid.NewGuid(), "token");

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("User not found");
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenMissingUser_ShouldReturnFailure()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.ResetPasswordAsync(Guid.NewGuid(), "token", "Password@123");

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("User not found");
    }

    [Fact]
    public async Task UpdateLastLoginAsync_WhenMissingUser_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var act = async () => await service.UpdateLastLoginAsync(Guid.NewGuid());

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeactivateUserAsync_WhenMissingUser_ShouldReturnFailure()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.DeactivateUserAsync(Guid.NewGuid());

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("User not found");
    }

    [Fact]
    public async Task SoftDeleteUserAsync_WhenMissingUser_ShouldReturnFailure()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.SoftDeleteUserAsync(Guid.NewGuid());

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("User not found");
    }

    [Fact]
    public async Task GetOrCreateAuthenticatorKeyAsync_WhenMissingUser_ShouldThrow()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var act = async () => await service.GetOrCreateAuthenticatorKeyAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*User not found*");
    }

    [Fact]
    public async Task GenerateNewRecoveryCodesAsync_WhenMissingUser_ShouldThrow()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var act = async () => await service.GenerateNewRecoveryCodesAsync(Guid.NewGuid(), 5);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*User not found*");
    }

    [Fact]
    public async Task GetPendingApprovalsCountAsync_WithNoUsers_ShouldBeZero()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var count = await service.GetPendingApprovalsCountAsync();

        count.Should().Be(0);
    }

    [Fact]
    public async Task GetUserDetailsAsync_WhenMissingUser_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var details = await service.GetUserDetailsAsync(Guid.NewGuid());

        details.Should().BeNull();
    }

    [Fact]
    public async Task UpdateUserProfileAsync_WhenMissingUser_ShouldReturnFailure()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.UpdateUserProfileAsync(Guid.NewGuid(), "Name", "0909", null, null, "Address");

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("User not found");
    }

    [Fact]
    public async Task UpdateAvatarUrlAsync_WhenMissingUser_ShouldReturnFailure()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.UpdateAvatarUrlAsync(Guid.NewGuid(), "https://avatar.local/a.png");

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("User not found");
    }

    [Fact]
    public async Task UpdateUserOrganizationAsync_WhenMissingUser_ShouldReturnFailure()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.UpdateUserOrganizationAsync(Guid.NewGuid(), Guid.NewGuid());

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("User not found");
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenMissingUser_ShouldReturnFailure()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.ChangePasswordAsync(Guid.NewGuid(), "Old@123", "New@123");

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("User not found");
    }

    private static IdentityService CreateService(ApplicationDbContext context)
    {
        var userStore = new UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken, ApplicationRoleClaim>(context);
        var userManager = new UserManager<ApplicationUser>(
            userStore,
            null,
            new PasswordHasher<ApplicationUser>(),
            [new UserValidator<ApplicationUser>()],
            [new PasswordValidator<ApplicationUser>()],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null,
            null);

        var roleStore = new RoleStore<ApplicationRole, ApplicationDbContext, Guid, ApplicationUserRole, ApplicationRoleClaim>(context);
        var roleManager = new RoleManager<ApplicationRole>(
            roleStore,
            [new RoleValidator<ApplicationRole>()],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null);

        return new IdentityService(userManager, roleManager);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
}
