using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.Commands.UpdateOphthalmologist;

/// <summary>
/// Handler for UpdateOphthalmologistCommand.
/// </summary>
public class UpdateOphthalmologistCommandHandler : ICommandHandler<UpdateOphthalmologistCommand>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOphthalmologistCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IUnitOfWork unitOfWork)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateOphthalmologistCommand request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.Id, cancellationToken);

        if (ophthalmologist is null)
        {
            return Result.NotFound($"Ophthalmologist with ID '{request.Id}' was not found.");
        }

        // Update profile using domain method
        ophthalmologist.UpdateProfile(request.Bio, request.YearsOfExperience);

        await _ophthalmologistRepository.UpdateAsync(ophthalmologist, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
