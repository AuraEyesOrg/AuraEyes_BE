using Domain.Entities.Scheduling;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class AppointmentSlotTests
{
    [Fact]
    public void Constructor_ValidInput_ShouldCreateAvailableSlot()
    {
        var slot = new AppointmentSlot(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
            new TimeOnly(9, 0),
            new TimeOnly(9, 30),
            2,
            150_000m);

        slot.Status.Should().Be(ScheduleStatus.Available);
        slot.MaxCapacity.Should().Be(2);
        slot.RemainingCapacity.Should().Be(2);
        slot.Cost.Should().Be(150_000m);
    }

    [Fact]
    public void Constructor_EndTimeBeforeStartTime_ShouldThrow()
    {
        var act = () => new AppointmentSlot(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)), new TimeOnly(10, 0), new TimeOnly(9, 30));

        act.Should().Throw<ArgumentException>()
            .WithMessage("End time must be after start time");
    }

    [Fact]
    public void Reserve_ValidInput_ShouldMoveToReserved()
    {
        var slot = new AppointmentSlot(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)), new TimeOnly(9, 0), new TimeOnly(9, 30));
        var expiration = DateTime.UtcNow.AddMinutes(30);

        slot.Reserve(Guid.NewGuid(), expiration);

        slot.Status.Should().Be(ScheduleStatus.Reserved);
        slot.ReservationExpireAt.Should().BeOnOrAfter(expiration.AddSeconds(-1));
    }

    [Fact]
    public void ConfirmReservation_WithDifferentPatient_ShouldThrow()
    {
        var slot = new AppointmentSlot(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)), new TimeOnly(9, 0), new TimeOnly(9, 30));
        slot.Reserve(Guid.NewGuid(), DateTime.UtcNow.AddMinutes(30));

        var act = () => slot.ConfirmReservation(Guid.NewGuid());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only the patient who reserved this slot can confirm it");
    }

    [Fact]
    public void ReleaseReservation_ShouldMakeSlotAvailable()
    {
        var patientId = Guid.NewGuid();
        var slot = new AppointmentSlot(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)), new TimeOnly(9, 0), new TimeOnly(9, 30));
        slot.Reserve(patientId, DateTime.UtcNow.AddMinutes(30));

        slot.ReleaseReservation();

        slot.Status.Should().Be(ScheduleStatus.Available);
        slot.ReservedBy.Should().BeNull();
    }

    [Fact]
    public void Block_AvailableSlot_ShouldMoveToBlocked()
    {
        var slot = new AppointmentSlot(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)), new TimeOnly(9, 0), new TimeOnly(9, 30));

        slot.Block();

        slot.Status.Should().Be(ScheduleStatus.Blocked);
    }

    [Fact]
    public void BookWithCapacity_WithinLimit_ShouldIncrementBookedCount()
    {
        var slot = new AppointmentSlot(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)), new TimeOnly(9, 0), new TimeOnly(9, 30), 2);

        slot.BookWithCapacity();
        slot.BookedCount.Should().Be(1);
        slot.RemainingCapacity.Should().Be(1);
    }

    [Fact]
    public void BookWithCapacity_AtLimit_ShouldThrow()
    {
        var slot = new AppointmentSlot(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)), new TimeOnly(9, 0), new TimeOnly(9, 30), 1);
        slot.BookWithCapacity();

        var act = () => slot.BookWithCapacity();

        act.Should().Throw<InvalidOperationException>()
                .WithMessage("Slot has reached maximum capacity");
    }

    [Fact]
    public void UpdateCapacity_BelowBookedCount_ShouldThrow()
    {
        var slot = new AppointmentSlot(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)), new TimeOnly(9, 0), new TimeOnly(9, 30), 2);
        slot.BookWithCapacity();
        slot.BookWithCapacity();

        var act = () => slot.UpdateCapacity(1);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot reduce capacity below current booked count (2)");
    }

    [Fact]
    public void Complete_BookedSlot_ShouldMoveToCompleted()
    {
        var slot = new AppointmentSlot(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)), new TimeOnly(9, 0), new TimeOnly(9, 30));
        slot.Book();

        slot.Complete();

        slot.Status.Should().Be(ScheduleStatus.Completed);
    }
}