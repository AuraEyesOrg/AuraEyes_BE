using FluentValidation;

namespace Application.SystemAdmin.ContractTemplates.Commands.UpdateContractTemplate;

public class UpdateContractTemplateCommandValidator : AbstractValidator<UpdateContractTemplateCommand>
{
    public UpdateContractTemplateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Template ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.ContractVersion)
            .NotEmpty().WithMessage("Contract version is required.")
            .MaximumLength(20).WithMessage("Version must not exceed 20 characters.");

        RuleFor(x => x.ContentTemplate)
            .NotEmpty().WithMessage("Content template is required.");
    }
}
