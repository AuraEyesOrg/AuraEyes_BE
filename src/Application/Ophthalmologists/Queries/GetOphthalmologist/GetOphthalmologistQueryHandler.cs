using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Ophthalmologists.Queries.GetOphthalmologist;

/// <summary>
/// Handler for GetOphthalmologistQuery.
/// </summary>
public class GetOphthalmologistQueryHandler : IQueryHandler<GetOphthalmologistQuery, OphthalmologistDto>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IIdentityService _identityService;

    public GetOphthalmologistQueryHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IIdentityService identityService)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _identityService = identityService;
    }

    public async Task<Result<OphthalmologistDto>> Handle(GetOphthalmologistQuery request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdWithCertificatesAsync(request.Id, cancellationToken);

        if (ophthalmologist is null)
        {
            return Result<OphthalmologistDto>.NotFound($"Ophthalmologist with ID '{request.Id}' was not found.");
        }

        // Get user information
        var user = await _identityService.GetUserByIdAsync(ophthalmologist.UserId, cancellationToken);
        var userDetails = await _identityService.GetUserDetailsAsync(ophthalmologist.UserId, cancellationToken);

        var slots = await _appointmentSlotRepository.GetByOphthalmologistAsync(
            ophthalmologist.Id,
            DateOnly.FromDateTime(DateTime.UtcNow),
            null,
            Domain.Enums.ScheduleStatus.Available,
            cancellationToken);

        decimal? minPrice = slots.Any() ? slots.Min(s => s.Cost) : null;
        decimal? maxPrice = slots.Any() ? slots.Max(s => s.Cost) : null;

        var dto = new OphthalmologistDto
        {
            Id = ophthalmologist.Id,
            UserId = ophthalmologist.UserId,
            UserFullName = user?.FullName,
            UserEmail = user?.Email,
            UserPhoneNumber = userDetails?.PhoneNumber,
            UserAddress = userDetails?.Address,
            Bio = ophthalmologist.Bio,
            YearsOfExperience = ophthalmologist.YearsOfExperience,
            IsVerified = ophthalmologist.IsVerified,
            CreatedAt = ophthalmologist.CreatedAt,
            UpdatedAt = ophthalmologist.UpdatedAt,
            RatingAverage = ophthalmologist.RatingAverage,
            RatingCount = ophthalmologist.RatingCount,
            CommissionRate = ophthalmologist.CommissionRate,
            ActualMonthlySalary = ophthalmologist.ActualMonthlySalary,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Degrees = ophthalmologist.Certificates
                .Where(c => c.Type == CertificateType.Degree)
                .OrderByDescending(c => c.IssuedDate)
                .Select(c => new DegreeDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    DegreeLevel = c.DegreeLevel?.ToString(),
                    IssuingAuthority = c.IssuingAuthority,
                    IssuedDate = c.IssuedDate,
                    DegreeUrl = c.CertificateUrl,
                    Title = GetDegreeTitle(c.DegreeLevel),
                    Abbreviation = GetDegreeAbbreviation(c.DegreeLevel)
                })
                .ToList(),
            Certificates = ophthalmologist.Certificates
                .Where(c => c.Type == CertificateType.License)
                .Select(c => new CertificateDto
                {
                    Id = c.Id,
                    Type = c.Type.ToString(),
                    Name = c.Name,
                    DegreeLevel = c.DegreeLevel?.ToString(),
                    IssuingAuthority = c.IssuingAuthority,
                    IssuedDate = c.IssuedDate,
                    ExpiryDate = c.ExpiryDate,
                    CertificateUrl = c.CertificateUrl,
                    IsExpired = c.IsExpired
                })
                .ToList()
        };

        return Result<OphthalmologistDto>.Success(dto);
    }

    private static string? GetDegreeTitle(DegreeLevel? level)
    {
        return level switch
        {
            DegreeLevel.Bachelor => "Bachelor",
            DegreeLevel.Master => "Master",
            DegreeLevel.Doctor => "Doctor (PhD)",
            DegreeLevel.AssociateProfessor => "Associate Professor",
            DegreeLevel.Professor => "Professor",
            _ => null
        };
    }

    private static string? GetDegreeAbbreviation(DegreeLevel? level)
    {
        return level switch
        {
            DegreeLevel.Bachelor => "B.S.",
            DegreeLevel.Master => "M.S.",
            DegreeLevel.Doctor => "Ph.D.",
            DegreeLevel.AssociateProfessor => "Assoc. Prof.",
            DegreeLevel.Professor => "Prof.",
            _ => null
        };
    }
}
