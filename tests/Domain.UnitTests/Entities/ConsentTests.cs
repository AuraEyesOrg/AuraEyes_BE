using Domain.Entities.Users;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class ConsentTests
{
    private readonly Guid _screeningId = Guid.NewGuid();
    private readonly Guid _patientId = Guid.NewGuid();

    private Consent CreateValidConsent(string content = "I agree to the terms.")
    {
        return new Consent(_screeningId, _patientId, content);
    }

    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateConsent()
    {
        var consent = new Consent(_screeningId, _patientId, " Consent text ");

        consent.AiScreeningId.Should().Be(_screeningId);
        consent.PatientId.Should().Be(_patientId);
        consent.Content.Should().Be("Consent text");
        consent.IsAgreed.Should().BeFalse();
        consent.SignedAt.Should().BeNull();
    }

    [Fact]
    public void Constructor_EmptyScreeningId_ShouldThrow()
    {
        var act = () => new Consent(Guid.Empty, _patientId, "Content");

        act.Should().Throw<ArgumentException>().WithParameterName("aiScreeningId");
    }

    [Fact]
    public void Constructor_EmptyPatientId_ShouldThrow()
    {
        var act = () => new Consent(_screeningId, Guid.Empty, "Content");

        act.Should().Throw<ArgumentException>().WithParameterName("patientId");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyContent_ShouldThrow(string? content)
    {
        var act = () => new Consent(_screeningId, _patientId, content!);

        act.Should().Throw<ArgumentException>().WithParameterName("content");
    }

    #endregion

    #region Agree

    [Fact]
    public void Agree_WithoutNewContent_ShouldSetAgreedAndSignedAt()
    {
        var consent = CreateValidConsent();

        consent.Agree();

        consent.IsAgreed.Should().BeTrue();
        consent.SignedAt.Should().NotBeNull();
        consent.SignedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        consent.Content.Should().Be("I agree to the terms.");
        consent.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Agree_WithNewContent_ShouldUpdateContent()
    {
        var consent = CreateValidConsent();

        consent.Agree(" Updated consent text ");

        consent.IsAgreed.Should().BeTrue();
        consent.Content.Should().Be("Updated consent text");
    }

    #endregion

    #region UpdateContent

    [Fact]
    public void UpdateContent_ValidContent_ShouldUpdate()
    {
        var consent = CreateValidConsent();

        consent.UpdateContent(" New content ");

        consent.Content.Should().Be("New content");
        consent.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateContent_EmptyContent_ShouldThrow(string? content)
    {
        var consent = CreateValidConsent();

        var act = () => consent.UpdateContent(content!);

        act.Should().Throw<ArgumentException>().WithParameterName("content");
    }

    #endregion

    #region Revoke

    [Fact]
    public void Revoke_ShouldSetIsAgreedFalse()
    {
        var consent = CreateValidConsent();
        consent.Agree();

        consent.Revoke();

        consent.IsAgreed.Should().BeFalse();
        consent.UpdatedAt.Should().NotBeNull();
    }

    #endregion
}
