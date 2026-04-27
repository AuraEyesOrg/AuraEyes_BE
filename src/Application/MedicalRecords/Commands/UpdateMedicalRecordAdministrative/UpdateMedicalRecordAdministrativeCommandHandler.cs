using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.MedicalRecords.Commands.UpdateMedicalRecordAdministrative;

public class UpdateMedicalRecordAdministrativeCommandHandler : IRequestHandler<UpdateMedicalRecordAdministrativeCommand, Result>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMedicalRecordAdministrativeCommandHandler(
        IMedicalRecordRepository medicalRecordRepository,
        IUnitOfWork unitOfWork)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateMedicalRecordAdministrativeCommand request, CancellationToken cancellationToken)
    {
        var record = await _medicalRecordRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (record == null)
        {
            return Result.NotFound("Medical record not found.");
        }

        try
        {
            record.UpdateAdministrativeInfo(request.AdministrativeDataJson);
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
