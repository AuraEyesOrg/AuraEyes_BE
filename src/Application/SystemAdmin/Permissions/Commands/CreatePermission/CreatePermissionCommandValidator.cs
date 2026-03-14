using FluentValidation;

namespace Application.SystemAdmin.Permissions.Commands.CreatePermission;

public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Permission name is required.")
            .MaximumLength(100).WithMessage("Permission name must not exceed 100 characters.")
            .Matches(@"^[a-zA-Z0-9_.:]+$")
            .WithMessage("Permission name may only contain letters, digits, underscores, dots, and colons (e.g. 'users:read').");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .MaximumLength(200).WithMessage("Display name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters.")
            .When(x => x.Category is not null);
    }
}
