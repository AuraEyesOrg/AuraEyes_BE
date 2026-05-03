using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.GenerateSlots;

public class GenerateSlotsCommandHandler : ICommandHandler<GenerateSlotsCommand, int>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IScheduleTemplateRepository _scheduleTemplateRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IOphthalmologistLeaveRequestRepository _leaveRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenerateSlotsCommandHandler> _logger;

    public GenerateSlotsCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IScheduleTemplateRepository scheduleTemplateRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IOphthalmologistLeaveRequestRepository leaveRequestRepository,
        IUnitOfWork unitOfWork,
        ILogger<GenerateSlotsCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _scheduleTemplateRepository = scheduleTemplateRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _leaveRequestRepository = leaveRequestRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(GenerateSlotsCommand request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateRequest(request);
        if (!validationResult.IsSuccess) return Result<int>.Failure(validationResult.ErrorMessage);

        var template = await _scheduleTemplateRepository.GetByIdAsync(request.ScheduleTemplateId, cancellationToken);
        if (template is null) return Result<int>.NotFound($"Schedule template '{request.ScheduleTemplateId}' not found.");

        var existingDates = await GetExistingDatesAsync(request, cancellationToken);
        var slotsByDate = GeneratePotentialSlots(request, template, existingDates);
        var (doctors, doctorLeaves) = await GetDoctorsWithLeavesAsync(request, cancellationToken);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var slotsCreated = await CreateAndPersistSlotsAsync(slotsByDate, doctors, doctorLeaves, template, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            LogGenerationSuccess(slotsCreated, request);
            return Result<int>.Success(slotsCreated);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            LogGenerationError(ex, request);
            throw;
        }
    }

    private static Result ValidateRequest(GenerateSlotsCommand request)
    {
        if (request.ToDate < request.FromDate) return Result.Failure("ToDate must be greater than or equal to FromDate.");

        var maxDays = 90;
        if (request.ToDate.DayNumber - request.FromDate.DayNumber > maxDays) return Result.Failure($"Date range cannot exceed {maxDays} days.");

        return Result.Success();
    }

    private async Task<HashSet<DateOnly>> GetExistingDatesAsync(GenerateSlotsCommand request, CancellationToken cancellationToken)
    {
        if (!request.SkipExistingDates) return new HashSet<DateOnly>();

        var existingSlots = await _appointmentSlotRepository.GetByDateRangeAsync(request.ScheduleTemplateId, request.FromDate, request.ToDate, cancellationToken);
        return existingSlots.Select(s => s.Date).ToHashSet();
    }

    private static Dictionary<DateOnly, List<(TimeOnly Start, TimeOnly End)>> GeneratePotentialSlots(GenerateSlotsCommand request, ScheduleTemplate template, HashSet<DateOnly> existingDates)
    {
        var slotsByDate = new Dictionary<DateOnly, List<(TimeOnly Start, TimeOnly End)>>();
        var currentDate = request.FromDate;

        while (currentDate <= request.ToDate)
        {
            if (currentDate.DayOfWeek == template.DayOfWeek && !existingDates.Contains(currentDate))
            {
                var daySlots = GenerateDaySlots(template);
                if (daySlots.Count > 0) slotsByDate[currentDate] = daySlots;
            }
            currentDate = currentDate.AddDays(1);
        }
        return slotsByDate;
    }

    private static List<(TimeOnly Start, TimeOnly End)> GenerateDaySlots(ScheduleTemplate template)
    {
        var daySlots = new List<(TimeOnly Start, TimeOnly End)>();
        var slotStart = template.StartTime;
        while (slotStart.Add(TimeSpan.FromMinutes(template.SlotDuration)) <= template.EndTime)
        {
            var slotEnd = slotStart.Add(TimeSpan.FromMinutes(template.SlotDuration));
            daySlots.Add((slotStart, slotEnd));
            slotStart = slotEnd;
        }
        return daySlots;
    }

    private async Task<(List<Ophthalmologist> Doctors, Dictionary<Guid, List<OphthalmologistLeaveRequest>> Leaves)> GetDoctorsWithLeavesAsync(GenerateSlotsCommand request, CancellationToken cancellationToken)
    {
        var doctors = await _ophthalmologistRepository.GetAllAsync(cancellationToken);
        var doctorLeaves = new Dictionary<Guid, List<OphthalmologistLeaveRequest>>();

        foreach (var doc in doctors)
        {
            var leaves = await _leaveRequestRepository.GetApprovedOverlappingAsync(doc.Id, request.FromDate, request.ToDate, cancellationToken);
            doctorLeaves[doc.Id] = leaves.ToList();
        }
        return (doctors.ToList(), doctorLeaves);
    }

    private async Task<int> CreateAndPersistSlotsAsync(Dictionary<DateOnly, List<(TimeOnly Start, TimeOnly End)>> slotsByDate, List<Ophthalmologist> doctors, Dictionary<Guid, List<OphthalmologistLeaveRequest>> doctorLeaves, ScheduleTemplate template, CancellationToken cancellationToken)
    {
        var slotsCreated = 0;
        foreach (var kvp in slotsByDate.OrderBy(x => x.Key))
        {
            foreach (var timeWindow in kvp.Value)
            {
                if (doctors.Any())
                {
                    slotsCreated += await CreateSlotsForDoctorsAsync(kvp.Key, timeWindow, doctors, doctorLeaves, template, cancellationToken);
                }
                else
                {
                    await CreateGenericSlotAsync(kvp.Key, timeWindow, template, cancellationToken);
                    slotsCreated++;
                }
            }
        }
        return slotsCreated;
    }

    private async Task<int> CreateSlotsForDoctorsAsync(DateOnly date, (TimeOnly Start, TimeOnly End) timeWindow, List<Ophthalmologist> doctors, Dictionary<Guid, List<OphthalmologistLeaveRequest>> doctorLeaves, ScheduleTemplate template, CancellationToken cancellationToken)
    {
        var count = 0;
        foreach (var doctor in doctors)
        {
            if (!doctorLeaves[doctor.Id].Any(l => l.Overlaps(date, date)))
            {
                var slot = new AppointmentSlot(template.Id, date, timeWindow.Start, timeWindow.End, 1);
                slot.UpdateOphthalId(doctor.Id);
                slot.UpdateCost(doctor.ConsultationFee > 0 ? doctor.ConsultationFee : (template.Cost ?? 0));
                await _appointmentSlotRepository.AddAsync(slot, cancellationToken);
                count++;
            }
        }
        return count;
    }

    private async Task CreateGenericSlotAsync(DateOnly date, (TimeOnly Start, TimeOnly End) timeWindow, ScheduleTemplate template, CancellationToken cancellationToken)
    {
        var slot = new AppointmentSlot(template.Id, date, timeWindow.Start, timeWindow.End, template.MaxCapacity);
        await _appointmentSlotRepository.AddAsync(slot, cancellationToken);
    }

    private void LogGenerationSuccess(int slotsCreated, GenerateSlotsCommand request)
    {
        _logger.LogInformation("Generated {SlotsCreated} slots for template {TemplateId} from {FromDate} to {ToDate}", slotsCreated, request.ScheduleTemplateId, request.FromDate, request.ToDate);
    }

    private void LogGenerationError(Exception ex, GenerateSlotsCommand request)
    {
        _logger.LogError(ex, "Error generating slots for template {TemplateId} from {FromDate} to {ToDate}", request.ScheduleTemplateId, request.FromDate, request.ToDate);
    }
}
