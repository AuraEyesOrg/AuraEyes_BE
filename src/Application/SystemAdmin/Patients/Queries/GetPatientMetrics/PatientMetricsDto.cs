namespace Application.SystemAdmin.Patients.Queries.GetPatientMetrics;

public class PatientMetricsDto
{
    public int TotalPatients { get; init; }
    public int RegisteredPatients { get; init; }
    public int WalkInPatients { get; init; }
    public int ActiveRegisteredPatients { get; init; }
    public int LockedRegisteredPatients { get; init; }
}
