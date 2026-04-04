using Domain.Entities.Consultation;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class ConsultationSessionTests
{
    #region Factory Methods

    [Fact]
    public void CreateVerification_ShouldCreateWithCorrectDefaults()
    {
        var patientId = Guid.NewGuid();
        var screeningId = Guid.NewGuid();

        var session = ConsultationSession.CreateVerification(patientId, screeningId, 50m);

        session.PatientId.Should().Be(patientId);
        session.AiScreeningId.Should().Be(screeningId);
        session.Price.Should().Be(50m);
        session.Type.Should().Be(ConsultationSessionType.Verification);
        session.Status.Should().Be(SessionStatus.Pending);
        session.ChatStatus.Should().Be(ChatStatus.Locked);
        session.OphthalmologistId.Should().BeNull();
    }

    [Fact]
    public void CreateVideoCall_ShouldCreateWithMemoOnlyChat()
    {
        var patientId = Guid.NewGuid();
        var appointmentTime = DateTime.UtcNow.AddDays(1);

        var session = ConsultationSession.CreateVideoCall(patientId, 100m, appointmentTime);

        session.PatientId.Should().Be(patientId);
        session.Price.Should().Be(100m);
        session.Type.Should().Be(ConsultationSessionType.VideoCall);
        session.Status.Should().Be(SessionStatus.Confirmed);
        session.ChatStatus.Should().Be(ChatStatus.MemoOnly);
        session.AppointmentTime.Should().Be(appointmentTime);
    }

    [Fact]
    public void CreateClinicBooking_ShouldCreateWithLockedChat()
    {
        var patientId = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var appointmentTime = DateTime.UtcNow.AddDays(1);

        var session = ConsultationSession.CreateClinicBooking(
            patientId, orgId, 200m, appointmentTime);

        session.PatientId.Should().Be(patientId);
        session.OrganisationId.Should().Be(orgId);
        session.Type.Should().Be(ConsultationSessionType.ClinicBooking);
        session.Status.Should().Be(SessionStatus.Pending);
        session.ChatStatus.Should().Be(ChatStatus.Locked);
        session.AppointmentTime.Should().Be(appointmentTime);
    }

    #endregion

    #region State Transitions

    [Fact]
    public void CompleteBySystem_AlreadyCompleted_ShouldRemainCompleted()
    {
        var doctorId = Guid.NewGuid();
        var session = ConsultationSession.CreateVerification(Guid.NewGuid(), Guid.NewGuid(), 50m, doctorId);
        session.Confirm();
        session.EndSession(doctorId);

        session.CompleteBySystem();

        session.Status.Should().Be(SessionStatus.Completed);
        session.ChatStatus.Should().Be(ChatStatus.Archived);
    }

    [Fact]
    public void CompleteBySystem_PendingSession_ShouldComplete()
    {
        var session = ConsultationSession.CreateVerification(Guid.NewGuid(), Guid.NewGuid(), 50m);

        session.CompleteBySystem("timeout");

        session.Status.Should().Be(SessionStatus.Completed);
        session.ChatStatus.Should().Be(ChatStatus.Archived);
        session.ClosingReason.Should().Be("timeout");
    }

    [Fact]
    public void Cancel_PendingSession_ShouldCancel()
    {
        var session = ConsultationSession.CreateVerification(Guid.NewGuid(), Guid.NewGuid(), 50m);
        var userId = Guid.NewGuid();

        session.Cancel(userId, "Changed mind");

        session.Status.Should().Be(SessionStatus.Cancelled);
        session.ChatStatus.Should().Be(ChatStatus.Archived);
        session.ClosedBy.Should().Be(userId);
        session.ClosingReason.Should().Be("Changed mind");
    }

    #endregion

    #region Chat & Metadata

    [Fact]
    public void OpenChat_PendingVerification_ShouldConfirmAndOpen()
    {
        var session = ConsultationSession.CreateVerification(Guid.NewGuid(), Guid.NewGuid(), 50m);

        session.OpenChat();

        session.Status.Should().Be(SessionStatus.Confirmed);
        session.ChatStatus.Should().Be(ChatStatus.Open);
    }

    [Fact]
    public void SetMeetingInfo_ValidLink_ShouldSet()
    {
        var session = ConsultationSession.CreateVideoCall(
            Guid.NewGuid(), 100m, DateTime.UtcNow.AddDays(1));

        session.SetMeetingInfo("https://meet.google.com/abc", "cal-123");

        session.MeetingLink.Should().Be("https://meet.google.com/abc");
        session.CalendarEventId.Should().Be("cal-123");
    }

    [Fact]
    public void RecordReminderSent_ShouldSetLastReminderSentAt()
    {
        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(), Guid.NewGuid(), 50m);

        session.RecordReminderSent();

        session.LastReminderSentAt.Should().NotBeNull();
    }

    #endregion
}
