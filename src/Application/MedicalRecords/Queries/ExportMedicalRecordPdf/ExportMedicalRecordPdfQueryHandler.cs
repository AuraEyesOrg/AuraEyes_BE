using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.MedicalRecords;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.MedicalRecords.Queries.ExportMedicalRecordPdf;

public sealed class ExportMedicalRecordPdfQueryHandler
    : IQueryHandler<ExportMedicalRecordPdfQuery, MedicalRecordPdfFileDto>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;
    private readonly IMedicalRecordPdfService _medicalRecordPdfService;

    public ExportMedicalRecordPdfQueryHandler(
        IMedicalRecordRepository medicalRecordRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService,
        IMedicalRecordPdfService medicalRecordPdfService)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
        _medicalRecordPdfService = medicalRecordPdfService;
    }

    public async Task<Result<MedicalRecordPdfFileDto>> Handle(
        ExportMedicalRecordPdfQuery request,
        CancellationToken cancellationToken)
    {
        var record = await _medicalRecordRepository.GetByIdAsync(request.MedicalRecordId, cancellationToken);
        if (record is null)
            return Result<MedicalRecordPdfFileDto>.NotFound("Medical record not found.");

        if ((int)record.Status < (int)MedicalRecordStatus.Finalized)
            return Result<MedicalRecordPdfFileDto>.Failure("EMR must be finalized before downloading PDF.");

        var patient = await _patientRepository.GetByIdAsync(record.PatientId, cancellationToken);
        if (patient is null)
            return Result<MedicalRecordPdfFileDto>.NotFound("Patient profile not found.");

        string? gender = null;
        if (patient.UserId.HasValue)
        {
            var userDetails = await _identityService.GetUserDetailsAsync(patient.UserId.Value, cancellationToken);
            gender = userDetails?.Gender?.ToString();
        }

        gender ??= patient.GenderId switch
        {
            1 => "Male",
            2 => "Female",
            _ => "Other"
        };

        var pdfBytes = _medicalRecordPdfService.GenerateMedicalRecordPdf(new MedicalRecordPdfModel
        {
            MedicalRecordNumber = record.MedicalRecordNumber,
            PatientName = patient.FullName ?? "Unknown",
            DateOfBirth = patient.DateOfBirth?.ToString("dd/MM/yyyy"),
            Gender = gender,
            Address = patient.Address,
            CreatedAt = record.CreatedAt,
            FinalDiagnosis = record.FinalDiagnosis,
            TreatmentPlan = record.TreatmentPlan,
            AdministrativeDataJson = record.AdministrativeDataJson,
            ClinicalDataJson = record.ClinicalDataJson
        });

        return Result<MedicalRecordPdfFileDto>.Success(new MedicalRecordPdfFileDto
        {
            Content = pdfBytes,
            ContentType = "application/pdf",
            FileName = $"EMR_{record.MedicalRecordNumber}.pdf"
        });
    }
}
