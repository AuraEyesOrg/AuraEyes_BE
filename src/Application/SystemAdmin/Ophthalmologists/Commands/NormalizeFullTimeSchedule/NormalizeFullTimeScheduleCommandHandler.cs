using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Ophthalmologists.Commands.NormalizeFullTimeSchedule;

public class NormalizeFullTimeScheduleCommandHandler : ICommandHandler<NormalizeFullTimeScheduleCommand, NormalizeFullTimeScheduleResultDto>
{
    public Task<Result<NormalizeFullTimeScheduleResultDto>> Handle(NormalizeFullTimeScheduleCommand request, CancellationToken cancellationToken)
    {
        // Clinic-centric scheduling means doctors don't own slots.
        return Task.FromResult(Result<NormalizeFullTimeScheduleResultDto>.Success(new NormalizeFullTimeScheduleResultDto
        {
            OphthalmologistId = request.OphthalmologistId,
            FromDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ToDate = DateOnly.FromDateTime(DateTime.UtcNow),
            WindowDays = request.WindowDays ?? 0
        }));
    }
}
