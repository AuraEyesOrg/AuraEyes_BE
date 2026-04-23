namespace Domain.Enums;

/// <summary>
/// PricingType - Determines how the appointment cost is calculated.
/// </summary>
public enum PricingType
{
    /// <summary>System assigns a doctor later (base price).</summary>
    AutoAssign = 1,

    /// <summary>Patient selects a specific doctor (doctor's consultation fee).</summary>
    DoctorSelected = 2
}
