using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Screening;

namespace Application.Feedback.Commands.CreateWebsiteFeedback;

public class CreateWebsiteFeedbackCommandHandler : ICommandHandler<CreateWebsiteFeedbackCommand, Guid>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IRepository<WebsiteFeedback> _websiteFeedbackRepository;
    private readonly IRepository<AiScreening> _aiScreeningRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWebsiteFeedbackCommandHandler(
        ICurrentUserService currentUser,
        IRepository<WebsiteFeedback> websiteFeedbackRepository,
        IRepository<AiScreening> aiScreeningRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUser = currentUser;
        _websiteFeedbackRepository = websiteFeedbackRepository;
        _aiScreeningRepository = aiScreeningRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateWebsiteFeedbackCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.ProfileId.HasValue)
            return Result<Guid>.Unauthorized("Patient must be logged in.");

        var patientId = _currentUser.ProfileId.Value;

        var hasAiUsage = await _aiScreeningRepository.ExistsAsync(
            x => x.PatientId == patientId && x.ProcessedAt.HasValue,
            cancellationToken);

        if (!hasAiUsage)
            return Result<Guid>.Failure("Website feedback can only be submitted after at least one AI analytics usage.");

        var feedback = new WebsiteFeedback(
            patientId,
            request.Rating,
            request.Category,
            request.Comment);

        await _websiteFeedbackRepository.AddAsync(feedback, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(feedback.Id);
    }
}
