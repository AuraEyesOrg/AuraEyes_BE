using Application.Common.Interfaces;
using Application.SystemAdmin.Contracts.Common;

namespace Application.Ophthalmologists.Contracts.GetMyContract;

/// <summary>
/// Query to get the current ophthalmologist's contract.
/// </summary>
public record GetMyContractQuery(Guid UserId) : IQuery<ContractDetailDto>;
