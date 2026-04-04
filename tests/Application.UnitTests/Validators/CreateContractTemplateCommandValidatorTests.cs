using Application.SystemAdmin.ContractTemplates.Commands.CreateContractTemplate;
using Domain.Enums;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreateContractTemplateCommandValidatorTests
{
    private readonly CreateContractTemplateCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateContractTemplateCommand
        {
            Title = "Standard Contract",
            ContractVersion = "1.0",
            ContentTemplate = "<html>Contract body</html>",
            Type = ContractType.MedicalOrganizationContract
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ValidOphthalmologistContractWithEmploymentType_ShouldPass()
    {
        var command = new CreateContractTemplateCommand
        {
            Title = "Doctor Contract",
            ContractVersion = "2.0",
            ContentTemplate = "<html>Doctor contract body</html>",
            Type = ContractType.OphthalmologistContract,
            EmploymentType = OphthalmologistEmploymentType.FullTime
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyTitle_ShouldFail()
    {
        var command = new CreateContractTemplateCommand
        {
            Title = "",
            ContractVersion = "1.0",
            ContentTemplate = "<html>body</html>"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required.");
    }

    [Fact]
    public void Validate_TitleExceeds200Characters_ShouldFail()
    {
        var command = new CreateContractTemplateCommand
        {
            Title = new string('A', 201),
            ContractVersion = "1.0",
            ContentTemplate = "<html>body</html>"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_TitleExactly200Characters_ShouldPass()
    {
        var command = new CreateContractTemplateCommand
        {
            Title = new string('A', 200),
            ContractVersion = "1.0",
            ContentTemplate = "<html>body</html>"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_EmptyContractVersion_ShouldFail()
    {
        var command = new CreateContractTemplateCommand
        {
            Title = "Contract",
            ContractVersion = "",
            ContentTemplate = "<html>body</html>"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ContractVersion)
            .WithErrorMessage("Contract version is required.");
    }

    [Fact]
    public void Validate_ContractVersionExceeds20Characters_ShouldFail()
    {
        var command = new CreateContractTemplateCommand
        {
            Title = "Contract",
            ContractVersion = new string('1', 21),
            ContentTemplate = "<html>body</html>"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ContractVersion)
            .WithErrorMessage("Version must not exceed 20 characters.");
    }

    [Fact]
    public void Validate_ContractVersionExactly20Characters_ShouldPass()
    {
        var command = new CreateContractTemplateCommand
        {
            Title = "Contract",
            ContractVersion = new string('1', 20),
            ContentTemplate = "<html>body</html>"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ContractVersion);
    }

    [Fact]
    public void Validate_EmptyContentTemplate_ShouldFail()
    {
        var command = new CreateContractTemplateCommand
        {
            Title = "Contract",
            ContractVersion = "1.0",
            ContentTemplate = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ContentTemplate)
            .WithErrorMessage("Content template is required.");
    }

    [Fact]
    public void Validate_OphthalmologistContractWithoutEmploymentType_ShouldFail()
    {
        var command = new CreateContractTemplateCommand
        {
            Title = "Doctor Contract",
            ContractVersion = "1.0",
            ContentTemplate = "<html>body</html>",
            Type = ContractType.OphthalmologistContract,
            EmploymentType = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EmploymentType)
            .WithErrorMessage("Employment type is required for ophthalmologist contract templates.");
    }

    [Fact]
    public void Validate_MedicalOrgContractWithoutEmploymentType_ShouldPass()
    {
        var command = new CreateContractTemplateCommand
        {
            Title = "Org Contract",
            ContractVersion = "1.0",
            ContentTemplate = "<html>body</html>",
            Type = ContractType.MedicalOrganizationContract,
            EmploymentType = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.EmploymentType);
    }
}
