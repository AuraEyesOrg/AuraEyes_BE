using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.MedicalRecords;
using Domain.Repositories;
using MediatR;

namespace Application.MedicalRecords.Commands.CreateMedicalRecord;

public class CreateMedicalRecordCommandHandler : IRequestHandler<CreateMedicalRecordCommand, Result<Guid>>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly Domain.Common.IRepository<Domain.Entities.Users.Patient> _patientRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMedicalRecordCommandHandler(
        IMedicalRecordRepository medicalRecordRepository,
        Domain.Common.IRepository<Domain.Entities.Users.Patient> patientRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        var record = new MedicalRecord(request.PatientId, request.MedicalRecordNumber);
        
        if (request.ConsultationSessionId.HasValue)
        {
            record.LinkToConsultation(request.ConsultationSessionId.Value);
        }

        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        var administrativeDataJson = request.AdministrativeDataJson;

        // If no administrative data is provided, map it from the patient profile (First time initialization)
        if (string.IsNullOrWhiteSpace(administrativeDataJson) && patient != null)
        {
            string? fullName = patient.FullName;
            string? phone = patient.PhoneNumber;
            DateTime? dob = patient.DateOfBirth;
            string? gender = patient.GenderId switch { 1 => "Nam", 2 => "Nữ", _ => "Khác" };
            string? address = patient.Address;
            string? citizenId = patient.CitizenId;

            if (patient.UserId.HasValue)
            {
                var userDetails = await _identityService.GetUserDetailsAsync(patient.UserId.Value, cancellationToken);
                if (userDetails != null)
                {
                    fullName ??= userDetails.FullName;
                    phone ??= userDetails.PhoneNumber;
                    dob ??= userDetails.DateOfBirth;
                    gender = userDetails.Gender?.ToString() switch { "Male" => "Nam", "Female" => "Nữ", _ => gender };
                    address ??= userDetails.Address;
                    citizenId ??= userDetails.CitizenId;
                }
            }

            var initialData = new Dictionary<string, object>
            {
                ["fullName"] = fullName ?? "",
                ["phoneNumber"] = phone ?? "",
                ["birthDate"] = dob?.ToString("yyyy-MM-dd") ?? "",
                ["age"] = dob.HasValue ? (DateTime.Now.Year - dob.Value.Year).ToString() : "",
                ["gender"] = gender ?? "",
                ["address"] = address ?? "",
                ["citizenId"] = citizenId ?? "",
                ["ethnicity"] = "Kinh", // Default for VN context, can be edited
                ["nationality"] = "Việt Nam",
                ["job"] = "",
                ["workplace"] = "",
                ["objectType"] = "Thu phí",
                ["maYT"] = request.MedicalRecordNumber,
                ["khoa"] = "Mắt"
            };

            administrativeDataJson = System.Text.Json.JsonSerializer.Serialize(initialData);
        }

        if (!string.IsNullOrEmpty(administrativeDataJson))
        {
            record.UpdateAdministrativeInfo(administrativeDataJson);
        }

        await _medicalRecordRepository.AddAsync(record, cancellationToken);
        
        // Sync MRN to Patient level if not set
        if (patient != null && string.IsNullOrEmpty(patient.MedicalRecordNumber))
        {
            patient.SetMedicalRecordNumber(request.MedicalRecordNumber);
            await _patientRepository.UpdateAsync(patient, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<Guid>.Success(record.Id);
    }
}
