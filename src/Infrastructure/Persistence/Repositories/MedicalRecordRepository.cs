using Domain.Entities.MedicalRecords;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MedicalRecordRepository : Repository<MedicalRecord>, IMedicalRecordRepository
{
    public MedicalRecordRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<MedicalRecord?> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(x => x.PatientId == patientId, cancellationToken);
    }

    public async Task<MedicalRecord?> GetByNumberAsync(string recordNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.MedicalRecordNumber == recordNumber, cancellationToken);
    }

    public async Task<MedicalRecord?> GetByVisitIdAsync(Guid visitId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.PatientVisitId == visitId, cancellationToken);
    }
}
