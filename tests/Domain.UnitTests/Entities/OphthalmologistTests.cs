using Domain.Entities.Users;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class OphthalmologistTests
{
    [Fact]
    public void Constructor_ShouldCreateWithPendingVerification()
    {
        var userId = Guid.NewGuid();

        var doctor = new Ophthalmologist(userId, "Eye specialist", 10, "+84123", "license.pdf", "degree.pdf");

        doctor.UserId.Should().Be(userId);
        doctor.Bio.Should().Be("Eye specialist");
        doctor.YearsOfExperience.Should().Be(10);
        doctor.Phone.Should().Be("+84123");
        doctor.LicenseUrl.Should().Be("license.pdf");
        doctor.DegreeUrl.Should().Be("degree.pdf");
        doctor.IsVerified.Should().BeFalse();
        doctor.VerificationStatus.Should().Be(VerificationStatus.PendingVerification);
        doctor.Certificates.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_DefaultValues_ShouldUseDefaults()
    {
        var doctor = new Ophthalmologist(Guid.NewGuid());

        doctor.Bio.Should().BeNull();
        doctor.YearsOfExperience.Should().Be(0);
        doctor.Phone.Should().BeNull();
    }

    [Fact]
    public void UpdateProfile_ValidInput_ShouldUpdate()
    {
        var doctor = new Ophthalmologist(Guid.NewGuid());

        doctor.UpdateProfile("Updated bio", 5);

        doctor.Bio.Should().Be("Updated bio");
        doctor.YearsOfExperience.Should().Be(5);
    }

    [Fact]
    public void UpdateProfile_NegativeYears_ShouldThrow()
    {
        var doctor = new Ophthalmologist(Guid.NewGuid());

        var act = () => doctor.UpdateProfile("bio", -1);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Years of experience cannot be negative*");
    }

    [Fact]
    public void Verify_ShouldSetApproved()
    {
        var doctor = new Ophthalmologist(Guid.NewGuid());

        doctor.Verify();

        doctor.IsVerified.Should().BeTrue();
        doctor.VerificationStatus.Should().Be(VerificationStatus.Approved);
        doctor.RejectionReason.Should().BeNull();
    }

    [Fact]
    public void Reject_ShouldSetRejected()
    {
        var doctor = new Ophthalmologist(Guid.NewGuid());

        doctor.Reject("Invalid license");

        doctor.IsVerified.Should().BeFalse();
        doctor.VerificationStatus.Should().Be(VerificationStatus.Rejected);
        doctor.RejectionReason.Should().Be("Invalid license");
    }

    [Fact]
    public void Unverify_ShouldResetToPending()
    {
        var doctor = new Ophthalmologist(Guid.NewGuid());
        doctor.Verify();

        doctor.Unverify();

        doctor.IsVerified.Should().BeFalse();
        doctor.VerificationStatus.Should().Be(VerificationStatus.PendingVerification);
    }

    [Fact]
    public void VerificationLifecycle_Verify_Unverify_Reject()
    {
        var doctor = new Ophthalmologist(Guid.NewGuid());

        doctor.VerificationStatus.Should().Be(VerificationStatus.PendingVerification);

        doctor.Verify();
        doctor.VerificationStatus.Should().Be(VerificationStatus.Approved);

        doctor.Unverify();
        doctor.VerificationStatus.Should().Be(VerificationStatus.PendingVerification);

        doctor.Reject("Fraudulent");
        doctor.VerificationStatus.Should().Be(VerificationStatus.Rejected);
    }

    [Fact]
    public void UpdateCredentialFiles_ShouldUpdateNonNullValues()
    {
        var doctor = new Ophthalmologist(Guid.NewGuid(), licenseUrl: "old-license.pdf");

        doctor.UpdateCredentialFiles("new-license.pdf", null);

        doctor.LicenseUrl.Should().Be("new-license.pdf");
        doctor.DegreeUrl.Should().BeNull();
    }

    [Fact]
    public void UpdateCredentialFiles_BothValues_ShouldUpdateBoth()
    {
        var doctor = new Ophthalmologist(Guid.NewGuid());

        doctor.UpdateCredentialFiles("license.pdf", "degree.pdf");

        doctor.LicenseUrl.Should().Be("license.pdf");
        doctor.DegreeUrl.Should().Be("degree.pdf");
    }
}
