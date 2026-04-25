namespace Domain.Enums;

/// <summary>
/// Status of a Healthcare Roadmap step.
/// Note: Overdue is *derived* on read (PlannedDate &lt; today AND status == Upcoming) and is NOT persisted.
/// </summary>
public enum RoadmapStepStatus
{
    /// <summary>Step is scheduled in the future or for today and not yet completed.</summary>
    Upcoming = 1,

    /// <summary>Step has been completed.</summary>
    Completed = 2,

    /// <summary>Step has been cancelled by the doctor.</summary>
    Cancelled = 3
}
