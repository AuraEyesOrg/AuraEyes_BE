using FluentValidation;

namespace Application.SystemAdmin.Organisations.Commands.UpdateMonthlyQuota;

public class UpdateOrganisationMonthlyQuotaCommandValidator
    : AbstractValidator<UpdateOrganisationMonthlyQuotaCommand>
{
    public UpdateOrganisationMonthlyQuotaCommandValidator()
    {
        RuleFor(x => x.OrganisationId)
            .NotEmpty().WithMessage("Organisation ID is required.");

        RuleFor(x => x.MonthlyQuotaLimit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Monthly quota limit must be zero or greater.");
    }
}
