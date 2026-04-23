using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Ophthalmologists.Commands.DeleteFutureOphthalmologistSlots;

public class DeleteFutureOphthalmologistSlotsCommandHandler : ICommandHandler<DeleteFutureOphthalmologistSlotsCommand, DeleteFutureOphthalmologistSlotsResultDto>
{
    public Task<Result<DeleteFutureOphthalmologistSlotsResultDto>> Handle(DeleteFutureOphthalmologistSlotsCommand request, CancellationToken cancellationToken)
    {
        // Clinic-centric scheduling means doctors don't own slots, so there's nothing to delete.
        return Task.FromResult(Result<DeleteFutureOphthalmologistSlotsResultDto>.Success(new DeleteFutureOphthalmologistSlotsResultDto
        {
            OphthalmologistId = request.OphthalmologistId,
            Today = DateOnly.FromDateTime(DateTime.UtcNow),
            CurrentTimeUtc = TimeOnly.FromDateTime(DateTime.UtcNow)
        }));
    }
}
