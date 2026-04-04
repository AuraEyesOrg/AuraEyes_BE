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
        org.UsedAiQuota.Should().Be(0);
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

        org.UpdateDetails("New Name", "New Address", "NEW-LIC");

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

        var act = () => org.UpdateDetails(name!, null, null);

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
    public void HasAvailableQuota_WithinFreeQuota_ShouldReturnTrue()
    {
        var org = CreateValidOrganisation();

        org.HasAvailableQuota(freeQuota: 5).Should().BeTrue();
    }

    [Fact]
    public void HasAvailableQuota_FreeQuotaExhausted_WithPurchased_ShouldReturnTrue()
    {
        var org = CreateValidOrganisation();
        org.AddPurchasedQuota(10);
        for (var i = 0; i < 5; i++) org.ConsumeQuota(5);

        org.HasAvailableQuota(freeQuota: 5).Should().BeTrue();
    }

    [Fact]
    public void HasAvailableQuota_AllExhausted_ShouldReturnFalse()
    {
        var org = CreateValidOrganisation();
        for (var i = 0; i < 3; i++) org.ConsumeQuota(3);

        org.HasAvailableQuota(freeQuota: 3).Should().BeFalse();
    }

    [Fact]
    public void ConsumeQuota_WithinFreeQuota_ShouldIncrementUsed()
    {
        var org = CreateValidOrganisation();

        org.ConsumeQuota(freeQuota: 5);

        org.UsedAiQuota.Should().Be(1);
        org.PurchasedAiQuota.Should().Be(0);
    }

    [Fact]
    public void ConsumeQuota_FreeQuotaExhausted_ShouldDecrementPurchased()
    {
        var org = CreateValidOrganisation();
        org.AddPurchasedQuota(10);
        for (var i = 0; i < 3; i++) org.ConsumeQuota(3);

        org.ConsumeQuota(freeQuota: 3);

        org.PurchasedAiQuota.Should().Be(9);
    }

    [Fact]
    public void ConsumeQuota_NoneAvailable_ShouldThrow()
    {
        var org = CreateValidOrganisation();
        for (var i = 0; i < 2; i++) org.ConsumeQuota(2);

        var act = () => org.ConsumeQuota(freeQuota: 2);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ConsumeQuota_NegativeFreeQuota_ShouldThrow()
    {
        var org = CreateValidOrganisation();

        var act = () => org.ConsumeQuota(freeQuota: -1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ResetDailyQuota_ShouldResetUsedToZero()
    {
        var org = CreateValidOrganisation();
        org.ConsumeQuota(freeQuota: 5);
        org.ConsumeQuota(freeQuota: 5);

        org.ResetDailyQuota();

        org.UsedAiQuota.Should().Be(0);
        org.UpdatedAt.Should().NotBeNull();
    }

    #endregion
}
