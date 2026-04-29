using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Ophthalmologists.Commands.UploadCredentials;

public class UploadCredentialsCommandHandler : ICommandHandler<UploadCredentialsCommand>
{
    private readonly IOphthalmologistRepository _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly INotificationService _notificationService;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public UploadCredentialsCommandHandler(
        IOphthalmologistRepository repository,
        IFileStorageService fileStorageService,
        INotificationService notificationService,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
        _notificationService = notificationService;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UploadCredentialsCommand request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _repository.GetByIdAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null) return Result.NotFound("Ophthalmologist not found");

        foreach (var cert in request.Certificates)
        {
            if (cert.File is null || cert.File.Length == 0) continue;

            await using var stream = cert.File.OpenReadStream();
            var uploadedUrl = await _fileStorageService.SaveFileAsync(
                stream,
                cert.File.FileName,
                $"ophthalmologists/credentials/{ophthalmologist.UserId}",
                cancellationToken);

            var newCert = new Certificate(
                ophthalmologist.Id,
                cert.Type,
                cert.Name,
                cert.Type == CertificateType.Degree ? cert.DegreeLevel : null,
                cert.IssuingAuthority,
                DateTime.SpecifyKind(cert.IssuedDate, DateTimeKind.Utc),
                cert.ExpiryDate.HasValue ? DateTime.SpecifyKind(cert.ExpiryDate.Value, DateTimeKind.Utc) : null,
                uploadedUrl
            );
            ophthalmologist.AddCertificate(newCert);

            ophthalmologist.UpdateCredentialFiles(
                cert.Type == CertificateType.License ? uploadedUrl : null,
                cert.Type == CertificateType.Degree ? uploadedUrl : null
            );
        }

        await _repository.UpdateAsync(ophthalmologist, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
