using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Ophthalmologists.Commands.UpdateCertificate;

public record UpdateCertificateCommand : ICommand
{
    public Guid OphthalmologistId { get; init; }
    public Guid CertificateId { get; init; }
    public string Name { get; init; } = default!;
    public string? DegreeLevel { get; init; }
    public string? IssuingAuthority { get; init; }
    public DateTime IssuedDate { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public IFormFile? File { get; init; }
}

public class UpdateCertificateCommandHandler : ICommandHandler<UpdateCertificateCommand>
{
    private readonly IOphthalmologistRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public UpdateCertificateCommandHandler(
        IOphthalmologistRepository repository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result> Handle(UpdateCertificateCommand request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _repository.GetByIdWithCertificatesAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null) return Result.NotFound("Ophthalmologist not found");

        var certificate = ophthalmologist.Certificates.FirstOrDefault(c => c.Id == request.CertificateId);
        if (certificate is null) return Result.NotFound("Certificate not found");

        string? newUrl = certificate.CertificateUrl;
        if (request.File != null)
        {
            await using var stream = request.File.OpenReadStream();
            newUrl = await _fileStorageService.SaveFileAsync(
                stream,
                request.File.FileName,
                $"ophthalmologists/credentials/{ophthalmologist.UserId}",
                cancellationToken);
        }

        certificate.UpdateCertificate(
            certificate.Type,
            request.Name,
            request.DegreeLevel != null ? Enum.Parse<Domain.Enums.DegreeLevel>(request.DegreeLevel) : certificate.DegreeLevel,
            request.IssuingAuthority,
            request.IssuedDate,
            request.ExpiryDate,
            newUrl,
            certificate.LicenseNumber,
            certificate.ScopeOfPractice,
            certificate.IssuingInstitution);

        await _repository.UpdateAsync(ophthalmologist, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
