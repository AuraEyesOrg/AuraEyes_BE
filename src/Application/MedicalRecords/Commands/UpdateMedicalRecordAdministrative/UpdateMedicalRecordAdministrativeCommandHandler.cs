using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;
using MediatR;

namespace Application.MedicalRecords.Commands.UpdateMedicalRecordAdministrative;

public class UpdateMedicalRecordAdministrativeCommandHandler : IRequestHandler<UpdateMedicalRecordAdministrativeCommand, Result>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMedicalRecordAdministrativeCommandHandler(
        IMedicalRecordRepository medicalRecordRepository,
        IRepository<Patient> patientRepository,
        IUnitOfWork unitOfWork)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _patientRepository = patientRepository;
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
            
            // Update associated Patient address/profile if it's a walk-in or needs sync
            var patient = await _patientRepository.GetByIdAsync(record.PatientId, cancellationToken);
            if (patient != null)
            {
                var adminData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(request.AdministrativeDataJson);
                if (adminData != null)
                {
                    var street = adminData.GetValueOrDefault("address")?.ToString();
                    var ward = adminData.GetValueOrDefault("ward")?.ToString();
                    var district = adminData.GetValueOrDefault("district")?.ToString();
                    var province = adminData.GetValueOrDefault("province")?.ToString();

                    // Update full address string on patient
                    var addressComponents = new[] { street, ward, district, province }
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();
                    
                    var fullAddress = string.Join(", ", addressComponents);
                    
                    if (patient.IsWalkIn)
                    {
                        patient.UpdateWalkInProfile(
                            adminData.GetValueOrDefault("fullName")?.ToString() ?? patient.FullName!,
                            adminData.GetValueOrDefault("phoneNumber")?.ToString(),
                            adminData.GetValueOrDefault("citizenId")?.ToString(),
                            patient.DateOfBirth,
                            patient.GenderId,
                            fullAddress
                        );
                    }
                    else
                    {
                        // Even for regular patients, we might want to sync the address provided in the medical record
                        // if the system design allows it. For now, let's at least update the walk-in profile.
                    }
                    
                    await _patientRepository.UpdateAsync(patient, cancellationToken);
                }
            }

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
