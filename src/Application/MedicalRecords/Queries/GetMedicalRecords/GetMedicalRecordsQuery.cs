using System.Linq;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.MedicalRecords.Common;
using AutoMapper;
using Domain.Entities.MedicalRecords;
using Domain.Enums;
using Domain.Common;
using Domain.Entities.Users;
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
    private readonly IRepository<Patient> _patientRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetMedicalRecordsQueryHandler(
        IMedicalRecordRepository medicalRecordRepository, 
        IRepository<Patient> patientRepository,
        ICurrentUserService currentUserService,
        IIdentityService identityService,
        IMapper mapper)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _patientRepository = patientRepository;
        _currentUserService = currentUserService;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<MedicalRecordDto>>> Handle(GetMedicalRecordsQuery request, CancellationToken cancellationToken)
    {
        var query = _medicalRecordRepository.Query()
            .Include(x => x.Patient)
            .AsNoTracking();

        // Security Filter: If user is a Patient, they only see their own records
        if (_currentUserService.UserId.HasValue && 
            !_currentUserService.IsInRole("SystemAdmin") && 
            !_currentUserService.IsInRole("Ophthalmologist") && 
            !_currentUserService.IsInRole("ClinicStaff"))
        {
            var patients = await _patientRepository.FindAsNoTrackingAsync(p => p.UserId == _currentUserService.UserId.Value, cancellationToken);
            var patient = patients.FirstOrDefault();
            
            if (patient != null)
            {
                query = query.Where(x => x.PatientId == patient.Id);
            }
            else
            {
                // Patient profile not found for this user, return empty result
                return Result<PagedResult<MedicalRecordDto>>.Success(
                    new PagedResult<MedicalRecordDto>(new List<MedicalRecordDto>(), 0, request.PageNumber, request.PageSize));
            }
        }

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
                (x.Patient != null && x.Patient.FullName.Contains(request.SearchTerm)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Include(x => x.Patient)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<MedicalRecordDto>>(items);

        // Populate patient info for registered users from Identity
        var registeredPatients = items
            .Where(x => x.Patient != null && x.Patient.UserId.HasValue)
            .Select(x => x.Patient)
            .ToList();

        if (registeredPatients.Any())
        {
            var userIds = registeredPatients.Select(x => x.UserId!.Value).Distinct();
            var users = await _identityService.GetUsersByIdsAsync(userIds, cancellationToken);
            var userDict = users.ToDictionary(x => x.Id);

            foreach (var dto in dtos)
            {
                var originalItem = items.First(x => x.Id == dto.Id);
                if (originalItem.Patient?.UserId != null && userDict.TryGetValue(originalItem.Patient.UserId.Value, out var user))
                {
                    dto.Patient ??= new PatientDto();
                    dto.Patient.FullName = user.FullName;
                    dto.Patient.Phone = user.PhoneNumber ?? originalItem.Patient.PhoneNumber ?? "";
                }
            }
        }

        return Result<PagedResult<MedicalRecordDto>>.Success(
            new PagedResult<MedicalRecordDto>(dtos, totalCount, request.PageNumber, request.PageSize));
    }
}

