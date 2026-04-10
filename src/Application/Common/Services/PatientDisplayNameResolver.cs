using Application.Common.Interfaces;
using Domain.Entities.Users;

namespace Application.Common.Services;

/// <summary>
/// Centralised patient display-name resolution.
/// Walk-in patients → profile fields on Patient entity.
/// Registered patients → profile fetched from Identity service.
/// </summary>
public interface IPatientDisplayNameResolver
{
    /// <summary>
    /// Resolve display name for a single patient.
    /// </summary>
    Task<string> ResolveAsync(Patient patient, CancellationToken cancellationToken = default);

    /// <summary>
    /// Batch-resolve display names for multiple patients.
    /// Returns a dictionary keyed by Patient.Id.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, string>> ResolveManyAsync(
        IEnumerable<Patient> patients,
        CancellationToken cancellationToken = default);
}

public class PatientDisplayNameResolver : IPatientDisplayNameResolver
{
    private readonly IIdentityService _identityService;

    public PatientDisplayNameResolver(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> ResolveAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        if (patient.IsWalkIn)
        {
            return patient.FullName ?? $"Patient {patient.Id.ToString()[..8]}";
        }

        var user = await _identityService.GetUserByIdAsync(patient.UserId!.Value, cancellationToken);

        if (user is not null && !string.IsNullOrWhiteSpace(user.FullName))
        {
            return user.FullName;
        }

        if (user is not null && !string.IsNullOrWhiteSpace(user.Email))
        {
            return user.Email;
        }

        return $"Patient {patient.Id.ToString()[..8]}";
    }

    public async Task<IReadOnlyDictionary<Guid, string>> ResolveManyAsync(
        IEnumerable<Patient> patients,
        CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<Guid, string>();

        // Group by walk-in vs registered to avoid unnecessary Identity calls
        var patientList = patients.ToList();

        foreach (var patient in patientList.Where(p => p.IsWalkIn))
        {
            result[patient.Id] = patient.FullName ?? $"Patient {patient.Id.ToString()[..8]}";
        }

        var registeredPatients = patientList.Where(p => !p.IsWalkIn).ToList();

        foreach (var patient in registeredPatients)
        {
            var user = await _identityService.GetUserByIdAsync(patient.UserId!.Value, cancellationToken);

            if (user is not null && !string.IsNullOrWhiteSpace(user.FullName))
            {
                result[patient.Id] = user.FullName;
            }
            else if (user is not null && !string.IsNullOrWhiteSpace(user.Email))
            {
                result[patient.Id] = user.Email;
            }
            else
            {
                result[patient.Id] = $"Patient {patient.Id.ToString()[..8]}";
            }
        }

        return result;
    }
}
