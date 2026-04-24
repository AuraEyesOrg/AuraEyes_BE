using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Ophthalmologists.EmploymentTypeChangeRequests.Commands.CreateEmploymentTypeChangeRequest;

public class CreateEmploymentTypeChangeRequestCommandHandler : ICommandHandler<CreateEmploymentTypeChangeRequestCommand, Guid>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IOphthalmologistEmploymentTypeChangeRequestRepository _requestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmploymentTypeChangeRequestCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IOphthalmologistEmploymentTypeChangeRequestRepository requestRepository,
        IUnitOfWork unitOfWork)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _requestRepository = requestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateEmploymentTypeChangeRequestCommand request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
        {
            return Result<Guid>.NotFound($"Ophthalmologist '{request.OphthalmologistId}' was not found.");
        }

        if (ophthalmologist.EmploymentType == request.TargetEmploymentType)
        {
            return Result<Guid>.Conflict("Target employment type must be different from current employment type.");
        }

        var hasPending = await _requestRepository.HasPendingRequestAsync(
            request.OphthalmologistId,
            cancellationToken: cancellationToken);

        if (hasPending)
        {
            return Result<Guid>.Conflict("An existing pending employment type change request already exists.");
        }

        OphthalmologistEmploymentTypeChangeRequest changeRequest;
        try
        {
            changeRequest = OphthalmologistEmploymentTypeChangeRequest.Create(
                request.OphthalmologistId,
                ophthalmologist.EmploymentType,
                request.TargetEmploymentType,
                request.Reason);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }

        await _requestRepository.AddAsync(changeRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(changeRequest.Id);
    }
}
