using Application.SystemAdmin.Contracts.Commands.CreateContract;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreateContractCommandValidatorTests
{
    private readonly CreateContractCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateContractCommand
        {
            UserId = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            ContractNumber = "CTR-2026-001",
            AiQuotaLimit = 100,
            PlatformCommissionRate = 0.15m
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyUserId_ShouldFail()
    {
        var command = new CreateContractCommand
        {
            UserId = Guid.Empty,
            TemplateId = Guid.NewGuid(),
            ContractNumber = "CTR-001",
            AiQuotaLimit = 100,
            PlatformCommissionRate = 0.1m
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public void Validate_EmptyTemplateId_ShouldFail()
    {
        var command = new CreateContractCommand
        {
            UserId = Guid.NewGuid(),
            TemplateId = Guid.Empty,
            ContractNumber = "CTR-001",
            AiQuotaLimit = 100,
            PlatformCommissionRate = 0.1m
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TemplateId)
            .WithErrorMessage("Template ID is required.");
    }

    [Fact]
    public void Validate_EmptyContractNumber_ShouldFail()
    {
        var command = new CreateContractCommand
        {
            UserId = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            ContractNumber = "",
            AiQuotaLimit = 100,
            PlatformCommissionRate = 0.1m
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ContractNumber)
            .WithErrorMessage("Contract number is required.");
    }

    [Fact]
    public void Validate_ContractNumberExceeds50Characters_ShouldFail()
    {
        var command = new CreateContractCommand
        {
            UserId = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            ContractNumber = new string('C', 51),
            AiQuotaLimit = 100,
            PlatformCommissionRate = 0.1m
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ContractNumber)
            .WithErrorMessage("Contract number must not exceed 50 characters.");
    }

    [Fact]
    public void Validate_ContractNumberExactly50Characters_ShouldPass()
    {
        var command = new CreateContractCommand
        {
            UserId = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            ContractNumber = new string('C', 50),
            AiQuotaLimit = 100,
            PlatformCommissionRate = 0.1m
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ContractNumber);
    }

    [Fact]
    public void Validate_NegativeAiQuotaLimit_ShouldFail()
    {
        var command = new CreateContractCommand
        {
            UserId = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            ContractNumber = "CTR-001",
            AiQuotaLimit = -1,
            PlatformCommissionRate = 0.1m
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AiQuotaLimit)
            .WithErrorMessage("AI quota limit cannot be negative.");
    }

    [Fact]
    public void Validate_ZeroAiQuotaLimit_ShouldPass()
    {
        var command = new CreateContractCommand
        {
            UserId = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            ContractNumber = "CTR-001",
            AiQuotaLimit = 0,
            PlatformCommissionRate = 0.1m
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.AiQuotaLimit);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(1.01)]
    [InlineData(2.0)]
    public void Validate_CommissionRateOutOfRange_ShouldFail(decimal rate)
    {
        var command = new CreateContractCommand
        {
            UserId = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            ContractNumber = "CTR-001",
            AiQuotaLimit = 100,
            PlatformCommissionRate = rate
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PlatformCommissionRate)
            .WithErrorMessage("Commission rate must be between 0 and 1 (0% – 100%).");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.5)]
    [InlineData(1)]
    public void Validate_CommissionRateBoundaryValues_ShouldPass(decimal rate)
    {
        var command = new CreateContractCommand
        {
            UserId = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            ContractNumber = "CTR-001",
            AiQuotaLimit = 100,
            PlatformCommissionRate = rate
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.PlatformCommissionRate);
    }
}
