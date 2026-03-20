using FluentValidation;
using Domain.Enums;

namespace Application.SystemAdmin.ContractTemplates.Commands.CreateContractTemplate;

public class CreateContractTemplateCommandValidator : AbstractValidator<CreateContractTemplateCommand>
{
    public CreateContractTemplateCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.ContractVersion)
            .NotEmpty().WithMessage("Contract version is required.")
            .MaximumLength(20).WithMessage("Version must not exceed 20 characters.");

        RuleFor(x => x.ContentTemplate)
            .NotEmpty().WithMessage("Content template is required.");

        RuleFor(x => x.EmploymentType)
            .NotNull()
            .When(x => x.Type == ContractType.OphthalmologistContract)
            .WithMessage("Employment type is required for ophthalmologist contract templates.");
    }
}
