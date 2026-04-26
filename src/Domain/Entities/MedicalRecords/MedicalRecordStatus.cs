namespace Domain.Entities.MedicalRecords;

public enum MedicalRecordStatus
{
    Draft = 1,
    ClinicFilling = 2,
    DoctorFilling = 3,
    Completed = 4,
    Locked = 5
}
