namespace Domain.Enums;

/// <summary>
/// Type of a Healthcare Roadmap step (doctor-authored care plan timeline).
/// </summary>
public enum RoadmapStepType
{
    /// <summary>Follow-up visit at the clinic.</summary>
    FollowUp = 1,

    /// <summary>Eye / diagnostic test.</summary>
    Test = 2,

    /// <summary>Medication review.</summary>
    Medication = 3,

    /// <summary>Custom doctor instruction.</summary>
    Custom = 4
}
