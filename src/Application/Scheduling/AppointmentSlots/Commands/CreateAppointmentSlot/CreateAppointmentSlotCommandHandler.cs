using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemSettings.Interfaces;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.CreateAppointmentSlot;

public class CreateAppointmentSlotCommandHandler : ICommandHandler<CreateAppointmentSlotCommand, Guid>
{
    private const int MaxRetryAttempts = 3;

    private readonly IAppointmentSlotRepository _repository;
    private readonly IScheduleTemplateRepository _templateRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly ISystemSettingService _settingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAppointmentSlotCommandHandler> _logger;

    public CreateAppointmentSlotCommandHandler(
        IAppointmentSlotRepository repository,
        IScheduleTemplateRepository templateRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        ISystemSettingService settingService,
        IUnitOfWork unitOfWork,
        ILogger<CreateAppointmentSlotCommandHandler> logger)
    {
        _repository = repository;
        _templateRepository = templateRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _settingService = settingService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        // Verify template exists
        var template = await _templateRepository.GetByIdAsync(request.ScheduleTemplateId, cancellationToken);
        if (template is null)
        {
            return Result<Guid>.NotFound($"Schedule template with ID '{request.ScheduleTemplateId}' was not found.");
        }

        Domain.Entities.Users.Ophthalmologist? ophthalmologist = null;
        if (template.OphthalId.HasValue)
        {
            ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(template.OphthalId.Value, cancellationToken);
            if (ophthalmologist is null)
            {
                return Result<Guid>.NotFound($"Ophthalmologist '{template.OphthalId.Value}' not found.");
            }

            if (ophthalmologist.EmploymentType == OphthalmologistEmploymentType.FullTime)
            {
                return Result<Guid>.Forbidden("Full-time ophthalmologists cannot manually create slots.");
            }
        }

        for (var attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var hasOverlap = await _repository.HasOverlappingSlotAsync(
                    request.Date,
                    request.StartTime,
                    request.EndTime,
                    cancellationToken: cancellationToken);

                if (hasOverlap)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<Guid>.Conflict("An overlapping appointment slot already exists for this date and time.");
                }

                if (ophthalmologist?.EmploymentType == OphthalmologistEmploymentType.PartTime)
                {
                    var quota = await GetPartTimeDailyQuotaAsync(cancellationToken);
                    var reserveResult = await _settingService.TryReservePartTimeSlotsAsync(
                        request.Date,
                        1,
                        quota,
                        cancellationToken);

                    if (!reserveResult.Success)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        _logger.LogWarning(
                            "Part-time slot quota exceeded on {Date}. Quota={Quota}, CurrentCount={CurrentCount}, Requested={Requested}",
                            request.Date,
                            reserveResult.Quota,
                            reserveResult.UsedSlots,
                            1);

                        return Result<Guid>.Conflict("Daily slot quota for part-time doctors has been reached");
                    }

                    var nearLimitThreshold = Math.Max(1, (int)Math.Ceiling(reserveResult.Quota * 0.1));
                    if (reserveResult.RemainingSlots <= nearLimitThreshold)
                    {
                        _logger.LogWarning(
                            "Part-time quota near limit on {Date}. Quota={Quota}, Used={Used}, Remaining={Remaining}",
                            request.Date,
                            reserveResult.Quota,
                            reserveResult.UsedSlots,
                            reserveResult.RemainingSlots);
                    }
                    else
                    {
                        _logger.LogInformation(
                            "Reserved part-time quota on {Date}. Quota={Quota}, Used={Used}, Remaining={Remaining}",
                            request.Date,
                            reserveResult.Quota,
                            reserveResult.UsedSlots,
                            reserveResult.RemainingSlots);
                    }
                }

                var slot = new AppointmentSlot(
                    request.ScheduleTemplateId,
                    request.Date,
                    request.StartTime,
                    request.EndTime,
                    template.MaxCapacity,
                    request.Cost,
                    SlotSource.Doctor);

                await _repository.AddAsync(slot, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<Guid>.Success(slot.Id);
            }
            catch (ConcurrencyException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                if (attempt == MaxRetryAttempts)
                {
                    return Result<Guid>.Conflict("Slot creation conflicted with another request. Please retry.");
                }

                _logger.LogWarning(
                    ex,
                    "Retrying slot creation due to concurrency conflict. Attempt {Attempt}/{MaxAttempts}",
                    attempt,
                    MaxRetryAttempts);

                await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt), cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        return Result<Guid>.Conflict("Slot creation conflicted with another request. Please retry.");
    }

    private async Task<int> GetPartTimeDailyQuotaAsync(CancellationToken cancellationToken)
    {
        var configured = await _settingService.GetSettingAsync(SystemSettingKeys.PartTimeMaxSlotsPerDay, cancellationToken);
        return int.TryParse(configured, out var value) && value > 0 ? value : 100;
    }
}
