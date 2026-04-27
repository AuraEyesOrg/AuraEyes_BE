using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Ophthalmologists.Commands.BackfillFullTimeSchedule;

public class BackfillFullTimeScheduleCommandHandler : ICommandHandler<BackfillFullTimeScheduleCommand, BackfillFullTimeScheduleResultDto>
{
    public Task<Result<BackfillFullTimeScheduleResultDto>> Handle(BackfillFullTimeScheduleCommand request, CancellationToken cancellationToken)
    {
        // Clinic-centric scheduling means doctors don't own slots, so backfilling is obsolete.
        return Task.FromResult(Result<BackfillFullTimeScheduleResultDto>.Success(new BackfillFullTimeScheduleResultDto
        {
            OphthalmologistId = request.OphthalmologistId,
            FromDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ToDate = DateOnly.FromDateTime(DateTime.UtcNow),
            WindowDays = request.WindowDays ?? 0
        }));
    }
}
