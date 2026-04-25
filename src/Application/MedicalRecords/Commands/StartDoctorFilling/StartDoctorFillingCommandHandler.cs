using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.MedicalRecords.Commands.StartDoctorFilling;

public class StartDoctorFillingCommandHandler : IRequestHandler<StartDoctorFillingCommand, Result>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartDoctorFillingCommandHandler(
        IMedicalRecordRepository medicalRecordRepository,
        IUnitOfWork unitOfWork)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(StartDoctorFillingCommand request, CancellationToken cancellationToken)
    {
        var record = await _medicalRecordRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (record == null)
        {
            return Result.NotFound("Medical record not found.");
        }

        try
        {
            record.StartDoctorFilling();
            await _medicalRecordRepository.UpdateAsync(record, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
