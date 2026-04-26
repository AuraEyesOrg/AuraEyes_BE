using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Financial;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.UnitTests.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Services;

public class ClinicVisitServiceTests
{
    private readonly Mock<IPatientVisitRepository> _patientVisitRepoMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
    private readonly Mock<IConsultationSessionRepository> _sessionRepoMock;
    private readonly Mock<IRepository<Patient>> _patientRepoMock;
    private readonly Mock<IOphthalmologistRepository> _ophthalRepoMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IChatHubService> _chatHubServiceMock;
    private readonly Mock<ILogger<ClinicVisitService>> _loggerMock;
    private readonly ClinicVisitService _service;

    public ClinicVisitServiceTests()
    {
        _patientVisitRepoMock = new Mock<IPatientVisitRepository>();
        _appointmentRepoMock = new Mock<IAppointmentRepository>();
        _sessionRepoMock = new Mock<IConsultationSessionRepository>();
        var context = CreateContext();
        _sessionRepoMock.Setup(r => r.Query()).Returns(context.ConsultationSessions);
        _patientVisitRepoMock.Setup(r => r.Query()).Returns(context.PatientVisits);
        _appointmentRepoMock.Setup(r => r.Query()).Returns(context.Appointments);
        _patientRepoMock = new Mock<IRepository<Patient>>();
        _patientRepoMock.Setup(r => r.Query()).Returns(context.Patients);
        _ophthalRepoMock = new Mock<IOphthalmologistRepository>();
        _ophthalRepoMock.Setup(r => r.Query()).Returns(context.Ophthalmologists);
        _notificationServiceMock = new Mock<INotificationService>();
        _chatHubServiceMock = new Mock<IChatHubService>();
        _loggerMock = new Mock<ILogger<ClinicVisitService>>();

        _service = new ClinicVisitService(
            _patientVisitRepoMock.Object,
            _appointmentRepoMock.Object,
            _sessionRepoMock.Object,
            _patientRepoMock.Object,
            _ophthalRepoMock.Object,
            _notificationServiceMock.Object,
            _chatHubServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ProcessPaymentCompletionAsync_WhenVisitFoundByAppointmentId_ShouldCompleteVisitAndAppointment()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var slotId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var appointment = new Appointment(patientId, slotId, 100000, PricingType.AutoAssign, doctorId);
        var appointmentId = appointment.Id;

        // Advance appointment to InProgress
        appointment.CheckIn();
        appointment.Start();

        var order = new Order(userId, 100000, 0, "Clinic Visit", appointmentId);

        var visit = PatientVisit.CreateFromAppointment(appointment);
        // Advance visit to WaitingForPayment
        visit.Start();
        visit.FinishConsultation("Diagnosis notes");

        _patientVisitRepoMock.Setup(r => r.GetByAppointmentIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        _appointmentRepoMock.Setup(r => r.GetByIdWithDetailsAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        _patientRepoMock.Setup(r => r.GetByIdAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Patient.CreateRegistered(userId));

        // Act
        await _service.ProcessPaymentCompletionAsync(order, "PayOS", CancellationToken.None);

        // Assert
        visit.Status.Should().Be(PatientVisitStatus.Completed);
        appointment.Status.Should().Be(AppointmentStatus.Completed);
        
        _patientVisitRepoMock.Verify(r => r.UpdateAsync(visit, It.IsAny<CancellationToken>()), Times.Once);
        _appointmentRepoMock.Verify(r => r.UpdateAsync(appointment, It.IsAny<CancellationToken>()), Times.Once);
        _sessionRepoMock.Verify(r => r.AddAsync(It.IsAny<ConsultationSession>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentCompletionAsync_WhenVisitNotFound_ShouldDoNothing()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), 100000, 0, "Visit Payment", Guid.NewGuid());

        _patientVisitRepoMock.Setup(r => r.GetByAppointmentIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PatientVisit)null);

        // Act
        await _service.ProcessPaymentCompletionAsync(order, "PayOS", CancellationToken.None);

        // Assert
        _patientVisitRepoMock.Verify(r => r.UpdateAsync(It.IsAny<PatientVisit>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessPaymentCompletionAsync_WhenVisitAlreadyCompleted_ShouldNotProcessAgain()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();
        var slotId = Guid.NewGuid();

        var appointment = new Appointment(patientId, slotId, 100000, PricingType.AutoAssign, doctorId);
        appointment.CheckIn();
        appointment.Start();

        var visit = PatientVisit.CreateFromAppointment(appointment);
        visit.Start();
        visit.FinishConsultation();
        visit.Complete("Already paid");

        var order = new Order(Guid.NewGuid(), 100000, 0, "Visit Payment", appointmentId);

        _patientVisitRepoMock.Setup(r => r.GetByAppointmentIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        // Act
        await _service.ProcessPaymentCompletionAsync(order, "PayOS", CancellationToken.None);

        // Assert
        _appointmentRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Never);
    }
 
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
}
