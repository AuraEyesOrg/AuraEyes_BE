using Application.Common.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Ophthalmologists.Commands.OnboardOphthalmologist;

public class OnboardOphthalmologistCommand : ICommand<bool>
{
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? DateOfBirth { get; set; } // Changed to string
    public int? Gender { get; set; }
    public string? CitizenId { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? NewPassword { get; set; }
    public string? CurrentPassword { get; set; }

    public List<OnboardDegreeDto> Degrees { get; set; } = new();
    public List<OnboardLicenseDto> Licenses { get; set; } = new();
}

public class OnboardDegreeDto
{
    public string Name { get; set; } = string.Empty;
    public DegreeLevel DegreeLevel { get; set; }
    public string? IssuingInstitution { get; set; }
    public string IssuedDate { get; set; } = string.Empty; // Changed to string
    public IFormFile? File { get; set; }
}

public class OnboardLicenseDto
{
    public string Name { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    public string? IssuingAuthority { get; set; }
    public string? ScopeOfPractice { get; set; }
    public string IssuedDate { get; set; } = string.Empty; // Changed to string
    public string? ExpirationDate { get; set; } // Changed to string
    public IFormFile? File { get; set; }
}
