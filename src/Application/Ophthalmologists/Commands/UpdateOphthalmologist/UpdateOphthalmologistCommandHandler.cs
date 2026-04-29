using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Ophthalmologists.Commands.UpdateOphthalmologist;

/// <summary>
/// Handler for UpdateOphthalmologistCommand.
/// </summary>
public class UpdateOphthalmologistCommandHandler : ICommandHandler<UpdateOphthalmologistCommand>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;
    private readonly ISender _sender;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOphthalmologistCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService,
        ISender sender,
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
        _sender = sender;
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateOphthalmologistCommand request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.Id, cancellationToken);

        if (ophthalmologist is null)
        {
            return Result.NotFound($"Ophthalmologist with ID '{request.Id}' was not found.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            ophthalmologist.UpdateProfile(request.Bio);

            if (request.EmploymentType.HasValue)
            {
                ophthalmologist.UpdateEmploymentType(request.EmploymentType.Value);
            }

            if (request.ConsultationFee.HasValue && request.ConsultationFee.Value != ophthalmologist.ConsultationFee)
            {
                ophthalmologist.UpdateConsultationFee(request.ConsultationFee.Value);

                // Propagate fee change to all future unbooked slots for this doctor
                var today = DateOnly.FromDateTime(DateTime.UtcNow); // Use UTC/Vietnam logic as needed
                var unbookedSlots = await _appointmentSlotRepository.GetUnbookedSlotsByDoctorAsync(
                    ophthalmologist.Id, 
                    today, 
                    cancellationToken);

                foreach (var slot in unbookedSlots)
                {
                    slot.UpdateCost(request.ConsultationFee.Value);
                    await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
                }
            }

            if (request.UserId.HasValue)
            {
                var (succeeded, errors) = await _identityService.UpdateUserProfileAsync(
                    request.UserId.Value,
                    request.FullName ?? string.Empty,
                    request.Phone,
                    null, null,
                    request.Address,
                    null, // CitizenId not updated from ophthalmologist profile
                    cancellationToken);

                if (!succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure(errors);
                }
            }

            await _ophthalmologistRepository.UpdateAsync(ophthalmologist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result.Success();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}