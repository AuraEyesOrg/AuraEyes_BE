using Domain.Entities.Users;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class OrganisationOnboardingRequestTests
{
    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateRequest()
    {
        var request = new OrganisationOnboardingRequest(
            " Eye Hospital ",
            OrgType.Hospital,
            " Dr. John ",
            " john@example.com ",
            " +123456 ",
            " 123 Main St ",
            " LIC-001 ",
            " Some notes ");

        request.OrganisationName.Should().Be("Eye Hospital");
        request.OrgType.Should().Be(OrgType.Hospital);
        request.ContactFullName.Should().Be("Dr. John");
        request.ContactEmail.Should().Be("john@example.com");
        request.ContactPhone.Should().Be("+123456");
        request.Address.Should().Be("123 Main St");
        request.LicenseNumber.Should().Be("LIC-001");
        request.Notes.Should().Be("Some notes");
        request.Status.Should().Be(OrganisationOnboardingStatus.Pending);
        request.ApprovedAt.Should().BeNull();
        request.ApprovedByUserId.Should().BeNull();
        request.OrganisationId.Should().BeNull();
        request.OrgAdminUserId.Should().BeNull();
    }

    [Fact]
    public void Constructor_MinimalInput_ShouldUseNullDefaults()
    {
        var request = new OrganisationOnboardingRequest("Hospital", OrgType.Clinic, "Jane", "jane@test.com");

        request.ContactPhone.Should().BeNull();
        request.Address.Should().BeNull();
        request.LicenseNumber.Should().BeNull();
        request.Notes.Should().BeNull();
    }

    [Fact]
    public void Constructor_WhitespaceOptionalFields_ShouldBeNull()
    {
        var request = new OrganisationOnboardingRequest(
            "Hospital", OrgType.Hospital, "John", "john@test.com", "   ", "   ", "   ", "   ");

        request.ContactPhone.Should().BeNull();
        request.Address.Should().BeNull();
        request.LicenseNumber.Should().BeNull();
        request.Notes.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyOrganisationName_ShouldThrow(string? name)
    {
        var act = () => new OrganisationOnboardingRequest(name!, OrgType.Hospital, "John", "john@test.com");

        act.Should().Throw<ArgumentException>().WithParameterName("organisationName");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyContactFullName_ShouldThrow(string? name)
    {
        var act = () => new OrganisationOnboardingRequest("Hospital", OrgType.Hospital, name!, "john@test.com");

        act.Should().Throw<ArgumentException>().WithParameterName("contactFullName");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyContactEmail_ShouldThrow(string? email)
    {
        var act = () => new OrganisationOnboardingRequest("Hospital", OrgType.Hospital, "John", email!);

        act.Should().Throw<ArgumentException>().WithParameterName("contactEmail");
    }

    #endregion

    #region Approve

    [Fact]
    public void Approve_PendingRequest_ShouldApprove()
    {
        var request = new OrganisationOnboardingRequest("Hospital", OrgType.Hospital, "John", "john@test.com");
        var approvedBy = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var adminUserId = Guid.NewGuid();

        request.Approve(approvedBy, orgId, adminUserId);

        request.Status.Should().Be(OrganisationOnboardingStatus.Approved);
        request.ApprovedByUserId.Should().Be(approvedBy);
        request.ApprovedAt.Should().NotBeNull();
        request.ApprovedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        request.OrganisationId.Should().Be(orgId);
        request.OrgAdminUserId.Should().Be(adminUserId);
        request.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Approve_AlreadyApproved_ShouldThrow()
    {
        var request = new OrganisationOnboardingRequest("Hospital", OrgType.Hospital, "John", "john@test.com");
        request.Approve(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var act = () => request.Approve(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*pending*");
    }

    #endregion
}
