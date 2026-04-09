using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Contracts.Common;
using Application.SystemAdmin.Contracts.Queries.GetContractById;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity;

namespace Infrastructure.QueryHandlers.SystemAdmin;

public class GetContractByIdQueryHandler : IQueryHandler<GetContractByIdQuery, ContractDetailDto>
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetContractByIdQueryHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Result<ContractDetailDto>> Handle(
        GetContractByIdQuery request,
        CancellationToken cancellationToken)
    {
        var contract = await _context.Contracts
            .AsNoTracking()
            .Include(c => c.Template)
            .Where(c => c.Id == request.Id && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (contract is null)
            return Result<ContractDetailDto>.NotFound($"Contract {request.Id} not found.");

        var user = await _userManager.FindByIdAsync(contract.UserId.ToString());
        var ophthalmologist = await _context.Ophthalmologists
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.UserId == contract.UserId, cancellationToken);

        var dto = new ContractDetailDto
        {
            Id = contract.Id,
            ContractNumber = contract.ContractNumber,
            Status = contract.Status.ToString(),
            TemplateId = contract.TemplateId,
            TemplateTitle = contract.Template?.Title ?? string.Empty,
            ContractType = contract.Template?.Type.ToString() ?? string.Empty,
            UserId = contract.UserId,
            UserFullName = user?.FullName ?? string.Empty,
            UserEmail = user?.Email ?? string.Empty,
            AiQuotaLimit = contract.AiQuotaLimit,
            MonthlyQuotaLimit = contract.MonthlyQuotaLimit,
            PlatformCommissionRate = contract.PlatformCommissionRate,
            CommissionRate = ophthalmologist?.CommissionRate,
            ActualMonthlySalary = ophthalmologist?.ActualMonthlySalary,
            SignedDate = contract.SignedDate,
            ScannedDocumentUrl = contract.ScannedDocumentUrl,
            SignedContent = contract.SignedContent,
            CreatedAt = contract.CreatedAt,
            UpdatedAt = contract.UpdatedAt
        };

        return Result<ContractDetailDto>.Success(dto);
    }
}
