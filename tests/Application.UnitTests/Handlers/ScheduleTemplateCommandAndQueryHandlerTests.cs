using Application.Common.Models;
using Application.Scheduling.ScheduleTemplates.Commands.CreateScheduleTemplate;
using Application.Scheduling.ScheduleTemplates.Commands.DeleteScheduleTemplate;
using Application.Scheduling.ScheduleTemplates.Commands.UpdateScheduleTemplate;
using Application.Scheduling.ScheduleTemplates.Queries.GetScheduleTemplate;
using Application.Scheduling.ScheduleTemplates.Queries.GetScheduleTemplates;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Reflection;

namespace Application.UnitTests.Handlers;

public class ScheduleTemplateCommandAndQueryHandlerTests
{
    private static void AddSlotToTemplate(ScheduleTemplate template, AppointmentSlot slot)
    {
        var field = typeof(ScheduleTemplate).GetField("_appointmentSlots", BindingFlags.Instance | BindingFlags.NonPublic);
        var list = (List<AppointmentSlot>)field!.GetValue(template)!;
        list.Add(slot);
    }

    #region CreateScheduleTemplateCommandHandler

    [Fact]
    public async Task CreateScheduleTemplate_WhenNoOverlap_ShouldReturnId()
    {
        var repo = Substitute.For<IScheduleTemplateRepository>();
        var uow = Substitute.For<IUnitOfWork>();
        var handler = new CreateScheduleTemplateCommandHandler(repo, uow);
        var ophthalId = Guid.NewGuid();
        repo.HasOverlappingTemplateAsync(ophthalId, null, DayOfWeek.Monday, Arg.Any<TimeOnly>(), Arg.Any<TimeOnly>(), null, Arg.Any<CancellationToken>())
            .Returns(false);
        repo.AddAsync(Arg.Any<ScheduleTemplate>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<ScheduleTemplate>());

        var cmd = new CreateScheduleTemplateCommand
        {
            OphthalId = ophthalId,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 30,
            MaxCapacity = 2,
            Cost = 100
        };

        var result = await handler.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        await repo.Received(1).AddAsync(Arg.Any<ScheduleTemplate>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateScheduleTemplate_WhenOverlap_ShouldReturnConflict()
    {
        var repo = Substitute.For<IScheduleTemplateRepository>();
        var handler = new CreateScheduleTemplateCommandHandler(repo, Substitute.For<IUnitOfWork>());
        var ophthalId = Guid.NewGuid();
        repo.HasOverlappingTemplateAsync(Arg.Any<Guid?>(), Arg.Any<Guid?>(), Arg.Any<DayOfWeek>(), Arg.Any<TimeOnly>(), Arg.Any<TimeOnly>(), null, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await handler.Handle(
            new CreateScheduleTemplateCommand
            {
                OphthalId = ophthalId,
                DayOfWeek = DayOfWeek.Tuesday,
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(11, 0),
                SlotDuration = 30,
                MaxCapacity = 1
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.IsConflict.Should().BeTrue();
    }

    #endregion

    #region UpdateScheduleTemplateCommandHandler

    [Fact]
    public async Task UpdateScheduleTemplate_WhenNotFound_ShouldReturnNotFound()
    {
        var repo = Substitute.For<IScheduleTemplateRepository>();
        var handler = new UpdateScheduleTemplateCommandHandler(repo, Substitute.For<IUnitOfWork>());
        repo.GetByIdWithSlotsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ScheduleTemplate?)null);

        var result = await handler.Handle(
            new UpdateScheduleTemplateCommand
            {
                ScheduleTemplateId = Guid.NewGuid(),
                DayOfWeek = DayOfWeek.Wednesday,
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(12, 0),
                SlotDuration = 30,
                MaxCapacity = 1
            },
            CancellationToken.None);

        result.IsNotFound.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateScheduleTemplate_WhenNoOverlap_ShouldSucceed()
    {
        var repo = Substitute.For<IScheduleTemplateRepository>();
        var uow = Substitute.For<IUnitOfWork>();
        var handler = new UpdateScheduleTemplateCommandHandler(repo, uow);
        var ophthalId = Guid.NewGuid();
        var template = new ScheduleTemplate(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0), 30, 2, null, ophthalId);
        repo.GetByIdWithSlotsAsync(template.Id, Arg.Any<CancellationToken>()).Returns(template);
        repo.HasOverlappingTemplateAsync(ophthalId, null, DayOfWeek.Tuesday, Arg.Any<TimeOnly>(), Arg.Any<TimeOnly>(), template.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await handler.Handle(
            new UpdateScheduleTemplateCommand
            {
                ScheduleTemplateId = template.Id,
                DayOfWeek = DayOfWeek.Tuesday,
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(18, 0),
                SlotDuration = 30,
                MaxCapacity = 2,
                Cost = 50
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await repo.Received(1).UpdateAsync(template, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateScheduleTemplate_WhenOverlap_ShouldReturnConflict()
    {
        var repo = Substitute.For<IScheduleTemplateRepository>();
        var handler = new UpdateScheduleTemplateCommandHandler(repo, Substitute.For<IUnitOfWork>());
        var ophthalId = Guid.NewGuid();
        var template = new ScheduleTemplate(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0), 30, 2, null, ophthalId);
        repo.GetByIdWithSlotsAsync(template.Id, Arg.Any<CancellationToken>()).Returns(template);
        repo.HasOverlappingTemplateAsync(Arg.Any<Guid?>(), Arg.Any<Guid?>(), Arg.Any<DayOfWeek>(), Arg.Any<TimeOnly>(), Arg.Any<TimeOnly>(), template.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await handler.Handle(
            new UpdateScheduleTemplateCommand
            {
                ScheduleTemplateId = template.Id,
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0),
                SlotDuration = 30,
                MaxCapacity = 2
            },
            CancellationToken.None);

        result.IsConflict.Should().BeTrue();
    }

    #endregion

    #region DeleteScheduleTemplateCommandHandler

    [Fact]
    public async Task DeleteScheduleTemplate_WhenNotFound_ShouldReturnNotFound()
    {
        var repo = Substitute.For<IScheduleTemplateRepository>();
        var handler = new DeleteScheduleTemplateCommandHandler(repo, Substitute.For<IUnitOfWork>(), Substitute.For<ILogger<DeleteScheduleTemplateCommandHandler>>());
        repo.GetByIdWithSlotsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ScheduleTemplate?)null);

        var result = await handler.Handle(new DeleteScheduleTemplateCommand { ScheduleTemplateId = Guid.NewGuid() }, CancellationToken.None);

        result.IsNotFound.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteScheduleTemplate_WhenNoActiveSlots_ShouldSucceed()
    {
        var repo = Substitute.For<IScheduleTemplateRepository>();
        var uow = Substitute.For<IUnitOfWork>();
        var handler = new DeleteScheduleTemplateCommandHandler(repo, uow, Substitute.For<ILogger<DeleteScheduleTemplateCommandHandler>>());
        var template = new ScheduleTemplate(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0), 30, 2, null, Guid.NewGuid());
        repo.GetByIdWithSlotsAsync(template.Id, Arg.Any<CancellationToken>()).Returns(template);

        var result = await handler.Handle(new DeleteScheduleTemplateCommand { ScheduleTemplateId = template.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await repo.Received(1).DeleteAsync(template, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteScheduleTemplate_WhenActiveSlotsExist_ShouldReturnFailure()
    {
        var repo = Substitute.For<IScheduleTemplateRepository>();
        var handler = new DeleteScheduleTemplateCommandHandler(repo, Substitute.For<IUnitOfWork>(), Substitute.For<ILogger<DeleteScheduleTemplateCommandHandler>>());
        var template = new ScheduleTemplate(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0), 30, 2, null, Guid.NewGuid());
        var slot = new AppointmentSlot(template.Id, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), new TimeOnly(9, 0), new TimeOnly(9, 30));
        AddSlotToTemplate(template, slot);
        repo.GetByIdWithSlotsAsync(template.Id, Arg.Any<CancellationToken>()).Returns(template);

        var result = await handler.Handle(new DeleteScheduleTemplateCommand { ScheduleTemplateId = template.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        await repo.DidNotReceive().DeleteAsync(Arg.Any<ScheduleTemplate>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Query handlers

    [Fact]
    public async Task GetScheduleTemplate_WhenFound_ShouldReturnDto()
    {
        var repo = Substitute.For<IScheduleTemplateRepository>();
        var handler = new GetScheduleTemplateQueryHandler(repo);
        var template = new ScheduleTemplate(DayOfWeek.Friday, new TimeOnly(8, 0), new TimeOnly(12, 0), 30, 1, null, Guid.NewGuid());
        repo.GetByIdAsync(template.Id, Arg.Any<CancellationToken>()).Returns(template);

        var result = await handler.Handle(new GetScheduleTemplateQuery(template.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Id.Should().Be(template.Id);
    }

    [Fact]
    public async Task GetScheduleTemplate_WhenMissing_ShouldReturnNotFound()
    {
        var repo = Substitute.For<IScheduleTemplateRepository>();
        var handler = new GetScheduleTemplateQueryHandler(repo);
        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ScheduleTemplate?)null);

        var result = await handler.Handle(new GetScheduleTemplateQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsNotFound.Should().BeTrue();
    }

    [Fact]
    public async Task GetScheduleTemplates_WhenPopulated_ShouldReturnPaged()
    {
        var repo = Substitute.For<IScheduleTemplateRepository>();
        var handler = new GetScheduleTemplatesQueryHandler(repo);
        var t = new ScheduleTemplate(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(10, 0), 30, 1, null, Guid.NewGuid());
        repo.GetPagedAsync(null, null, null, 1, 10, Arg.Any<CancellationToken>())
            .Returns(([t], 1));

        var result = await handler.Handle(new GetScheduleTemplatesQuery { PageNumber = 1, PageSize = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetScheduleTemplates_WhenEmpty_ShouldReturnEmptyPage()
    {
        var repo = Substitute.For<IScheduleTemplateRepository>();
        var handler = new GetScheduleTemplatesQueryHandler(repo);
        repo.GetPagedAsync(null, null, null, 1, 10, Arg.Any<CancellationToken>())
            .Returns((Array.Empty<ScheduleTemplate>(), 0));

        var result = await handler.Handle(new GetScheduleTemplatesQuery { PageNumber = 1, PageSize = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }

    #endregion
}
