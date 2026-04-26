using Application.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Application.OrganisationPatients.Commands.CreateWalkInPatient;

/// <summary>
/// Internal CQRS command. Never bind directly from HTTP body — use <see cref="CreateWalkInPatientRequest"/> instead.
/// </summary>
public record CreateWalkInPatientCommand : ICommand<CreateWalkInPatientResponse>
{
    public required string FullName { get; init; }
    public required DateTime DateOfBirth { get; init; }
    public required string Gender { get; init; }
    public string? Address { get; init; }
    public string? PhoneNumber { get; init; }
    public string? CitizenId { get; init; }
    public string? Email { get; init; }

    /// <summary>
    /// Base URL of the email-confirmation page on the frontend (injected by controller from config).
    /// Required when a real patient email is provided so a verification link can be sent.
    /// Example: "https://auraeyes.vn/confirm-email"
    /// </summary>
    public string? ConfirmationUrlBase { get; init; }
}

/// <summary>
/// HTTP request DTO for POST /api/organisations/patients/walk-in.
/// Validated by <see cref="CreateWalkInPatientCommandValidator"/> via MediatR pipeline.
/// </summary>
public sealed record CreateWalkInPatientRequest
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; init; }

    [Required]
    public string Gender { get; init; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; init; }

    [MaxLength(20)]
    [Phone]
    public string? PhoneNumber { get; init; }

    [MaxLength(20)]
    public string? CitizenId { get; init; }

    /// <summary>
    /// Optional. If provided, an account will be created with this real email
    /// and a confirmation link will be emailed to the patient.
    /// If omitted, a generated email will be used and credentials are returned in the response.
    /// </summary>
    [EmailAddress]
    public string? Email { get; init; }
}

public sealed record CreateWalkInPatientResponse
{
    public Guid PatientId { get; init; }
    public Guid UserId { get; init; }
    public string LoginEmail { get; init; } = string.Empty;
    public bool IsGeneratedEmail { get; init; }
    public bool EmailSent { get; init; }
    /// <summary>
    /// Null when a real email was provided AND email was sent successfully
    /// (patient should receive credentials via email and must verify before logging in).
    /// Populated when using a generated/fake email so staff can hand credentials to the patient.
    /// </summary>
    public string? TemporaryPassword { get; init; }
}
