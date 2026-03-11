using Application.Common.Interfaces;
using Application.SystemAdmin.ContractTemplates.Common;

namespace Application.SystemAdmin.ContractTemplates.Queries.GetContractTemplateById;

public record GetContractTemplateByIdQuery(Guid Id) : IQuery<ContractTemplateDetailDto>;
