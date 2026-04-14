using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Ophthalmologists.LeaveRequests.Commands.CreateLeaveRequest;

public class CreateLeaveRequestCommandHandler : ICommandHandler<CreateLeaveRequestCommand, Guid>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IOphthalmologistLeaveRequestRepository _leaveRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLeaveRequestCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IOphthalmologistLeaveRequestRepository leaveRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _leaveRequestRepository = leaveRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
        {
            return Result<Guid>.NotFound($"Ophthalmologist '{request.OphthalmologistId}' was not found.");
        }

        if (ophthalmologist.EmploymentType != OphthalmologistEmploymentType.FullTime)
        {
            return Result<Guid>.Conflict("Only full-time ophthalmologists can submit leave requests.");
        }

        var hasOverlap = await _leaveRequestRepository.HasOverlappingActiveRequestAsync(
            request.OphthalmologistId,
            request.StartDate,
            request.EndDate,
            cancellationToken: cancellationToken);

        if (hasOverlap)
        {
            return Result<Guid>.Conflict("An overlapping pending or approved leave request already exists.");
        }

        OphthalmologistLeaveRequest leaveRequest;
        try
        {
            leaveRequest = OphthalmologistLeaveRequest.Create(
                request.OphthalmologistId,
                request.StartDate,
                request.EndDate,
                request.Reason);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }

        await _leaveRequestRepository.AddAsync(leaveRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(leaveRequest.Id);
    }
}
