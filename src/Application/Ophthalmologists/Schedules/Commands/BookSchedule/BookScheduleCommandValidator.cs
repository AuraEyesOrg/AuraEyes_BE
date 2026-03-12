using FluentValidation;

namespace Application.Ophthalmologists.Schedules.Commands.BookSchedule;

public class BookScheduleCommandValidator : AbstractValidator<BookScheduleCommand>
{
    public BookScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty()
            .WithMessage("Schedule ID is required.");

        RuleFor(x => x.PatientId)
            .NotEmpty()
            .WithMessage("Patient ID is required.");
    }
}
