using Application.Common.Models;
using Application.MedicalRecords.Common;
using Domain.Repositories;
using MediatR;
using AutoMapper;

namespace Application.MedicalRecords.Queries.GetPatientMedicalRecords;

public record GetPatientMedicalRecordsQuery(Guid PatientId) : IRequest<Result<List<MedicalRecordDto>>>;

public class GetPatientMedicalRecordsQueryHandler : IRequestHandler<GetPatientMedicalRecordsQuery, Result<List<MedicalRecordDto>>>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IMapper _mapper;

    public GetPatientMedicalRecordsQueryHandler(IMedicalRecordRepository medicalRecordRepository, IMapper mapper)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<MedicalRecordDto>>> Handle(GetPatientMedicalRecordsQuery request, CancellationToken cancellationToken)
    {
        // Actually I should have a method in IMedicalRecordRepository to get list by patient
        // For now I'll use a generic approach if possible or I'll need to add it to the repository
        
        // Let's assume we can get all and filter, but better to have it in repository.
        // I saw IMedicalRecordRepository has GetByPatientIdAsync which returns a single record.
        // In this integrated flow, a patient might have multiple records over time.
        
        var records = await _medicalRecordRepository.ListAsync(cancellationToken);
        var patientRecords = records.Where(x => x.PatientId == request.PatientId && !x.IsDeleted)
                                    .OrderByDescending(x => x.CreatedAt)
                                    .ToList();

        return Result<List<MedicalRecordDto>>.Success(_mapper.Map<List<MedicalRecordDto>>(patientRecords));
    }
}
