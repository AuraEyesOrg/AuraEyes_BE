using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.MedicalRecords;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Patients.Queries.GetMedicalRecords;

public class PatientMedicalHistoryDto
{
    public string Mrn { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public List<MedicalRecordHistoryDto> Records { get; set; } = new();
}

public class MedicalRecordHistoryDto
{
    public Guid RecordId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Diagnosis { get; set; }
}

public class GetPatientMedicalRecordsQuery : IQuery<PatientMedicalHistoryDto>
{
    public string Mrn { get; set; } = string.Empty;
}

public class GetPatientMedicalRecordsQueryHandler : IQueryHandler<GetPatientMedicalRecordsQuery, PatientMedicalHistoryDto>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<MedicalRecord> _recordRepository;

    public GetPatientMedicalRecordsQueryHandler(
        IRepository<Patient> patientRepository,
        IRepository<MedicalRecord> recordRepository)
    {
        _patientRepository = patientRepository;
        _recordRepository = recordRepository;
    }

    public async Task<Result<PatientMedicalHistoryDto>> Handle(GetPatientMedicalRecordsQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.MedicalRecordNumber == request.Mrn, cancellationToken);

        if (patient == null)
            return Result<PatientMedicalHistoryDto>.Failure("Patient not found with the provided MRN");

        var records = await _recordRepository.Query()
            .AsNoTracking()
            .Where(r => r.PatientId == patient.Id)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new MedicalRecordHistoryDto
            {
                RecordId = r.Id,
                Date = r.CreatedAt,
                Status = r.Status.ToString(),
                // Assuming diagnosis is in ClinicalDataJson or AdministrativeDataJson
                // For now, mapping as placeholder or checking if there's a diagnosis field
                Diagnosis = "N/A" 
            })
            .ToListAsync(cancellationToken);

        return Result<PatientMedicalHistoryDto>.Success(new PatientMedicalHistoryDto
        {
            Mrn = patient.MedicalRecordNumber ?? string.Empty,
            PatientName = patient.FullName ?? "Registered User", // Would need Identity join for registered patients
            Records = records
        });
    }
}
