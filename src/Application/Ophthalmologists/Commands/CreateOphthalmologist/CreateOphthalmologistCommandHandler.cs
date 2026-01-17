using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Ophthalmologists.Commands.CreateOphthalmologist;

public class CreateOphthalmologistCommandHandler : ICommandHandler<CreateOphthalmologistCommand, Guid>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOphthalmologistCommandHandler(IOphthalmologistRepository ophthalmologistRepository, IUnitOfWork unitOfWork)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateOphthalmologistCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if ophthalmologist already exists for this user
            var existingOphthalmologist = await _ophthalmologistRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (existingOphthalmologist != null)
            {
                return Result<Guid>.Failure($"Ophthalmologist already exists for user ID {request.UserId}");
            }

            var ophthalmologist = new Ophthalmologist(
                request.UserId,
                request.Bio,
                request.YearsOfExperience
            );

            await _ophthalmologistRepository.AddAsync(ophthalmologist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(ophthalmologist.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Error creating ophthalmologist: {ex.Message}");
        }
    }
}
