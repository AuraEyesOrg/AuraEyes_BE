using Domain.Common;
using Domain.Entities.MedicalRecords;

namespace Domain.Repositories;

public interface IMedicalRecordRepository : IRepository<MedicalRecord>
{
    Task<MedicalRecord?> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<MedicalRecord?> GetByNumberAsync(string recordNumber, CancellationToken cancellationToken = default);
    Task<MedicalRecord?> GetByVisitIdAsync(Guid visitId, CancellationToken cancellationToken = default);
}
