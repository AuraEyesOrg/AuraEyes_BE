using Application.Common.Models;
using Application.MedicalRecords.Common;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.MedicalRecords.Queries.GetMedicalRecordById;

public class GetMedicalRecordByIdQueryHandler : IRequestHandler<GetMedicalRecordByIdQuery, Result<MedicalRecordDto>>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IMapper _mapper;

    public GetMedicalRecordByIdQueryHandler(IMedicalRecordRepository medicalRecordRepository, IMapper mapper)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _mapper = mapper;
    }

    public async Task<Result<MedicalRecordDto>> Handle(GetMedicalRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var record = await _medicalRecordRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (record == null)
        {
            return Result<MedicalRecordDto>.NotFound("Medical record not found.");
        }

        var dto = _mapper.Map<MedicalRecordDto>(record);
        return Result<MedicalRecordDto>.Success(dto);
    }
}
