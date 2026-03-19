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
    public void CreateVerification_WithDoctor_ShouldAssignDoctor()
    {
        var doctorId = Guid.NewGuid();

        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(), Guid.NewGuid(), 50m, doctorId);

        session.OphthalmologistId.Should().Be(doctorId);
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
    public void CreateVideoCall_PastAppointment_ShouldThrow()
    {
        var act = () => ConsultationSession.CreateVideoCall(
            Guid.NewGuid(), 100m, DateTime.UtcNow.AddMinutes(-5));

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Appointment time must be in the future*");
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

    [Fact]
    public void CreateClinicBooking_PastAppointment_ShouldThrow()
    {
        var act = () => ConsultationSession.CreateClinicBooking(
            Guid.NewGuid(), Guid.NewGuid(), 200m, DateTime.UtcNow.AddMinutes(-5));

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Appointment time must be in the future*");
    }

    #endregion

    #region State Transitions

    [Fact]
    public void Confirm_PendingSession_ShouldConfirm()
    {
        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(), Guid.NewGuid(), 50m);

        session.Confirm();

        session.Status.Should().Be(SessionStatus.Confirmed);
    }

    [Fact]
    public void Confirm_NonPendingSession_ShouldThrow()
    {
        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(), Guid.NewGuid(), 50m);
        session.Confirm();

        var act = () => session.Confirm();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending sessions can be confirmed");
    }

    [Fact]
    public void EndSession_AssignedDoctor_ShouldComplete()
    {
        var doctorId = Guid.NewGuid();
        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(), Guid.NewGuid(), 50m, doctorId);
        session.Confirm();

        session.EndSession(doctorId, "Completed normally");

        session.Status.Should().Be(SessionStatus.Completed);
        session.ChatStatus.Should().Be(ChatStatus.Archived);
        session.ClosedBy.Should().Be(doctorId);
        session.ClosingReason.Should().Be("Completed normally");
        session.ClosedAt.Should().NotBeNull();
    }

    [Fact]
    public void EndSession_WrongDoctor_ShouldThrow()
    {
        var doctorId = Guid.NewGuid();
        var wrongDoctorId = Guid.NewGuid();
        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(), Guid.NewGuid(), 50m, doctorId);
        session.Confirm();

        var act = () => session.EndSession(wrongDoctorId);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only the assigned ophthalmologist can end this session");
    }

    [Fact]
    public void Cancel_PendingSession_ShouldCancel()
    {
        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(), Guid.NewGuid(), 50m);
        var userId = Guid.NewGuid();

        session.Cancel(userId, "Changed mind");

        session.Status.Should().Be(SessionStatus.Cancelled);
        session.ChatStatus.Should().Be(ChatStatus.Archived);
        session.ClosedBy.Should().Be(userId);
        session.ClosingReason.Should().Be("Changed mind");
    }

    [Fact]
    public void Cancel_CompletedSession_ShouldThrow()
    {
        var doctorId = Guid.NewGuid();
        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(), Guid.NewGuid(), 50m, doctorId);
        session.Confirm();
        session.EndSession(doctorId);

        var act = () => session.Cancel(Guid.NewGuid());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot cancel a completed session");
    }

    #endregion

    #region Chat Operations

    [Fact]
    public void OpenChat_LockedSession_ShouldOpen()
    {
        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(), Guid.NewGuid(), 50m);

        session.OpenChat();

        session.ChatStatus.Should().Be(ChatStatus.Open);
    }

    [Fact]
    public void OpenChat_ArchivedSession_ShouldThrow()
    {
        var doctorId = Guid.NewGuid();
        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(), Guid.NewGuid(), 50m, doctorId);
        session.Confirm();
        session.EndSession(doctorId);

        var act = () => session.OpenChat();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot reopen an archived session");
    }

    #endregion

    #region Other Methods

    [Fact]
    public void AssignDoctor_ShouldSetOphthalmologistId()
    {
        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(), Guid.NewGuid(), 50m);
        var doctorId = Guid.NewGuid();

        session.AssignDoctor(doctorId);

        session.OphthalmologistId.Should().Be(doctorId);
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
    public void SetMeetingInfo_EmptyLink_ShouldThrow()
    {
        var session = ConsultationSession.CreateVideoCall(
            Guid.NewGuid(), 100m, DateTime.UtcNow.AddDays(1));

        var act = () => session.SetMeetingInfo("");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Meeting link cannot be empty*");
    }

    [Fact]
    public void ClearMeetingInfo_ShouldNullify()
    {
        var session = ConsultationSession.CreateVideoCall(
            Guid.NewGuid(), 100m, DateTime.UtcNow.AddDays(1),
            meetingLink: "https://meet.google.com/abc",
            calendarEventId: "cal-123");

        session.ClearMeetingInfo();

        session.MeetingLink.Should().BeNull();
        session.CalendarEventId.Should().BeNull();
    }

    [Fact]
    public void RecordActivity_ShouldUpdateLastActivityAt()
    {
        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(), Guid.NewGuid(), 50m);
        var before = DateTime.UtcNow;

        session.RecordActivity();

        session.LastActivityAt.Should().BeOnOrAfter(before);
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
