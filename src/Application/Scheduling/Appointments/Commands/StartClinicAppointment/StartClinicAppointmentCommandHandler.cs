using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.StartClinicAppointment;

public class StartClinicAppointmentCommandHandler : ICommandHandler<StartClinicAppointmentCommand>
{
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartClinicAppointmentCommandHandler(
        IPatientVisitRepository patientVisitRepository,
        IUnitOfWork unitOfWork)
    {
        _patientVisitRepository = patientVisitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(StartClinicAppointmentCommand request, CancellationToken cancellationToken)
    {
        var visit = await _patientVisitRepository.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        if (visit is null)
        {
            return Result.NotFound("Patient visit not found. Check-in is required first.");
        }

        try
        {
            visit.Start();
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
