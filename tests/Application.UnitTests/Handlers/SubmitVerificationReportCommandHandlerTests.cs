using Application.Common.Interfaces;
using Application.ConsultationSessions.Commands.SubmitVerificationReport;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Handlers;

public class SubmitVerificationReportCommandHandlerTests
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<MedicalDiagnosis> _diagnosisRepository;
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<PatientRoadmap> _roadmapRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IPatientRoadmapGenerationService _roadmapGenerationService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SubmitVerificationReportCommandHandler _handler;

    public SubmitVerificationReportCommandHandlerTests()
    {
        _sessionRepository = Substitute.For<IConsultationSessionRepository>();
        _diagnosisRepository = Substitute.For<IRepository<MedicalDiagnosis>>();
        _screeningRepository = Substitute.For<IRepository<AiScreening>>();
        _roadmapRepository = Substitute.For<IRepository<PatientRoadmap>>();
        _patientRepository = Substitute.For<IRepository<Patient>>();
        _roadmapGenerationService = Substitute.For<IPatientRoadmapGenerationService>();
        _notificationService = Substitute.For<INotificationService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _handler = new SubmitVerificationReportCommandHandler(
            _sessionRepository,
            _diagnosisRepository,
            _screeningRepository,
            _roadmapRepository,
            _patientRepository,
            _roadmapGenerationService,
            _notificationService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_VideoCallSession_ShouldPassTypeGuard()
    {
        // Arrange
        var session = ConsultationSession.CreateVideoCall(
            patientId: Guid.NewGuid(),
            price: 100m,
            appointmentTime: DateTime.UtcNow.AddHours(2));

        var command = new SubmitVerificationReportCommand
        {
            SessionId = session.Id,
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.3",
            ClinicalFindings = "Findings"
        };

        _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Session has no linked AI screening.");
    }

    [Fact]
    public async Task Handle_ClinicBookingSession_ShouldRejectType()
    {
        // Arrange
        var session = ConsultationSession.CreateClinicBooking(
            patientId: Guid.NewGuid(),
            organisationId: Guid.NewGuid(),
            price: 120m,
            appointmentTime: DateTime.UtcNow.AddHours(3));

        var command = new SubmitVerificationReportCommand
        {
            SessionId = session.Id,
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.3",
            ClinicalFindings = "Findings"
        };

        _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Only verification or video call sessions accept reports.");

        await _screeningRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}