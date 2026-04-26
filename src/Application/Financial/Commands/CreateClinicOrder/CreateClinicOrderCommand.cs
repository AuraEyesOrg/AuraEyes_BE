using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Financial.Commands.CreateClinicOrder;

public record CreateClinicOrderCommand : ICommand<CreateClinicOrderResponse>
{
    public Guid VisitId { get; init; }
    public decimal ServiceFee { get; init; }
    public List<MedicationPriceDto> MedicationPrices { get; init; } = new();
    public string ReturnUrl { get; init; } = string.Empty;
    public string CancelUrl { get; init; } = string.Empty;
}

public record MedicationPriceDto
{
    public string MedicineName { get; init; } = string.Empty;
    public decimal Price { get; init; }
}

public record CreateClinicOrderResponse
{
    public Guid OrderId { get; init; }
    public string PaymentUrl { get; init; } = string.Empty;
}

public class CreateClinicOrderCommandHandler : ICommandHandler<CreateClinicOrderCommand, CreateClinicOrderResponse>
{
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPayOSService _payOSService;
    private readonly IConsultationSessionRepository _consultationSessionRepository;
    private readonly IRepository<Domain.Entities.Screening.MedicalDiagnosis> _diagnosisRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;
    private readonly PayOSSettings _payOSSettings;

    public CreateClinicOrderCommandHandler(
        IPatientVisitRepository patientVisitRepository,
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IPayOSService payOSService,
        IConsultationSessionRepository consultationSessionRepository,
        IRepository<Domain.Entities.Screening.MedicalDiagnosis> diagnosisRepository,
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        ICurrentUserService currentUser,
        Microsoft.Extensions.Options.IOptions<PayOSSettings> payOSSettings)
    {
        _patientVisitRepository = patientVisitRepository;
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _payOSService = payOSService;
        _consultationSessionRepository = consultationSessionRepository;
        _diagnosisRepository = diagnosisRepository;
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _currentUser = currentUser;
        _payOSSettings = payOSSettings.Value;
    }

    public async Task<Result<CreateClinicOrderResponse>> Handle(CreateClinicOrderCommand request, CancellationToken cancellationToken)
    {
        var visit = await _patientVisitRepository.GetByIdWithDetailsAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result<CreateClinicOrderResponse>.NotFound("Visit not found.");

        if (visit.Status != PatientVisitStatus.WaitingForPayment)
            return Result<CreateClinicOrderResponse>.Failure("Visit is not in a payable state.");

        // Calculate total
        decimal totalMedicationAmount = request.MedicationPrices.Sum(x => x.Price);
        decimal totalAmount = request.ServiceFee + totalMedicationAmount;

        if (totalAmount <= 0)
            return Result<CreateClinicOrderResponse>.Failure("Total amount must be greater than zero.");

        // Get doctor name from diagnosis for metadata (more reliable than visit)
        string doctorName = "N/A";
        var consultation = await _consultationSessionRepository.Query()
            .Where(c => c.PatientId == visit.PatientId && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (consultation != null)
        {
            var diagnosis = await _diagnosisRepository.Query()
                .Where(d => d.ConsultationSessionId == consultation.Id && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (diagnosis != null)
            {
                var doctor = await _identityService.GetUserByIdAsync(diagnosis.DoctorId, cancellationToken);
                doctorName = doctor?.FullName ?? "N/A";
            }
        }

        // Create detailed description with METADATA prefix for webhook detection
        // Format: METADATA:{"V":"visitId"} | BN: [Name] | BS: [Doctor] | Items...
        var metadata = new { V = visit.Id };
        var metadataJson = System.Text.Json.JsonSerializer.Serialize(metadata);
        
        string patientName = visit.Patient?.FullName ?? "Patient";
        string shortDescription = $"Thanh toán thuốc & dịch vụ - BN: {patientName} - BS: {doctorName}";
        string fullDescription = $"METADATA:{metadataJson} | {shortDescription}";
        
        // PayOS display description (clean and short)
        // PayOS has 25 char limit. Use "Thuoc [ShortName]"
        string shortName = patientName.Split(' ').LastOrDefault() ?? "BN";
        string payosDisplayDesc = $"Thuoc {shortName} {visit.Id.ToString().Substring(0, 5)}";

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // 1. Create Order
            // FIX: Use UserId from patient, or fallback to current staff if walk-in
            var orderUserId = visit.Patient?.UserId ?? _currentUser.UserId.Value;

            var order = new Order(
                orderUserId, 
                totalAmount, 
                null, 
                fullDescription, 
                visit.AppointmentId);
            
            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 2. Create Payment
            var payment = new Payment(
                order.Id, 
                totalAmount, 
                PaymentMethod.PayOS, 
                fullDescription);
            
            await _paymentRepository.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 3. Create PayOS Link
            var (paymentUrl, orderCode) = await _payOSService.CreatePaymentLinkAsync(
                payment.Id,
                totalAmount,
                payosDisplayDesc,
                string.IsNullOrWhiteSpace(request.ReturnUrl) ? _payOSSettings.DefaultReturnUrl : request.ReturnUrl,
                string.IsNullOrWhiteSpace(request.CancelUrl) ? _payOSSettings.DefaultCancelUrl : request.CancelUrl);

            payment.SetPaymentLink(paymentUrl, orderCode);
            await _paymentRepository.UpdateAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<CreateClinicOrderResponse>.Success(new CreateClinicOrderResponse
            {
                OrderId = order.Id,
                PaymentUrl = paymentUrl
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<CreateClinicOrderResponse>.Failure($"Failed to create payment: {ex.Message}");
        }
    }
}
