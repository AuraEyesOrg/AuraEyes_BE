using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Ophthalmologists.Commands.DeleteCertificate;

public record DeleteCertificateCommand(Guid OphthalmologistId, Guid CertificateId) : ICommand;

public class DeleteCertificateCommandHandler : ICommandHandler<DeleteCertificateCommand>
{
    private readonly IOphthalmologistRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCertificateCommandHandler(IOphthalmologistRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCertificateCommand request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _repository.GetByIdWithCertificatesAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null) return Result.NotFound("Ophthalmologist not found");

        ophthalmologist.RemoveCertificate(request.CertificateId);

        await _repository.UpdateAsync(ophthalmologist, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
