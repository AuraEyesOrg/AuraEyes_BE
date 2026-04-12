using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Pricing.Interfaces;
using Application.SystemSettings.Interfaces;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.CreateAppointmentSlot;

public class CreateAppointmentSlotCommandHandler : ICommandHandler<CreateAppointmentSlotCommand, Guid>
{
    private readonly IAppointmentSlotRepository _repository;
    private readonly IScheduleTemplateRepository _templateRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IExperiencePricingService _experiencePricingService;
    private readonly ISystemSettingService _settingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAppointmentSlotCommandHandler> _logger;

    public CreateAppointmentSlotCommandHandler(
        IAppointmentSlotRepository repository,
        IScheduleTemplateRepository templateRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IExperiencePricingService experiencePricingService,
        ISystemSettingService settingService,
        IUnitOfWork unitOfWork,
        ILogger<CreateAppointmentSlotCommandHandler> logger)
    {
        _repository = repository;
        _templateRepository = templateRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _experiencePricingService = experiencePricingService;
        _settingService = settingService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var template = await _templateRepository.GetByIdAsync(request.ScheduleTemplateId, cancellationToken);
            if (template is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.NotFound($"Schedule template with ID '{request.ScheduleTemplateId}' was not found.");
            }

            Domain.Entities.Users.Ophthalmologist? ophthalmologist = null;
            if (template.OphthalId.HasValue)
            {
                ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(template.OphthalId.Value, cancellationToken);
                if (ophthalmologist is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<Guid>.NotFound($"Ophthalmologist '{template.OphthalId.Value}' not found.");
                }

                if (ophthalmologist.EmploymentType == OphthalmologistEmploymentType.FullTime)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<Guid>.Forbidden("Full-time ophthalmologists cannot manually create slots.");
                }

                if (ophthalmologist.EmploymentType == OphthalmologistEmploymentType.PartTime)
                {
                    var pricingValidation = await _experiencePricingService.ValidatePartTimeCostAsync(
                        ophthalmologist.Id,
                        request.Cost,
                        cancellationToken);

                    if (!pricingValidation.IsSuccess)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return pricingValidation.IsNotFound
                            ? Result<Guid>.NotFound(pricingValidation.ErrorMessage)
                            : pricingValidation.IsForbidden
                                ? Result<Guid>.Forbidden(pricingValidation.ErrorMessage)
                                : Result<Guid>.Failure(pricingValidation.ErrorMessage);
                    }
                }
            }

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
            _logger.LogWarning(
                ex,
                "Concurrency conflict while creating slot {ScheduleTemplateId} on {Date} ({StartTime}-{EndTime})",
                request.ScheduleTemplateId,
                request.Date,
                request.StartTime,
                request.EndTime);

            return Result<Guid>.Conflict("Slot creation conflicted with another request. Please retry.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(
                ex,
                "Error creating slot for template {ScheduleTemplateId} on {Date}",
                request.ScheduleTemplateId,
                request.Date);
            throw;
        }
    }

    private async Task<int> GetPartTimeDailyQuotaAsync(CancellationToken cancellationToken)
    {
        var configured = await _settingService.GetSettingAsync("PART_TIME_MAX_SLOTS_PER_DAY", cancellationToken);
        return int.TryParse(configured, out var value) && value > 0 ? value : 100;
    }
}
