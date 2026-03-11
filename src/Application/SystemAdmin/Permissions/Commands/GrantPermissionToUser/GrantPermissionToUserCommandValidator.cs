using FluentValidation;

namespace Application.SystemAdmin.Permissions.Commands.GrantPermissionToUser;

public class GrantPermissionToUserCommandValidator : AbstractValidator<GrantPermissionToUserCommand>
{
    public GrantPermissionToUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.PermissionId)
            .NotEmpty().WithMessage("Permission ID is required.");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.")
            .When(x => x.ExpiresAt.HasValue);
    }
}
