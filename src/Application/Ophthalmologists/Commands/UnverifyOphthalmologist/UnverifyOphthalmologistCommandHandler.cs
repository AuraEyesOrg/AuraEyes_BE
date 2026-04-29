using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.Commands.UnverifyOphthalmologist;

/// <summary>
/// Handler for UnverifyOphthalmologistCommand.
/// </summary>
public class UnverifyOphthalmologistCommandHandler : ICommandHandler<UnverifyOphthalmologistCommand>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnverifyOphthalmologistCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IUnitOfWork unitOfWork)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UnverifyOphthalmologistCommand request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.Id, cancellationToken);

        if (ophthalmologist is null)
        {
            return Result.NotFound($"Ophthalmologist with ID '{request.Id}' was not found.");
        }

        await _ophthalmologistRepository.UpdateAsync(ophthalmologist, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
