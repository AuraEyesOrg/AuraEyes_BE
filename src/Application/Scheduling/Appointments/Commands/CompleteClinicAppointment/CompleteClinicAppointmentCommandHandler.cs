using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.CompleteClinicAppointment;

public class CompleteClinicAppointmentCommandHandler : ICommandHandler<CompleteClinicAppointmentCommand>
{
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteClinicAppointmentCommandHandler(
        IPatientVisitRepository patientVisitRepository,
        IUnitOfWork unitOfWork)
    {
        _patientVisitRepository = patientVisitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CompleteClinicAppointmentCommand request, CancellationToken cancellationToken)
    {
        var visit = await _patientVisitRepository.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        if (visit is null)
        {
            return Result.NotFound("Patient visit not found. Check-in is required first.");
        }

        try
        {
            visit.Complete(request.Notes);
            await _patientVisitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
