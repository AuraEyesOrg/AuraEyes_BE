using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;

namespace Application.SystemAdmin.Organisations.Commands.UpdateMonthlyQuota;

public class UpdateOrganisationMonthlyQuotaCommandHandler
    : ICommandHandler<UpdateOrganisationMonthlyQuotaCommand>
{
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrganisationMonthlyQuotaCommandHandler(
        IRepository<Organisation> organisationRepository,
        IUnitOfWork unitOfWork)
    {
        _organisationRepository = organisationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateOrganisationMonthlyQuotaCommand request,
        CancellationToken cancellationToken)
    {
        var organisation = await _organisationRepository.GetByIdAsync(
            request.OrganisationId,
            cancellationToken);

        if (organisation is null)
            return Result.NotFound("Organisation not found.");

        organisation.UpdateMonthlyQuotaLimit(request.MonthlyQuotaLimit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
