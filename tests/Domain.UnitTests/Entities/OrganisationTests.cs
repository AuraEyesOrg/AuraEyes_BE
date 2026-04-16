using Domain.Entities.Users;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class OrganisationTests
{
    private readonly Guid _ownerId = Guid.NewGuid();

    private Organisation CreateValidOrganisation(string name = "Eye Hospital")
    {
        return new Organisation(_ownerId, name, OrgType.Hospital, "123 Main St", "LIC-001");
    }

    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateOrganisation()
    {
        var org = new Organisation(_ownerId, "Test Clinic", OrgType.Clinic, "456 Ave", "LIC-002");

        org.OwnerId.Should().Be(_ownerId);
        org.Name.Should().Be("Test Clinic");
        org.OrgType.Should().Be(OrgType.Clinic);
        org.Address.Should().Be("456 Ave");
        org.LicenseNumber.Should().Be("LIC-002");
        org.RatingAverage.Should().Be(0m);
        org.RatingCount.Should().Be(0);
        org.PurchasedAiQuota.Should().Be(0);
        org.MonthlyQuotaLimit.Should().Be(0);
        org.MonthlyQuotaUsed.Should().Be(0);
        org.MonthlyQuotaLastResetAt.Should().BeNull();
    }

    [Fact]
    public void Constructor_MinimalInput_ShouldUseNullDefaults()
    {
        var org = new Organisation(_ownerId, "Hospital", OrgType.Hospital);

        org.Address.Should().BeNull();
        org.LicenseNumber.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyName_ShouldThrow(string? name)
    {
        var act = () => new Organisation(_ownerId, name!, OrgType.Hospital);

        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    #endregion

    #region UpdateDetails

    [Fact]
    public void UpdateDetails_ValidInput_ShouldUpdateFields()
    {
        var org = CreateValidOrganisation();

        org.UpdateDetails("New Name", "New Address", "NEW-LIC", null, null);

        org.Name.Should().Be("New Name");
        org.Address.Should().Be("New Address");
        org.LicenseNumber.Should().Be("NEW-LIC");
        org.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_EmptyName_ShouldThrow(string? name)
    {
        var org = CreateValidOrganisation();

        var act = () => org.UpdateDetails(name!, null, null, null, null);

        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    #endregion

    #region ChangeOrgType

    [Fact]
    public void ChangeOrgType_ShouldUpdate()
    {
        var org = CreateValidOrganisation();

        org.ChangeOrgType(OrgType.Clinic);

        org.OrgType.Should().Be(OrgType.Clinic);
        org.UpdatedAt.Should().NotBeNull();
    }

    #endregion

    #region ApplyNewRating

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void ApplyNewRating_ValidRating_ShouldUpdateAverage(int rating)
    {
        var org = CreateValidOrganisation();

        org.ApplyNewRating(rating);

        org.RatingCount.Should().Be(1);
        org.RatingAverage.Should().Be(rating);
    }

    [Fact]
    public void ApplyNewRating_MultipleRatings_ShouldCalculateAverage()
    {
        var org = CreateValidOrganisation();

        org.ApplyNewRating(5);
        org.ApplyNewRating(3);

        org.RatingCount.Should().Be(2);
        org.RatingAverage.Should().Be(4.0m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void ApplyNewRating_InvalidRating_ShouldThrow(int rating)
    {
        var org = CreateValidOrganisation();

        var act = () => org.ApplyNewRating(rating);

        act.Should().Throw<ArgumentException>().WithParameterName("rating");
    }

    #endregion

    #region Quota Management

    [Fact]
    public void AddPurchasedQuota_ValidAmount_ShouldIncrease()
    {
        var org = CreateValidOrganisation();

        org.AddPurchasedQuota(100);

        org.PurchasedAiQuota.Should().Be(100);
        org.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AddPurchasedQuota_MultipleAdds_ShouldAccumulate()
    {
        var org = CreateValidOrganisation();

        org.AddPurchasedQuota(50);
        org.AddPurchasedQuota(30);

        org.PurchasedAiQuota.Should().Be(80);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void AddPurchasedQuota_ZeroOrNegative_ShouldThrow(int amount)
    {
        var org = CreateValidOrganisation();

        var act = () => org.AddPurchasedQuota(amount);

        act.Should().Throw<ArgumentException>().WithParameterName("amount");
    }

    [Fact]
    public void HasAvailableQuota_WithinMonthlyQuota_ShouldReturnTrue()
    {
        var org = CreateValidOrganisation();
        org.ConfigureMonthlyQuota(5, DateTime.UtcNow);

        org.HasAvailableQuota().Should().BeTrue();
    }

    [Fact]
    public void HasAvailableQuota_MonthlyExhausted_WithPurchased_ShouldReturnTrue()
    {
        var org = CreateValidOrganisation();
        org.ConfigureMonthlyQuota(3, DateTime.UtcNow);
        org.AddPurchasedQuota(10);
        for (var i = 0; i < 3; i++) org.ConsumeQuota();

        org.HasAvailableQuota().Should().BeTrue();
    }

    [Fact]
    public void HasAvailableQuota_AllExhausted_ShouldReturnFalse()
    {
        var org = CreateValidOrganisation();
        org.ConfigureMonthlyQuota(3, DateTime.UtcNow);
        for (var i = 0; i < 3; i++) org.ConsumeQuota();

        org.HasAvailableQuota().Should().BeFalse();
    }

    [Fact]
    public void ConsumeQuota_WithinMonthlyQuota_ShouldIncrementMonthlyUsed()
    {
        var org = CreateValidOrganisation();
        org.ConfigureMonthlyQuota(5, DateTime.UtcNow);

        org.ConsumeQuota();

        org.MonthlyQuotaUsed.Should().Be(1);
        org.PurchasedAiQuota.Should().Be(0);
    }

    [Fact]
    public void ConsumeQuota_MonthlyQuotaExhausted_ShouldDecrementPurchased()
    {
        var org = CreateValidOrganisation();
        org.ConfigureMonthlyQuota(3, DateTime.UtcNow);
        org.AddPurchasedQuota(10);
        for (var i = 0; i < 3; i++) org.ConsumeQuota();

        org.ConsumeQuota();

        org.PurchasedAiQuota.Should().Be(9);
    }

    [Fact]
    public void ConsumeQuota_NoneAvailable_ShouldThrow()
    {
        var org = CreateValidOrganisation();
        org.ConfigureMonthlyQuota(2, DateTime.UtcNow);
        for (var i = 0; i < 2; i++) org.ConsumeQuota();

        var act = () => org.ConsumeQuota();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ConfigureMonthlyQuota_NegativeLimit_ShouldThrow()
    {
        var org = CreateValidOrganisation();

        var act = () => org.ConfigureMonthlyQuota(-1, DateTime.UtcNow);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ResetMonthlyQuota_ShouldResetMonthlyUsedToZero()
    {
        var org = CreateValidOrganisation();
        var configuredAt = DateTime.UtcNow.AddDays(-5);
        org.ConfigureMonthlyQuota(5, configuredAt);
        org.ConsumeQuota();
        org.ConsumeQuota();

        var resetAt = DateTime.UtcNow;
        org.ResetMonthlyQuota(resetAt);

        org.MonthlyQuotaUsed.Should().Be(0);
        org.MonthlyQuotaLastResetAt.Should().Be(resetAt);
        org.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void ConfigureMonthlyQuota_ShouldSetLimitAndResetUsage()
    {
        var org = CreateValidOrganisation();
        org.ConfigureMonthlyQuota(10, DateTime.UtcNow.AddDays(-2));
        org.ConsumeQuota();

        var resetAt = DateTime.UtcNow;
        org.ConfigureMonthlyQuota(20, resetAt);

        org.MonthlyQuotaLimit.Should().Be(20);
        org.MonthlyQuotaUsed.Should().Be(0);
        org.MonthlyQuotaLastResetAt.Should().Be(resetAt);
        org.UpdatedAt.Should().NotBeNull();
    }

    #endregion
}
