using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Pricing.Interfaces;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlotCost;

/// <summary>
/// Handler for UpdateAppointmentSlotCostCommand.
/// </summary>
public class UpdateAppointmentSlotCostCommandHandler : ICommandHandler<UpdateAppointmentSlotCostCommand>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IExperiencePricingService _experiencePricingService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAppointmentSlotCostCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IExperiencePricingService experiencePricingService,
        IUnitOfWork unitOfWork)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _experiencePricingService = experiencePricingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateAppointmentSlotCostCommand request, CancellationToken cancellationToken)
    {
        var slot = await _appointmentSlotRepository.GetByIdWithTemplateAsync(request.AppointmentSlotId, cancellationToken);

        if (slot is null)
        {
            return Result.NotFound($"Appointment slot '{request.AppointmentSlotId}' not found.");
        }

        if (slot.Status is ScheduleStatus.Reserved or ScheduleStatus.Booked)
        {
            return Result.Failure("Cannot update cost for reserved or booked slots.");
        }

        var ophthalmologistId = slot.ScheduleTemplate?.OphthalId;
        if (ophthalmologistId.HasValue)
        {
            var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(ophthalmologistId.Value, cancellationToken);
            if (ophthalmologist is null)
            {
                return Result.NotFound($"Ophthalmologist '{ophthalmologistId.Value}' not found.");
            }

            if (ophthalmologist.EmploymentType == OphthalmologistEmploymentType.FullTime)
            {
                return Result.Forbidden("Full-time ophthalmologists cannot manually update slot costs.");
            }

            if (ophthalmologist.EmploymentType == OphthalmologistEmploymentType.PartTime)
            {
                var pricingValidation = await _experiencePricingService.ValidatePartTimeCostAsync(
                    ophthalmologist.Id,
                    request.Cost,
                    cancellationToken);

                if (!pricingValidation.IsSuccess)
                {
                    return pricingValidation.IsNotFound
                        ? Result.NotFound(pricingValidation.ErrorMessage)
                        : pricingValidation.IsForbidden
                            ? Result.Forbidden(pricingValidation.ErrorMessage)
                            : Result.Failure(pricingValidation.ErrorMessage);
                }
            }
        }

        slot.UpdateCost(request.Cost);

        await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
