using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.OrganisationPatients.Queries.GetOrganisationRecentPatients;

public sealed class GetOrganisationRecentPatientsQueryHandler
    : IQueryHandler<GetOrganisationRecentPatientsQuery, IReadOnlyList<OrganisationRecentPatientDto>>
{
    private readonly IOrganisationPatientsRepository _organisationPatientsRepository;

    public GetOrganisationRecentPatientsQueryHandler(IOrganisationPatientsRepository organisationPatientsRepository)
    {
        _organisationPatientsRepository = organisationPatientsRepository;
    }

    public async Task<Result<IReadOnlyList<OrganisationRecentPatientDto>>> Handle(
        GetOrganisationRecentPatientsQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _organisationPatientsRepository.GetRecentPatientsForOrganisationAdminAsync(
            request.OrgAdminUserId,
            request.Take,
            cancellationToken);

        var items = rows.Select(r => new OrganisationRecentPatientDto
        {
            Id = r.Id,
            Name = r.Name,
            Age = r.Age,
            Gender = r.Gender,
            DateOfBirth = r.DateOfBirth,
            CitizenId = r.CitizenId,
            Address = r.Address,
            Email = r.Email,
            PhoneNumber = r.PhoneNumber,
            LastScreening = r.LastScreening,
            AiPrediction = r.AiPrediction,
            Confidence = r.Confidence,
            Status = r.Status,
            Priority = r.Priority
        }).ToList();

        return Result<IReadOnlyList<OrganisationRecentPatientDto>>.Success(items);
    }
}
