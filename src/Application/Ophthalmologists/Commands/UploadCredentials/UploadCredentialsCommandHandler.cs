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

        var oldStatus = ophthalmologist.VerificationStatus;
        bool hasChanges = false;

        foreach (var cert in request.Certificates)
        {
            if (cert.File is null || cert.File.Length == 0) continue;

            await using var stream = cert.File.OpenReadStream();
            var uploadedUrl = await _fileStorageService.SaveFileAsync(
                stream,
                cert.File.FileName,
                $"credentials/{ophthalmologist.UserId}",
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

            // Call UpdateCredentialFiles to trigger status change to PendingUpdate
            ophthalmologist.UpdateCredentialFiles(
                cert.Type == CertificateType.License ? uploadedUrl : null,      
                cert.Type == CertificateType.Degree ? uploadedUrl : null        
            );

            hasChanges = true;
        }

        if (hasChanges)
        {
            ophthalmologist.SubmitCredentialReviewRequest();

            await _repository.UpdateAsync(ophthalmologist, cancellationToken);  
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (ophthalmologist.VerificationStatus is VerificationStatus.PendingVerification or VerificationStatus.PendingUpdate)
            {
                var verificationFlowType = ophthalmologist.VerificationStatus == VerificationStatus.PendingUpdate
                    ? "CredentialUpdateReview"
                    : "OnboardingVerification";
                var notificationTitle = ophthalmologist.VerificationStatus == VerificationStatus.PendingUpdate
                    ? "Chờ duyệt cập nhật chứng chỉ"
                    : "Chờ duyệt hồ sơ onboarding bác sĩ";
                var notificationMessage = ophthalmologist.VerificationStatus == VerificationStatus.PendingUpdate
                    ? "Một bác sĩ vừa cập nhật chứng chỉ. Vui lòng vào mục Verification để duyệt."
                    : "Một bác sĩ vừa nộp hồ sơ chứng chỉ onboarding. Vui lòng vào mục Verification để duyệt.";

                var (systemAdmins, _) = await _identityService.GetUsersAsync(roleFilter: "SystemAdmin", pageNumber: 1, pageSize: 1000, cancellationToken: cancellationToken);
                foreach (var admin in systemAdmins)
                {
                    await _notificationService.SendAsync(  
                        admin.Id,
                        notificationTitle,
                        notificationMessage,
                        NotificationType.SystemAlert,
                        payload: new
                        {
                            action = "verification_request_submitted",
                            verificationFlowType,
                            ophthalmologistId = ophthalmologist.Id,
                            previousStatus = oldStatus.ToString(),
                            currentStatus = ophthalmologist.VerificationStatus.ToString()
                        },
                        cancellationToken: cancellationToken,
                        referenceId: ophthalmologist.Id);
                }
            }
        }

        return Result.Success();
    }
}
