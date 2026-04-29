using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Ophthalmologists.Commands.UpdateOphthalmologist;

/// <summary>
/// Command to update an existing ophthalmologist profile.
/// </summary>
public record UpdateOphthalmologistCommand : ICommand
{
    /// <summary>
    /// The ID of the ophthalmologist to update.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Optional authenticated user ID for self-profile updates.
    /// When provided, user profile fields can also be updated.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Optional full name update for self-profile flow.
    /// </summary>
    public string? FullName { get; init; }

    /// <summary>
    /// Optional phone update for self-profile flow.
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// Optional address update for self-profile flow.
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    /// Updated bio/description.
    /// </summary>
    public string? Bio { get; init; }

    /// <summary>
    /// Optional employment type update.
    /// </summary>
    public OphthalmologistEmploymentType? EmploymentType { get; init; }

    /// <summary>
    /// Optional consultation fee update.
    /// </summary>
    public decimal? ConsultationFee { get; init; }
}
