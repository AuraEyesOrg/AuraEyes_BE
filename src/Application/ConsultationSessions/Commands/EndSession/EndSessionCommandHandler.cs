using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.ConsultationSessions.Commands.EndSession;

public class EndSessionCommandHandler : ICommandHandler<EndSessionCommand>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EndSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(EndSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
            return Result.NotFound($"Session '{request.SessionId}' not found.");

        if (session.Status == SessionStatus.Completed)
            return Result.Failure("Session is already completed.");

        if (session.OphthalmologistId.HasValue && session.OphthalmologistId.Value != request.DoctorId)
            return Result.Forbidden("Only the assigned ophthalmologist can end this session.");

        session.EndSession(request.DoctorId);
        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
