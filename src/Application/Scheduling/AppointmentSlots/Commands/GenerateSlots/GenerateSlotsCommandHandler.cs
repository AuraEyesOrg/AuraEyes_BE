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
        // Validate date range
        if (request.ToDate < request.FromDate)
        {
            return Result<int>.Failure("ToDate must be greater than or equal to FromDate.");
        }

        // Limit date range to prevent excessive slot creation
        var maxDays = 90;
        if (request.ToDate.DayNumber - request.FromDate.DayNumber > maxDays)
        {
            return Result<int>.Failure($"Date range cannot exceed {maxDays} days.");
        }

        // Get the template
        var template = await _scheduleTemplateRepository.GetByIdAsync(request.ScheduleTemplateId, cancellationToken);
        if (template is null)
        {
            return Result<int>.NotFound($"Schedule template '{request.ScheduleTemplateId}' not found.");
        }

        // Get existing slots if we need to skip existing dates
        HashSet<DateOnly> existingDates = new();
        if (request.SkipExistingDates)
        {
            var existingSlots = await _appointmentSlotRepository.GetByDateRangeAsync(
                request.ScheduleTemplateId,
                request.FromDate,
                request.ToDate,
                cancellationToken);
            existingDates = existingSlots.Select(s => s.Date).ToHashSet();
        }

        var slotsByDate = new Dictionary<DateOnly, List<(TimeOnly Start, TimeOnly End)>>();
        var currentDate = request.FromDate;

        while (currentDate <= request.ToDate)
        {
            // Only generate slots for the template's day of week
            if (currentDate.DayOfWeek == template.DayOfWeek)
            {
                // Skip if date already has slots (and SkipExistingDates is true)
                if (!existingDates.Contains(currentDate))
                {
                    var slotStart = template.StartTime;
                    var daySlots = new List<(TimeOnly Start, TimeOnly End)>();
                    while (slotStart.Add(TimeSpan.FromMinutes(template.SlotDuration)) <= template.EndTime)
                    {
                        var slotEnd = slotStart.Add(TimeSpan.FromMinutes(template.SlotDuration));
                        daySlots.Add((slotStart, slotEnd));
                        slotStart = slotEnd;
                    }

                    if (daySlots.Count > 0)
                    {
                        slotsByDate[currentDate] = daySlots;
                    }
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        // Get verified doctors if it's a general clinic template
        IReadOnlyList<Ophthalmologist>? doctors = null;
        var doctorLeaves = new Dictionary<Guid, List<OphthalmologistLeaveRequest>>();

        if (template.OphthalId.HasValue)
        {
            var leaves = await _leaveRequestRepository.GetApprovedOverlappingAsync(
                template.OphthalId.Value, request.FromDate, request.ToDate, cancellationToken);
            doctorLeaves[template.OphthalId.Value] = leaves.ToList();
        }
        else
        {
            var allDoctors = await _ophthalmologistRepository.GetAllAsync(cancellationToken);
            doctors = allDoctors.Where(d => d.VerificationStatus != VerificationStatus.Rejected).ToList();
            
            foreach (var doc in doctors)
            {
                var leaves = await _leaveRequestRepository.GetApprovedOverlappingAsync(
                    doc.Id, request.FromDate, request.ToDate, cancellationToken);
                doctorLeaves[doc.Id] = leaves.ToList();
            }
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var slotsCreated = 0;

            foreach (var kvp in slotsByDate.OrderBy(x => x.Key))
            {
                foreach (var timeWindow in kvp.Value)
                {
                    if (template.OphthalId.HasValue)
                    {
                        var docId = template.OphthalId.Value;
                        var isOnLeave = doctorLeaves[docId].Any(l => l.Overlaps(kvp.Key, kvp.Key));

                        if (!isOnLeave)
                        {
                            var slot = new AppointmentSlot(
                                template.Id,
                                kvp.Key,
                                timeWindow.Start,
                                timeWindow.End,
                                template.MaxCapacity,
                                SlotSource.System);
                            
                            slot.UpdateOphthalId(docId);
                            await _appointmentSlotRepository.AddAsync(slot, cancellationToken);
                            slotsCreated++;
                        }
                    }
                    else if (doctors != null && doctors.Any())
                    {
                        foreach (var doctor in doctors)
                        {
                            var isOnLeave = doctorLeaves[doctor.Id].Any(l => l.Overlaps(kvp.Key, kvp.Key));

                            if (!isOnLeave)
                            {
                                var slot = new AppointmentSlot(
                                    template.Id,
                                    kvp.Key,
                                    timeWindow.Start,
                                    timeWindow.End,
                                    1, // Max capacity 1 per doctor
                                    SlotSource.System);
                                
                                slot.UpdateOphthalId(doctor.Id);
                                
                                // Set cost: Prioritize doctor's specific fee, then template's cost
                                var cost = doctor.ConsultationFee > 0 
                                    ? doctor.ConsultationFee 
                                    : (template.Cost ?? 0);
                                slot.UpdateCost(cost);

                                await _appointmentSlotRepository.AddAsync(slot, cancellationToken);
                                slotsCreated++;
                            }
                        }
                    }
                    else
                    {
                        // Fallback if no doctors are verified, just generate the generic slot
                        var slot = new AppointmentSlot(
                            template.Id,
                            kvp.Key,
                            timeWindow.Start,
                            timeWindow.End,
                            template.MaxCapacity,
                            SlotSource.System);

                        await _appointmentSlotRepository.AddAsync(slot, cancellationToken);
                        slotsCreated++;
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Generated {SlotsCreated} slots for template {TemplateId} from {FromDate} to {ToDate}",
                slotsCreated,
                request.ScheduleTemplateId,
                request.FromDate,
                request.ToDate);

            return Result<int>.Success(slotsCreated);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(
                ex,
                "Error generating slots for template {TemplateId} from {FromDate} to {ToDate}",
                request.ScheduleTemplateId,
                request.FromDate,
                request.ToDate);
            throw;
        }
    }
}
