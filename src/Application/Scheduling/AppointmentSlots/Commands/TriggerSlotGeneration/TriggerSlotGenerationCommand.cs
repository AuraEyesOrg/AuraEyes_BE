using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Scheduling.AppointmentSlots.Commands.TriggerSlotGeneration;

public record TriggerSlotGenerationCommand : IRequest<Result<Unit>>;

public class TriggerSlotGenerationCommandHandler : IRequestHandler<TriggerSlotGenerationCommand, Result<Unit>>
{
    private readonly IFullTimeSlotGenerationService _slotGenerationService;

    public TriggerSlotGenerationCommandHandler(IFullTimeSlotGenerationService slotGenerationService)
    {
        _slotGenerationService = slotGenerationService;
    }

    public async Task<Result<Unit>> Handle(TriggerSlotGenerationCommand request, CancellationToken cancellationToken)
    {
        await _slotGenerationService.TriggerGenerationAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
