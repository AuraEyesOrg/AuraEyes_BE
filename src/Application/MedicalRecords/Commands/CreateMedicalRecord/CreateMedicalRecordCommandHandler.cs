using Application.Common.Models;
using Domain.Entities.MedicalRecords;
using Domain.Repositories;
using MediatR;

namespace Application.MedicalRecords.Commands.CreateMedicalRecord;

public class CreateMedicalRecordCommandHandler : IRequestHandler<CreateMedicalRecordCommand, Result<Guid>>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly Domain.Common.IRepository<Domain.Entities.Users.Patient> _patientRepository;

    public CreateMedicalRecordCommandHandler(
        IMedicalRecordRepository medicalRecordRepository,
        Domain.Common.IRepository<Domain.Entities.Users.Patient> patientRepository)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _patientRepository = patientRepository;
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
        
        // Sync MRN to Patient level if not set
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient != null && string.IsNullOrEmpty(patient.MedicalRecordNumber))
        {
            patient.SetMedicalRecordNumber(request.MedicalRecordNumber);
            await _patientRepository.UpdateAsync(patient, cancellationToken);
        }
        
        return Result<Guid>.Success(record.Id);
    }
}
