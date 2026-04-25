using Application.Common.Interfaces;
using Application.Common.Models;
using Application.MedicalRecords.Common;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.MedicalRecords.Queries.GetMedicalRecords;

public record GetMedicalRecordsQuery : IQuery<PagedResult<MedicalRecordDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public MedicalRecordStatus? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? SearchTerm { get; init; }
}

public class GetMedicalRecordsQueryHandler : IQueryHandler<GetMedicalRecordsQuery, PagedResult<MedicalRecordDto>>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IMapper _mapper;

    public GetMedicalRecordsQueryHandler(IMedicalRecordRepository medicalRecordRepository, IMapper mapper)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<MedicalRecordDto>>> Handle(GetMedicalRecordsQuery request, CancellationToken cancellationToken)
    {
        var query = _medicalRecordRepository.Query()
            .Include(x => x.Patient)
            .AsNoTracking();

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= request.ToDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x => 
                x.MedicalRecordNumber.Contains(request.SearchTerm) || 
                x.Patient.FullName.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<MedicalRecordDto>>(items);

        return Result<PagedResult<MedicalRecordDto>>.Success(
            new PagedResult<MedicalRecordDto>(dtos, totalCount, request.PageNumber, request.PageSize));
    }
}
