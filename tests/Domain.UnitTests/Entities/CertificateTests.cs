using Domain.Entities.Users;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class CertificateTests
{
    private readonly Guid _ophthalmologistId = Guid.NewGuid();

    private Certificate CreateValidCertificate(
        CertificateType type = CertificateType.License,
        DegreeLevel? degreeLevel = null,
        DateTime? expiryDate = null)
    {
        return new Certificate(
            _ophthalmologistId,
            type,
            "Board Certification",
            degreeLevel,
            "Medical Board",
            DateTime.UtcNow.AddYears(-2),
            expiryDate,
            "https://certs/cert.pdf");
    }

    #region Constructor

    [Fact]
    public void Constructor_ValidLicense_ShouldCreate()
    {
        var cert = new Certificate(
            _ophthalmologistId,
            CertificateType.License,
            "Medical License",
            null,
            "State Board",
            new DateTime(2023, 1, 1),
            new DateTime(2028, 1, 1),
            "https://certs/lic.pdf");

        cert.OphthalmologistId.Should().Be(_ophthalmologistId);
        cert.Type.Should().Be(CertificateType.License);
        cert.Name.Should().Be("Medical License");
        cert.DegreeLevel.Should().BeNull();
        cert.IssuingAuthority.Should().Be("State Board");
        cert.IssuedDate.Should().Be(new DateTime(2023, 1, 1));
        cert.ExpiryDate.Should().Be(new DateTime(2028, 1, 1));
        cert.CertificateUrl.Should().Be("https://certs/lic.pdf");
    }

    [Fact]
    public void Constructor_ValidDegree_ShouldCreate()
    {
        var cert = new Certificate(
            _ophthalmologistId,
            CertificateType.Degree,
            "MD Ophthalmology",
            DegreeLevel.Doctor,
            "University of Medicine",
            new DateTime(2020, 6, 1));

        cert.Type.Should().Be(CertificateType.Degree);
        cert.DegreeLevel.Should().Be(DegreeLevel.Doctor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyName_ShouldThrow(string? name)
    {
        var act = () => new Certificate(
            _ophthalmologistId, CertificateType.License, name!, null, null, DateTime.UtcNow);

        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void Constructor_Degree_WithoutDegreeLevel_ShouldThrow()
    {
        var act = () => new Certificate(
            _ophthalmologistId, CertificateType.Degree, "MD", null, null, DateTime.UtcNow);

        act.Should().Throw<ArgumentException>().WithParameterName("degreeLevel");
    }

    [Fact]
    public void Constructor_NonDegree_WithDegreeLevel_ShouldThrow()
    {
        var act = () => new Certificate(
            _ophthalmologistId, CertificateType.License, "License", DegreeLevel.Master, null, DateTime.UtcNow);

        act.Should().Throw<ArgumentException>().WithParameterName("degreeLevel");
    }

    #endregion

    #region UpdateCertificate

    [Fact]
    public void UpdateCertificate_ValidInput_ShouldUpdateFields()
    {
        var cert = CreateValidCertificate();
        var newExpiry = DateTime.UtcNow.AddYears(5);

        cert.UpdateCertificate(
            CertificateType.Degree,
            "Updated Name",
            DegreeLevel.Professor,
            "New Authority",
            DateTime.UtcNow,
            newExpiry,
            "https://new-url");

        cert.Type.Should().Be(CertificateType.Degree);
        cert.Name.Should().Be("Updated Name");
        cert.DegreeLevel.Should().Be(DegreeLevel.Professor);
        cert.IssuingAuthority.Should().Be("New Authority");
        cert.ExpiryDate.Should().Be(newExpiry);
        cert.CertificateUrl.Should().Be("https://new-url");
        cert.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateCertificate_EmptyName_ShouldThrow(string? name)
    {
        var cert = CreateValidCertificate();

        var act = () => cert.UpdateCertificate(CertificateType.License, name!, null, null, DateTime.UtcNow, null, null);

        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void UpdateCertificate_Degree_WithoutDegreeLevel_ShouldThrow()
    {
        var cert = CreateValidCertificate();

        var act = () => cert.UpdateCertificate(CertificateType.Degree, "Name", null, null, DateTime.UtcNow, null, null);

        act.Should().Throw<ArgumentException>().WithParameterName("degreeLevel");
    }

    [Fact]
    public void UpdateCertificate_NonDegree_WithDegreeLevel_ShouldThrow()
    {
        var cert = CreateValidCertificate();

        var act = () => cert.UpdateCertificate(
            CertificateType.License, "Name", DegreeLevel.Bachelor, null, DateTime.UtcNow, null, null);

        act.Should().Throw<ArgumentException>().WithParameterName("degreeLevel");
    }

    #endregion

    #region IsExpired

    [Fact]
    public void IsExpired_NoExpiryDate_ShouldReturnFalse()
    {
        var cert = CreateValidCertificate(expiryDate: null);

        cert.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_FutureExpiry_ShouldReturnFalse()
    {
        var cert = CreateValidCertificate(expiryDate: DateTime.UtcNow.AddYears(5));

        cert.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_PastExpiry_ShouldReturnTrue()
    {
        var cert = CreateValidCertificate(expiryDate: DateTime.UtcNow.AddDays(-1));

        cert.IsExpired.Should().BeTrue();
    }

    #endregion
}
