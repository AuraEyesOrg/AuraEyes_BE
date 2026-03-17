using Application.Common.Models;
using Application.Common.Models.Auth;
using Application.SystemAdmin.Organisations.Common;

namespace Application.Common.Interfaces;

public interface IOrganisationOnboardingService
{
    Task<Result<OrganisationRegistrationResponse>> SubmitRequestAsync(
        RegisterOrganisationRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<OrganisationOnboardingRequestDto>>> GetRequestsAsync(
        CancellationToken cancellationToken = default);

    Task<Result<ApproveOrganisationOnboardingResult>> ApproveRequestAsync(
        Guid requestId,
        Guid approvedByUserId,
        CancellationToken cancellationToken = default);
}