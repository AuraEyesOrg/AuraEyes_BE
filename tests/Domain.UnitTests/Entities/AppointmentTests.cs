using Domain.Entities.Scheduling;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class AppointmentTests
{
    [Fact]
    public void CreateOnlineConsultation_ValidInput_ShouldCreatePendingAppointment()
    {
        var patientId = Guid.NewGuid();
        var slotId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();

        var appointment = Appointment.CreateOnlineConsultation(
            patientId,
            slotId,
            doctorId,
            "Eye check",
            organisationId: null,
            isRetinalImagesShared: true,
            isAiResultShared: false);

        appointment.Type.Should().Be(AppointmentType.OnlineConsultation);
        appointment.PatientId.Should().Be(patientId);
        appointment.AppointmentSlotId.Should().Be(slotId);
        appointment.DoctorId.Should().Be(doctorId);
        appointment.Status.Should().Be(AppointmentStatus.Pending);
        appointment.VisitReason.Should().Be("Eye check");
        appointment.IsRetinalImagesShared.Should().BeTrue();
        appointment.IsAiResultShared.Should().BeFalse();
    }

    [Fact]
    public void CreateClinicVisit_ValidInput_ShouldCreatePendingAppointment()
    {
        var appointment = Appointment.CreateClinicVisit(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Clinic visit");

        appointment.Type.Should().Be(AppointmentType.ClinicVisit);
        appointment.DoctorId.Should().BeNull();
        appointment.Status.Should().Be(AppointmentStatus.Pending);
    }

    [Fact]
    public void Confirm_PendingAppointment_ShouldMoveToConfirmed()
    {
        var appointment = Appointment.CreateOnlineConsultation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        appointment.Confirm();

        appointment.Status.Should().Be(AppointmentStatus.Confirmed);
    }

    [Fact]
    public void Confirm_NonPendingAppointment_ShouldThrow()
    {
        var appointment = Appointment.CreateOnlineConsultation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        appointment.Confirm();

        var act = () => appointment.Confirm();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot confirm appointment with status Confirmed. Only pending appointments can be confirmed.");
    }

    [Fact]
    public void LinkConsultationSession_OnClinicVisit_ShouldThrow()
    {
        var appointment = Appointment.CreateClinicVisit(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var act = () => appointment.LinkConsultationSession(Guid.NewGuid());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("ConsultationSession can only be linked to online consultations.");
    }

    [Fact]
    public void CheckIn_ClinicVisit_ShouldMoveToCheckedIn()
    {
        var appointment = Appointment.CreateClinicVisit(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        appointment.CheckIn();

        appointment.Status.Should().Be(AppointmentStatus.CheckedIn);
        appointment.CheckedInAt.Should().NotBeNull();
    }

    [Fact]
    public void Start_OnlineConsultationWithDoctor_ShouldMoveToInProgress()
    {
        var appointment = Appointment.CreateOnlineConsultation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        appointment.Confirm();
        appointment.LinkConsultationSession(Guid.NewGuid());

        appointment.Start();

        appointment.Status.Should().Be(AppointmentStatus.InProgress);
        appointment.StartedAt.Should().NotBeNull();
    }

    [Fact]
    public void Complete_InProgressAppointment_ShouldFinishAndStoreNotes()
    {
        var appointment = Appointment.CreateClinicVisit(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        appointment.CheckIn();
        appointment.Start();

        appointment.Complete("done");

        appointment.Status.Should().Be(AppointmentStatus.Completed);
        appointment.Notes.Should().Be("done");
        appointment.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_CompletedAppointment_ShouldThrow()
    {
        var appointment = Appointment.CreateClinicVisit(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        appointment.CheckIn();
        appointment.Start();
        appointment.Complete();

        var act = () => appointment.Cancel(Guid.NewGuid(), "late");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot cancel a completed appointment.");
    }

    [Fact]
    public void UpdateSharingConsents_ShouldUpdateBothFlags()
    {
        var appointment = Appointment.CreateOnlineConsultation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        appointment.UpdateSharingConsents(true, true);

        appointment.IsRetinalImagesShared.Should().BeTrue();
        appointment.IsAiResultShared.Should().BeTrue();
    }
}