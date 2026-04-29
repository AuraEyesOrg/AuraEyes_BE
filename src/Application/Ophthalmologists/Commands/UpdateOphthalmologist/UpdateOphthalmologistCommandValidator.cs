using FluentValidation;

namespace Application.Ophthalmologists.Commands.UpdateOphthalmologist;

/// <summary>
/// Validator for UpdateOphthalmologistCommand.
/// </summary>
public class UpdateOphthalmologistCommandValidator : AbstractValidator<UpdateOphthalmologistCommand>
{
    public UpdateOphthalmologistCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Ophthalmologist ID is required.");

        RuleFor(x => x.Bio)
            .MaximumLength(2000)
            .WithMessage("Bio cannot exceed 2000 characters.");

        RuleFor(x => x.EmploymentType)
            .IsInEnum()
            .When(x => x.EmploymentType.HasValue)
            .WithMessage("Employment type is invalid.");

        When(x => x.UserId.HasValue, () =>
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required for self-profile update.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name is required.")
                .MaximumLength(200)
                .WithMessage("Full name must not exceed 200 characters.");

            RuleFor(x => x.Phone)
                .MaximumLength(20)
                .WithMessage("Phone number must not exceed 20 characters.")
                .When(x => x.Phone is not null);

            RuleFor(x => x.Address)
                .MaximumLength(500)
                .WithMessage("Address must not exceed 500 characters.")
                .When(x => x.Address is not null);
        });
    }
}
