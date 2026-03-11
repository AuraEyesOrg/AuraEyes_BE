using Domain.Entities.Scheduling;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class ScheduleTests
{
    private static Schedule CreateValidSchedule(
        ScheduleStatus? desiredStatus = null,
        decimal? cost = null)
    {
        var schedule = new Schedule(
            availableSlotId: Guid.NewGuid(),
            patientId: Guid.NewGuid(),
            date: DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            startTime: new TimeOnly(9, 0),
            endTime: new TimeOnly(10, 0),
            slotType: SlotType.Consultation,
            cost: cost);

        if (desiredStatus == ScheduleStatus.Booked)
            schedule.Book();

        return schedule;
    }

    [Fact]
    public void Constructor_ValidInput_ShouldCreateAvailableSchedule()
    {
        var slotId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var schedule = new Schedule(slotId, patientId, date,
            new TimeOnly(9, 0), new TimeOnly(10, 0), SlotType.Consultation, 50m);

        schedule.AvailableSlotId.Should().Be(slotId);
        schedule.PatientId.Should().Be(patientId);
        schedule.Date.Should().Be(date);
        schedule.StartTime.Should().Be(new TimeOnly(9, 0));
        schedule.EndTime.Should().Be(new TimeOnly(10, 0));
        schedule.SlotType.Should().Be(SlotType.Consultation);
        schedule.Cost.Should().Be(50m);
        schedule.Status.Should().Be(ScheduleStatus.Available);
    }

    [Fact]
    public void Constructor_EndTimeBeforeStartTime_ShouldThrow()
    {
        var act = () => new Schedule(
            Guid.NewGuid(), Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            new TimeOnly(10, 0), new TimeOnly(9, 0),
            SlotType.Consultation);

        act.Should().Throw<ArgumentException>()
            .WithMessage("End time must be after start time");
    }

    [Fact]
    public void Constructor_EqualStartAndEndTime_ShouldThrow()
    {
        var act = () => new Schedule(
            Guid.NewGuid(), Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            new TimeOnly(10, 0), new TimeOnly(10, 0),
            SlotType.Consultation);

        act.Should().Throw<ArgumentException>()
            .WithMessage("End time must be after start time");
    }

    [Fact]
    public void Book_AvailableSchedule_ShouldBook()
    {
        var schedule = CreateValidSchedule();

        schedule.Book();

        schedule.Status.Should().Be(ScheduleStatus.Booked);
    }

    [Fact]
    public void Book_AlreadyBookedSchedule_ShouldThrow()
    {
        var schedule = CreateValidSchedule(ScheduleStatus.Booked);

        var act = () => schedule.Book();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Schedule is not available for booking");
    }

    [Fact]
    public void Cancel_ShouldSetCancelledStatus()
    {
        var schedule = CreateValidSchedule(ScheduleStatus.Booked);

        schedule.Cancel();

        schedule.Status.Should().Be(ScheduleStatus.Cancelled);
    }

    [Fact]
    public void Complete_BookedSchedule_ShouldComplete()
    {
        var schedule = CreateValidSchedule(ScheduleStatus.Booked);

        schedule.Complete();

        schedule.Status.Should().Be(ScheduleStatus.Completed);
    }

    [Fact]
    public void Complete_AvailableSchedule_ShouldThrow()
    {
        var schedule = CreateValidSchedule();

        var act = () => schedule.Complete();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only booked schedules can be completed");
    }

    [Fact]
    public void MarkNoShow_BookedSchedule_ShouldMarkNoShow()
    {
        var schedule = CreateValidSchedule(ScheduleStatus.Booked);

        schedule.MarkNoShow();

        schedule.Status.Should().Be(ScheduleStatus.NoShow);
    }

    [Fact]
    public void MarkNoShow_AvailableSchedule_ShouldThrow()
    {
        var schedule = CreateValidSchedule();

        var act = () => schedule.MarkNoShow();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only booked schedules can be marked as no-show");
    }

    [Fact]
    public void UpdateCost_ShouldSetNewCost()
    {
        var schedule = CreateValidSchedule();

        schedule.UpdateCost(75.50m);

        schedule.Cost.Should().Be(75.50m);
    }

    [Fact]
    public void UpdateCost_Null_ShouldClearCost()
    {
        var schedule = CreateValidSchedule(cost: 50m);

        schedule.UpdateCost(null);

        schedule.Cost.Should().BeNull();
    }
}
