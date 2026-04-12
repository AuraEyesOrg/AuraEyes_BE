using Domain.Entities.Contracts;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class ContractTemplateTests
{
    private const string ValidTitle = "Standard Contract";
    private const string ValidVersion = "1.0";
    private const string ValidContent = "<html>Contract body</html>";

    private ContractTemplate CreateValidTemplate(
        ContractType type = ContractType.MedicalOrganizationContract,
        OphthalmologistEmploymentType? employmentType = null)
        => new(ValidTitle, type, ValidVersion, ValidContent, DateTime.UtcNow, employmentType);

    #region Constructor

    [Fact]
    public void Constructor_ValidMedicalOrganizationContract_ShouldCreate()
    {
        var effectiveDate = DateTime.UtcNow;

        var template = new ContractTemplate(
            ValidTitle, ContractType.MedicalOrganizationContract,
            ValidVersion, ValidContent, effectiveDate);

        template.Title.Should().Be(ValidTitle);
        template.Type.Should().Be(ContractType.MedicalOrganizationContract);
        template.ContractVersion.Should().Be(ValidVersion);
        template.ContentTemplate.Should().Be(ValidContent);
        template.EffectiveDate.Should().Be(effectiveDate);
        template.IsActive.Should().BeTrue();
        template.EmploymentType.Should().BeNull();
        template.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_ValidOphthalmologistContract_ShouldCreate()
    {
        var template = new ContractTemplate(
            ValidTitle, ContractType.OphthalmologistContract,
            ValidVersion, ValidContent, null, OphthalmologistEmploymentType.FullTime);

        template.Type.Should().Be(ContractType.OphthalmologistContract);
        template.EmploymentType.Should().Be(OphthalmologistEmploymentType.FullTime);
    }

    [Fact]
    public void Constructor_OphthalmologistWithoutEmploymentType_ShouldThrow()
    {
        var act = () => new ContractTemplate(
            ValidTitle, ContractType.OphthalmologistContract,
            ValidVersion, ValidContent);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("employmentType");
    }

    [Fact]
    public void Constructor_MedicalOrgWithEmploymentType_ShouldIgnoreEmploymentType()
    {
        var template = new ContractTemplate(
            ValidTitle, ContractType.MedicalOrganizationContract,
            ValidVersion, ValidContent, null, OphthalmologistEmploymentType.PartTime);

        template.EmploymentType.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyTitle_ShouldThrow(string? invalidTitle)
    {
        var act = () => new ContractTemplate(
            invalidTitle!, ContractType.MedicalOrganizationContract, ValidVersion, ValidContent);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("title");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyVersion_ShouldThrow(string? invalidVersion)
    {
        var act = () => new ContractTemplate(
            ValidTitle, ContractType.MedicalOrganizationContract, invalidVersion!, ValidContent);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("contractVersion");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyContent_ShouldThrow(string? invalidContent)
    {
        var act = () => new ContractTemplate(
            ValidTitle, ContractType.MedicalOrganizationContract, ValidVersion, invalidContent!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("contentTemplate");
    }

    [Fact]
    public void Constructor_NullEffectiveDate_ShouldCreate()
    {
        var template = new ContractTemplate(
            ValidTitle, ContractType.MedicalOrganizationContract, ValidVersion, ValidContent);

        template.EffectiveDate.Should().BeNull();
    }

    #endregion

    #region Update

    [Fact]
    public void Update_ValidInput_ShouldUpdateAllFields()
    {
        var template = CreateValidTemplate();
        var newDate = DateTime.UtcNow.AddDays(30);

        template.Update("New Title", ContractType.OphthalmologistContract,
            "2.0", "<html>New</html>", newDate, OphthalmologistEmploymentType.PartTime);

        template.Title.Should().Be("New Title");
        template.Type.Should().Be(ContractType.OphthalmologistContract);
        template.ContractVersion.Should().Be("2.0");
        template.ContentTemplate.Should().Be("<html>New</html>");
        template.EffectiveDate.Should().Be(newDate);
        template.EmploymentType.Should().Be(OphthalmologistEmploymentType.PartTime);
        template.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_EmptyTitle_ShouldThrow()
    {
        var template = CreateValidTemplate();

        var act = () => template.Update("", ContractType.MedicalOrganizationContract,
            ValidVersion, ValidContent, null, null);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("title");
    }

    [Fact]
    public void Update_OphthalmologistWithoutEmploymentType_ShouldThrow()
    {
        var template = CreateValidTemplate();

        var act = () => template.Update(ValidTitle, ContractType.OphthalmologistContract,
            ValidVersion, ValidContent, null, null);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("employmentType");
    }

    #endregion

    #region Activate / Deactivate

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        var template = CreateValidTemplate();
        template.Deactivate();

        template.Activate();

        template.IsActive.Should().BeTrue();
        template.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var template = CreateValidTemplate();

        template.Deactivate();

        template.IsActive.Should().BeFalse();
        template.UpdatedAt.Should().NotBeNull();
    }

    #endregion

    #region SoftDelete

    [Fact]
    public void SoftDelete_ShouldSetIsDeletedTrue()
    {
        var template = CreateValidTemplate();

        template.SoftDelete();

        template.IsDeleted.Should().BeTrue();
        template.UpdatedAt.Should().NotBeNull();
    }

    #endregion
}
