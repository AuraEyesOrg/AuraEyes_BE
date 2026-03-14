using Application.Common.Interfaces;
using Application.SystemAdmin.Contracts.Common;

namespace Application.SystemAdmin.Contracts.Queries.GetContractById;

public record GetContractByIdQuery(Guid Id) : IQuery<ContractDetailDto>;
