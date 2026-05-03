using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Constants;
using Application.MedicalRecords.Common;
using AutoMapper;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;
using MediatR;

namespace Application.MedicalRecords.Queries.GetMedicalRecordById;

public class GetMedicalRecordByIdQueryHandler : IRequestHandler<GetMedicalRecordByIdQuery, Result<MedicalRecordDto>>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetMedicalRecordByIdQueryHandler(
        IMedicalRecordRepository medicalRecordRepository, 
        IRepository<Patient> patientRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _patientRepository = patientRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<MedicalRecordDto>> Handle(GetMedicalRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var record = await _medicalRecordRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (record == null)
        {
            return Result<MedicalRecordDto>.NotFound("Medical record not found.");
        }

        // Security Check: Enforce ownership or staff permissions
        if (_currentUserService.UserId.HasValue)
        {
            var isStaff = _currentUserService.IsInRole(Roles.SystemAdmin) || 
                          _currentUserService.IsInRole(Roles.Ophthalmologist) || 
                          _currentUserService.IsInRole(Roles.ClinicStaff);

            if (!isStaff)
            {
                var patients = await _patientRepository.FindAsync(p => p.UserId == _currentUserService.UserId.Value, cancellationToken);
                var patient = patients.FirstOrDefault();

                if (patient == null || record.PatientId != patient.Id)
                {
                    return Result<MedicalRecordDto>.Forbidden("You are not authorized to view this medical record.");
                }
            }
        }
        else
        {
            return Result<MedicalRecordDto>.Unauthorized("User not authenticated.");
        }

        var dto = _mapper.Map<MedicalRecordDto>(record);
        return Result<MedicalRecordDto>.Success(dto);
    }
}
