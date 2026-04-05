using Application.SystemAdmin.Permissions.Commands.UpdatePermission;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class UpdatePermissionCommandValidatorTests
{
    private readonly UpdatePermissionCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new UpdatePermissionCommand
        {
            PermissionId = Guid.NewGuid(),
            DisplayName = "Updated Permission",
            Description = "Updated description",
            Category = "Admin"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyPermissionId_ShouldFail()
    {
        var command = new UpdatePermissionCommand
        {
            PermissionId = Guid.Empty,
            DisplayName = "Test"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PermissionId)
            .WithErrorMessage("Permission ID is required.");
    }

    [Fact]
    public void Validate_EmptyDisplayName_ShouldFail()
    {
        var command = new UpdatePermissionCommand
        {
            PermissionId = Guid.NewGuid(),
            DisplayName = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DisplayName)
            .WithErrorMessage("Display name is required.");
    }

    [Fact]
    public void Validate_DisplayNameExceeds200Characters_ShouldFail()
    {
        var command = new UpdatePermissionCommand
        {
            PermissionId = Guid.NewGuid(),
            DisplayName = new string('A', 201)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DisplayName)
            .WithErrorMessage("Display name must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_DisplayNameExactly200Characters_ShouldPass()
    {
        var command = new UpdatePermissionCommand
        {
            PermissionId = Guid.NewGuid(),
            DisplayName = new string('A', 200)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public void Validate_DescriptionExceeds500Characters_ShouldFail()
    {
        var command = new UpdatePermissionCommand
        {
            PermissionId = Guid.NewGuid(),
            DisplayName = "Test",
            Description = new string('A', 501)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 500 characters.");
    }

    [Fact]
    public void Validate_CategoryExceeds100Characters_ShouldFail()
    {
        var command = new UpdatePermissionCommand
        {
            PermissionId = Guid.NewGuid(),
            DisplayName = "Test",
            Category = new string('A', 101)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Category)
            .WithErrorMessage("Category must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_NullDescriptionAndCategory_ShouldPass()
    {
        var command = new UpdatePermissionCommand
        {
            PermissionId = Guid.NewGuid(),
            DisplayName = "Test",
            Description = null,
            Category = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
        result.ShouldNotHaveValidationErrorFor(x => x.Category);
    }
}
