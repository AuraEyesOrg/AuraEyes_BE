using Application.Patients.Commands.ChangePassword;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class ChangePasswordCommandValidatorTests
{
    private readonly ChangePasswordCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new ChangePasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "OldPass123",
            NewPassword = "NewPass456",
            ConfirmNewPassword = "NewPass456"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyUserId_ShouldFail()
    {
        var command = new ChangePasswordCommand
        {
            UserId = Guid.Empty,
            CurrentPassword = "OldPass123",
            NewPassword = "NewPass456",
            ConfirmNewPassword = "NewPass456"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public void Validate_EmptyCurrentPassword_ShouldFail()
    {
        var command = new ChangePasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "",
            NewPassword = "NewPass456",
            ConfirmNewPassword = "NewPass456"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword)
            .WithErrorMessage("Current password is required.");
    }

    [Fact]
    public void Validate_EmptyNewPassword_ShouldFail()
    {
        var command = new ChangePasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "OldPass123",
            NewPassword = "",
            ConfirmNewPassword = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password is required.");
    }

    [Fact]
    public void Validate_NewPasswordTooShort_ShouldFail()
    {
        var command = new ChangePasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "OldPass123",
            NewPassword = "Short7!",
            ConfirmNewPassword = "Short7!"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("Password must be at least 8 characters.");
    }

    [Fact]
    public void Validate_NewPasswordExactly8Chars_ShouldPass()
    {
        var command = new ChangePasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "OldPass123",
            NewPassword = "Exactly8",
            ConfirmNewPassword = "Exactly8"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void Validate_NewPasswordSameAsCurrent_ShouldFail()
    {
        var command = new ChangePasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "SamePass123",
            NewPassword = "SamePass123",
            ConfirmNewPassword = "SamePass123"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password must be different from current password.");
    }

    [Fact]
    public void Validate_ConfirmPasswordDoesNotMatch_ShouldFail()
    {
        var command = new ChangePasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "OldPass123",
            NewPassword = "NewPass456",
            ConfirmNewPassword = "Mismatch789"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword)
            .WithErrorMessage("Passwords do not match.");
    }

    [Fact]
    public void Validate_EmptyConfirmPassword_ShouldFail()
    {
        var command = new ChangePasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "OldPass123",
            NewPassword = "NewPass456",
            ConfirmNewPassword = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword)
            .WithErrorMessage("Password confirmation is required.");
    }
}
