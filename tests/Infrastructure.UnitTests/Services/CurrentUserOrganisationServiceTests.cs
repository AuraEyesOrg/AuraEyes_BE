using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Services;

public class CurrentUserOrganisationServiceTests
{
    [Fact]
    public async Task GetOrganisationIdAsync_WhenUserExists_ShouldReturnOrgId()
    {
        await using var context = CreateContext();
        var orgId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "test@test.local",
            Email = "test@test.local",
            FullName = "Test User",
            OrganizationId = orgId
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var service = new CurrentUserOrganisationService(context);
        var result = await service.GetOrganisationIdAsync(user.Id);

        result.Should().Be(orgId);
    }

    [Fact]
    public async Task GetOrganisationIdAsync_WhenUserNotFound_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var service = new CurrentUserOrganisationService(context);

        var result = await service.GetOrganisationIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetOrganisationIdAsync_WhenUserDeleted_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "deleted@test.local",
            Email = "deleted@test.local",
            FullName = "Deleted User",
            IsDeleted = true,
            OrganizationId = Guid.NewGuid()
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var service = new CurrentUserOrganisationService(context);
        var result = await service.GetOrganisationIdAsync(user.Id);

        // Global filter or explicit filter in service will handle this
        result.Should().BeNull();
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
}
