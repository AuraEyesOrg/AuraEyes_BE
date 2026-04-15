using Domain.Entities.Contracts;
using Domain.Entities.Users;

namespace Application.SystemAdmin.Ophthalmologists.Interfaces;

public interface IOphthalmologistContractProvisioningService
{
    Task<Contract?> CreatePendingContractForEmploymentTypeAsync(
        Ophthalmologist ophthalmologist,
        CancellationToken cancellationToken = default);
}
