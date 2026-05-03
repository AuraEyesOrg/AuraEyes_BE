using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Patients.Queries.GetRecentClinicPatients;

public record GetRecentClinicPatientsQuery : IQuery<List<RecentClinicPatientDto>>;

public class GetRecentClinicPatientsQueryHandler : IQueryHandler<GetRecentClinicPatientsQuery, List<RecentClinicPatientDto>>
{
    private readonly IRepository<Domain.Entities.Users.Patient> _patientRepository;
    private readonly IIdentityService _identityService;

    public GetRecentClinicPatientsQueryHandler(
        IRepository<Domain.Entities.Users.Patient> patientRepository,
        IIdentityService identityService)
    {
        _patientRepository = patientRepository;
        _identityService = identityService;
    }

    public async Task<Result<List<RecentClinicPatientDto>>> Handle(GetRecentClinicPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await _patientRepository.Query().AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Take(50)
            .ToListAsync(cancellationToken);

        var result = new List<RecentClinicPatientDto>();

        foreach (var p in patients)
        {
            string? name = p.FullName;
            string? email = null;
            string? phone = p.PhoneNumber;

            if (p.UserId.HasValue)
            {
                var user = await _identityService.GetUserByIdAsync(p.UserId.Value, cancellationToken);
                if (user != null)
                {
                    name = user.FullName;
                    email = user.Email;
                    // For phone/address we might need user details, but let's keep it simple for now
                }
            }

            result.Add(new RecentClinicPatientDto
            {
                Id = p.Id,
                Name = name ?? "Unknown",
                Age = CalculateAge(p.DateOfBirth),
                Gender = p.GenderId?.ToString() ?? "M",
                DateOfBirth = p.DateOfBirth.HasValue ? DateOnly.FromDateTime(p.DateOfBirth.Value) : null,
                CitizenId = p.CitizenId,
                Address = p.Address,
                Email = email,
                PhoneNumber = phone,
                IsWalkIn = p.IsWalkIn,
                Bmi = p.BMI.HasValue ? (double)p.BMI.Value : null,
                DiseaseHistory = p.DiseaseHistory,
                LastScreening = p.UpdatedAt ?? p.CreatedAt, 
                AiPrediction = "Healthy",
                Confidence = 0.95,
                Status = "reviewed",
                Priority = "low"
            });
        }

        return Result<List<RecentClinicPatientDto>>.Success(result);
    }

    private static int CalculateAge(DateTime? dob)
    {
        if (!dob.HasValue) return 0;
        var today = DateTime.UtcNow;
        int age = today.Year - dob.Value.Year;
        if (dob.Value.Date > today.AddYears(-age)) age--;
        return age;
    }
}

