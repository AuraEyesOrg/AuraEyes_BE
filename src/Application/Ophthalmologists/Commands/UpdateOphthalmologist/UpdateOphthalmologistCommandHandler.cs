using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.Commands.UpdateOphthalmologist;

public class UpdateOphthalmologistCommandHandler : ICommandHandler<UpdateOphthalmologistCommand>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOphthalmologistCommandHandler(IOphthalmologistRepository ophthalmologistRepository, IUnitOfWork unitOfWork)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateOphthalmologistCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.Id, cancellationToken);

            if (ophthalmologist == null)
            {
                return Result.Failure($"Ophthalmologist with ID {request.Id} not found");
            }

            ophthalmologist.UpdateProfile(request.Bio, request.YearsOfExperience);

            await _ophthalmologistRepository.UpdateAsync(ophthalmologist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error updating ophthalmologist: {ex.Message}");
        }
    }
}
