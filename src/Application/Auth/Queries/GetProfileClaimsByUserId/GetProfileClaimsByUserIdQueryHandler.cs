using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Application.Common.Constants;

namespace Application.Auth.Queries.GetProfileClaimsByUserId;

public class GetProfileClaimsByUserIdQueryHandler
    : IQueryHandler<GetProfileClaimsByUserIdQuery, ProfileClaimsDto>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<Ophthalmologist> _ophthalmologistRepository;

    public GetProfileClaimsByUserIdQueryHandler(
        IRepository<Patient> patientRepository,
        IRepository<Ophthalmologist> ophthalmologistRepository)
    {
        _patientRepository = patientRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    public async Task<Result<ProfileClaimsDto>> Handle(
        GetProfileClaimsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Roles.Contains(Roles.Patient))
        {
            var patients = await _patientRepository.FindAsync(
                p => p.UserId == request.UserId,
                cancellationToken);

            var patient = patients.FirstOrDefault();
            if (patient is null)
                return Result<ProfileClaimsDto>.Success(new ProfileClaimsDto());

            return Result<ProfileClaimsDto>.Success(new ProfileClaimsDto
            {
                ProfileId = patient.Id,
            });
        }

        if (request.Roles.Contains(Roles.Ophthalmologist))
        {
            var doctors = await _ophthalmologistRepository.FindAsync(
                o => o.UserId == request.UserId,
                cancellationToken);

            var doctor = doctors.FirstOrDefault();
            if (doctor is null)
                return Result<ProfileClaimsDto>.Success(new ProfileClaimsDto());

            return Result<ProfileClaimsDto>.Success(new ProfileClaimsDto
            {
                ProfileId = doctor.Id,
            });
        }

        return Result<ProfileClaimsDto>.Success(new ProfileClaimsDto());
    }
}
