using Application.SystemAdmin.Permissions.Commands.CreatePermission;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreatePermissionCommandValidatorTests
{
    private readonly CreatePermissionCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CreatePermissionCommand
        {
            Name = "users:read",
            DisplayName = "Read Users",
            Description = "Allows reading user data",
            Category = "Users"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ValidCommandMinimalFields_ShouldPass()
    {
        var command = new CreatePermissionCommand
        {
            Name = "admin.all",
            DisplayName = "Admin All"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_ShouldFail()
    {
        var command = new CreatePermissionCommand
        {
            Name = "",
            DisplayName = "Some Permission"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Permission name is required.");
    }

    [Fact]
    public void Validate_NameExceeds100Characters_ShouldFail()
    {
        var command = new CreatePermissionCommand
        {
            Name = new string('a', 101),
            DisplayName = "Some Permission"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Permission name must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_NameExactly100Characters_ShouldPass()
    {
        var command = new CreatePermissionCommand
        {
            Name = new string('a', 100),
            DisplayName = "Some Permission"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("users read")]
    [InlineData("users-read")]
    [InlineData("users@read")]
    [InlineData("users/read")]
    public void Validate_NameWithInvalidCharacters_ShouldFail(string name)
    {
        var command = new CreatePermissionCommand
        {
            Name = name,
            DisplayName = "Some Permission"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Permission name may only contain letters, digits, underscores, dots, and colons (e.g. 'users:read').");
    }

    [Theory]
    [InlineData("users:read")]
    [InlineData("admin.users_write")]
    [InlineData("A1_b2.C3:D4")]
    public void Validate_NameWithValidCharacters_ShouldPass(string name)
    {
        var command = new CreatePermissionCommand
        {
            Name = name,
            DisplayName = "Some Permission"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_EmptyDisplayName_ShouldFail()
    {
        var command = new CreatePermissionCommand
        {
            Name = "test:perm",
            DisplayName = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DisplayName)
            .WithErrorMessage("Display name is required.");
    }

    [Fact]
    public void Validate_DisplayNameExceeds200Characters_ShouldFail()
    {
        var command = new CreatePermissionCommand
        {
            Name = "test:perm",
            DisplayName = new string('A', 201)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DisplayName)
            .WithErrorMessage("Display name must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_DescriptionExceeds500Characters_ShouldFail()
    {
        var command = new CreatePermissionCommand
        {
            Name = "test:perm",
            DisplayName = "Test",
            Description = new string('A', 501)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 500 characters.");
    }

    [Fact]
    public void Validate_DescriptionExactly500Characters_ShouldPass()
    {
        var command = new CreatePermissionCommand
        {
            Name = "test:perm",
            DisplayName = "Test",
            Description = new string('A', 500)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_CategoryExceeds100Characters_ShouldFail()
    {
        var command = new CreatePermissionCommand
        {
            Name = "test:perm",
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
        var command = new CreatePermissionCommand
        {
            Name = "test:perm",
            DisplayName = "Test",
            Description = null,
            Category = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
        result.ShouldNotHaveValidationErrorFor(x => x.Category);
    }
}
