using Domain.Entities.MedicalRecords;

namespace Application.ClinicQueue.Common;

/// <summary>
/// Single rule for “administrative ERM saved” used by queue flow, DTO flags, and screening creation.
/// </summary>
public static class ClinicAdministrativeErmGate
{
    /// <summary>
    /// True when staff has saved real administrative ERM content (not draft-only / not placeholder).
    /// </summary>
    public static bool IsSatisfied(MedicalRecord? medicalRecord)
    {
        if (medicalRecord is null)
            return false;

        if (medicalRecord.Status == MedicalRecordStatus.DraftAdmin)
            return false;

        var json = medicalRecord.AdministrativeDataJson?.Trim();
        if (string.IsNullOrWhiteSpace(json) || json == "null")
            return false;

        // Legacy: check-in used UpdateAdministrativeInfo("{}") which incorrectly left PendingClinical with empty JSON.
        if (json == "{}")
            return false;

        return true;
    }
}
