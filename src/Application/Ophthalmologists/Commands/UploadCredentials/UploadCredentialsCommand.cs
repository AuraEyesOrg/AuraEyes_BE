using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Domain.Enums;

namespace Application.Ophthalmologists.Commands.UploadCredentials;

public record UploadCredentialsCommand : ICommand
{
    public Guid OphthalmologistId { get; init; }
    public List<UploadCredentialItemDto> Certificates { get; init; } = new();
}

public class UploadCredentialItemDto
{
    public CertificateType Type { get; set; }
    public DegreeLevel? DegreeLevel { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IssuingAuthority { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public IFormFile File { get; set; } = null!;
}
