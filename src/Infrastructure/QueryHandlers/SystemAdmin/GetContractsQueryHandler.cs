using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Contracts.Common;
using Application.SystemAdmin.Contracts.Queries.GetContracts;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity;

namespace Infrastructure.QueryHandlers.SystemAdmin;

public class GetContractsQueryHandler : IQueryHandler<GetContractsQuery, PagedResult<ContractDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetContractsQueryHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Result<PagedResult<ContractDto>>> Handle(
        GetContractsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Contracts
            .AsNoTracking()
            .Include(c => c.Template)
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(c => c.ContractNumber.ToLower().Contains(term));
        }

        if (request.UserId.HasValue)
            query = query.Where(c => c.UserId == request.UserId.Value);

        if (request.Status.HasValue)
            query = query.Where(c => c.Status == request.Status.Value);

        if (request.ContractType.HasValue)
            query = query.Where(c => c.Template != null && c.Template.Type == request.ContractType.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var contracts = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Batch-load user info
        var userIds = contracts.Select(c => c.UserId).Distinct().ToList();
        var users = await _userManager.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FullName, u.Email })
            .ToListAsync(cancellationToken);

        var userMap = users.ToDictionary(u => u.Id);
        var ophthalmologistMap = await _context.Ophthalmologists
            .AsNoTracking()
            .Where(o => userIds.Contains(o.UserId))
            .Select(o => new { o.UserId, o.CommissionRate, o.ActualMonthlySalary })
            .ToDictionaryAsync(o => o.UserId, cancellationToken);

        var items = contracts.Select(c =>
        {
            var user = userMap.TryGetValue(c.UserId, out var u) ? u : null;
            var ophthalmologist = ophthalmologistMap.TryGetValue(c.UserId, out var o) ? o : null;
            return new ContractDto
            {
                Id = c.Id,
                ContractNumber = c.ContractNumber,
                Status = c.Status.ToString(),
                TemplateId = c.TemplateId,
                TemplateTitle = c.Template?.Title ?? string.Empty,
                ContractType = c.Template?.Type.ToString() ?? string.Empty,
                UserId = c.UserId,
                UserFullName = user?.FullName ?? string.Empty,
                UserEmail = user?.Email ?? string.Empty,
                AiQuotaLimit = c.AiQuotaLimit,
                PlatformCommissionRate = c.PlatformCommissionRate,
                CommissionRate = ophthalmologist?.CommissionRate,
                ActualMonthlySalary = ophthalmologist?.ActualMonthlySalary,
                SignedDate = c.SignedDate,
                ScannedDocumentUrl = c.ScannedDocumentUrl,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            };
        }).ToList();

        return Result<PagedResult<ContractDto>>.Success(
            new PagedResult<ContractDto>(items, totalCount, request.PageNumber, request.PageSize));
    }
}
