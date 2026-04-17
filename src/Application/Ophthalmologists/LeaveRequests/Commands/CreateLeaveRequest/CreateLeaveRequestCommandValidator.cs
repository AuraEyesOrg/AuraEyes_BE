using Application.Common.Helpers;
using FluentValidation;

namespace Application.Ophthalmologists.LeaveRequests.Commands.CreateLeaveRequest;

public class CreateLeaveRequestCommandValidator : AbstractValidator<CreateLeaveRequestCommand>
{
    private const int MinAdvanceNoticeDays = 3;

    public CreateLeaveRequestCommandValidator()
    {
        RuleFor(x => x.OphthalmologistId)
            .NotEmpty().WithMessage("Ophthalmologist ID is required.");

        RuleFor(x => x.StartDate)
            .Must(BeAtLeastMinNotice)
            .WithMessage($"Start date must be at least {MinAdvanceNoticeDays} days from today.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be greater than or equal to start date.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(1000).WithMessage("Reason must be 1000 characters or fewer.");
    }

    private static bool BeAtLeastMinNotice(DateOnly startDate)
    {
        var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZoneResolver.TimeZone);
        var minDate = DateOnly.FromDateTime(vietnamNow).AddDays(MinAdvanceNoticeDays);
        return startDate >= minDate;
    }
}
