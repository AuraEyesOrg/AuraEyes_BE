using Application.Common.Models;
using Application.SystemAdmin.Ophthalmologists.Commands.NormalizeFullTimeSchedule;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Ophthalmologists.Commands.NormalizeAllFullTimeSchedules;

/// <summary>
/// Runs full-time schedule normalization for all full-time ophthalmologists and aggregates outcomes.
/// </summary>
public class NormalizeAllFullTimeSchedulesCommandHandler
    : IRequestHandler<NormalizeAllFullTimeSchedulesCommand, Result<NormalizeAllFullTimeSchedulesResultDto>>
{
    private const int DefaultBulkWindowDays = 7;
    private const int MaxAllowedWindowDays = 180;

    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly ISender _sender;
    private readonly ILogger<NormalizeAllFullTimeSchedulesCommandHandler> _logger;

    public NormalizeAllFullTimeSchedulesCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        ISender sender,
        ILogger<NormalizeAllFullTimeSchedulesCommandHandler> logger)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _sender = sender;
        _logger = logger;
    }

    public async Task<Result<NormalizeAllFullTimeSchedulesResultDto>> Handle(
        NormalizeAllFullTimeSchedulesCommand request,
        CancellationToken cancellationToken)
    {
        var windowDays = ResolveWindowDays(request.WindowDays);
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = fromDate.AddDays(windowDays - 1);

        var fullTimeOphthalmologistIds = await _ophthalmologistRepository
            .Query()
            .Where(x => x.EmploymentType == OphthalmologistEmploymentType.FullTime)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (fullTimeOphthalmologistIds.Count == 0)
        {
            return Result<NormalizeAllFullTimeSchedulesResultDto>.Success(new NormalizeAllFullTimeSchedulesResultDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                WindowDays = windowDays,
                TotalFullTimeOphthalmologists = 0,
                Processed = 0,
                Succeeded = 0,
                Failed = 0
            });
        }

        var failures = new List<NormalizeAllFullTimeSchedulesFailureDto>();

        var processed = 0;
        var succeeded = 0;
        var failed = 0;

        var totalTemplatesEnsured = 0;
        var totalTemplatesUpdated = 0;
        var totalTemplatesDeactivated = 0;
        var totalSlotsDeleted = 0;
        var totalSlotsCreated = 0;
        var totalSlotsProtected = 0;

        foreach (var ophthalmologistId in fullTimeOphthalmologistIds)
        {
            var result = await _sender.Send(
                new NormalizeFullTimeScheduleCommand
                {
                    OphthalmologistId = ophthalmologistId,
                    WindowDays = windowDays
                },
                cancellationToken);

            processed++;

            if (result.IsSuccess && result.Data is not null)
            {
                succeeded++;

                totalTemplatesEnsured += result.Data.TemplatesEnsured;
                totalTemplatesUpdated += result.Data.TemplatesUpdated;
                totalTemplatesDeactivated += result.Data.TemplatesDeactivated;
                totalSlotsDeleted += result.Data.SlotsDeleted;
                totalSlotsCreated += result.Data.SlotsCreated;
                totalSlotsProtected += result.Data.SlotsProtected;
            }
            else
            {
                failed++;
                failures.Add(new NormalizeAllFullTimeSchedulesFailureDto
                {
                    OphthalmologistId = ophthalmologistId,
                    Error = result.ErrorMessage
                });
            }
        }

        _logger.LogInformation(
            "Bulk normalize full-time schedules completed. Total={Total}, Processed={Processed}, Succeeded={Succeeded}, Failed={Failed}, WindowDays={WindowDays}, SlotsCreated={SlotsCreated}, SlotsDeleted={SlotsDeleted}",
            fullTimeOphthalmologistIds.Count,
            processed,
            succeeded,
            failed,
            windowDays,
            totalSlotsCreated,
            totalSlotsDeleted);

        return Result<NormalizeAllFullTimeSchedulesResultDto>.Success(new NormalizeAllFullTimeSchedulesResultDto
        {
            FromDate = fromDate,
            ToDate = toDate,
            WindowDays = windowDays,
            TotalFullTimeOphthalmologists = fullTimeOphthalmologistIds.Count,
            Processed = processed,
            Succeeded = succeeded,
            Failed = failed,
            TotalTemplatesEnsured = totalTemplatesEnsured,
            TotalTemplatesUpdated = totalTemplatesUpdated,
            TotalTemplatesDeactivated = totalTemplatesDeactivated,
            TotalSlotsDeleted = totalSlotsDeleted,
            TotalSlotsCreated = totalSlotsCreated,
            TotalSlotsProtected = totalSlotsProtected,
            Failures = failures
        });
    }

    private static int ResolveWindowDays(int? requestedWindowDays)
    {
        if (!requestedWindowDays.HasValue)
        {
            return DefaultBulkWindowDays;
        }

        if (requestedWindowDays.Value < 1 || requestedWindowDays.Value > MaxAllowedWindowDays)
        {
            return DefaultBulkWindowDays;
        }

        return requestedWindowDays.Value;
    }
}
