using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.MedicalRecords.Commands.UpdateMedicalRecordClinical;

public class UpdateMedicalRecordClinicalCommandHandler : IRequestHandler<UpdateMedicalRecordClinicalCommand, Result>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMedicalRecordClinicalCommandHandler(
        IMedicalRecordRepository medicalRecordRepository,
        IUnitOfWork unitOfWork)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateMedicalRecordClinicalCommand request, CancellationToken cancellationToken)
    {
        var record = await _medicalRecordRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (record == null)
        {
            return Result.NotFound("Medical record not found.");
        }

        try
        {
            record.UpdateClinicalInfo(
                request.ClinicalDataJson, 
                request.FinalDiagnosis, 
                request.TreatmentPlan);

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
