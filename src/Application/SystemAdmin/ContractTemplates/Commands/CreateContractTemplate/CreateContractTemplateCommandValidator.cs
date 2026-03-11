using FluentValidation;

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

        RuleForEach(x => x.Variables).SetValidator(new CreateVariableRequestValidator());
    }
}

public class CreateVariableRequestValidator : AbstractValidator<CreateVariableRequest>
{
    public CreateVariableRequestValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Variable key is required.")
            .MaximumLength(100).WithMessage("Key must not exceed 100 characters.")
            .Matches(@"^[a-zA-Z][a-zA-Z0-9_]*$")
            .WithMessage("Key must start with a letter and contain only letters, digits, or underscores.");

        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("Variable label is required.")
            .MaximumLength(200).WithMessage("Label must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.SelectOptions)
            .NotEmpty().WithMessage("SelectOptions is required for Select-type variables.")
            .When(x => x.VariableType == Domain.Enums.VariableType.Select);
    }
}
