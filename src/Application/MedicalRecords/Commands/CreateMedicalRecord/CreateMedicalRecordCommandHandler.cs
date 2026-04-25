using Application.Common.Models;
using Domain.Entities.MedicalRecords;
using Domain.Repositories;
using MediatR;

namespace Application.MedicalRecords.Commands.CreateMedicalRecord;

public class CreateMedicalRecordCommandHandler : IRequestHandler<CreateMedicalRecordCommand, Result<Guid>>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;

    public CreateMedicalRecordCommandHandler(IMedicalRecordRepository medicalRecordRepository)
    {
        _medicalRecordRepository = medicalRecordRepository;
    }

    public async Task<Result<Guid>> Handle(CreateMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        var record = new MedicalRecord(request.PatientId, request.MedicalRecordNumber);
        
        if (request.ConsultationSessionId.HasValue)
        {
            record.LinkToConsultation(request.ConsultationSessionId.Value);
        }

        if (!string.IsNullOrEmpty(request.AdministrativeDataJson))
        {
            record.UpdateAdministrativeInfo(request.AdministrativeDataJson);
        }

        await _medicalRecordRepository.AddAsync(record, cancellationToken);
        
        return Result<Guid>.Success(record.Id);
    }
}
