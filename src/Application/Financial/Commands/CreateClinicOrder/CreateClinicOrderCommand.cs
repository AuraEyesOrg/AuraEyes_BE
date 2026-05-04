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
    public string Method { get; init; } = "PayOS";
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
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IRepository<Domain.Entities.Users.Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;
    private readonly IClinicVisitService _clinicVisitService;
    private readonly PayOSSettings _payOSSettings;

    public CreateClinicOrderCommandHandler(
        IPatientVisitRepository patientVisitRepository,
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IPayOSService payOSService,
        IConsultationSessionRepository consultationSessionRepository,
        IRepository<Domain.Entities.Screening.MedicalDiagnosis> diagnosisRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IRepository<Domain.Entities.Users.Patient> patientRepository,
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        ICurrentUserService currentUser,
        IClinicVisitService clinicVisitService,
        Microsoft.Extensions.Options.IOptions<PayOSSettings> payOSSettings)
    {
        _patientVisitRepository = patientVisitRepository;
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _payOSService = payOSService;
        _consultationSessionRepository = consultationSessionRepository;
        _diagnosisRepository = diagnosisRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _currentUser = currentUser;
        _clinicVisitService = clinicVisitService;
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

        // Get doctor name from diagnosis for metadata
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
                var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(diagnosis.DoctorId, cancellationToken);
                if (ophthalmologist != null)
                {
                    var doctorUser = await _identityService.GetUserByIdAsync(ophthalmologist.UserId, cancellationToken);
                    doctorName = doctorUser?.FullName ?? "N/A";
                }
            }
        }

        // Ensure patient is loaded for name
        var patient = visit.Patient;
        if (patient == null)
        {
            patient = await _patientRepository.GetByIdAsync(visit.PatientId, cancellationToken);
        }

        var metadata = new { V = visit.Id };
        var metadataJson = System.Text.Json.JsonSerializer.Serialize(metadata);

        string patientName = patient?.FullName ?? "Patient";
        string shortDescription = $"Thanh toán thuốc & dịch vụ - BN: {patientName} - BS: {doctorName}";
        string fullDescription = $"METADATA:{metadataJson} | {shortDescription}";

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var orderUserId = visit.Patient?.UserId ?? _currentUser.UserId.Value;

            var order = new Order(
                orderUserId,
                totalAmount,
                null,
                fullDescription,
                visit.AppointmentId);

            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            bool isCash = request.Method.Equals("Cash", StringComparison.OrdinalIgnoreCase);
            var domainMethod = isCash ? Domain.Enums.PaymentMethod.Cash : Domain.Enums.PaymentMethod.PayOS;

            var payment = new Payment(
                order.Id,
                totalAmount,
                domainMethod,
                fullDescription);

            if (isCash)
            {
                // Cash payment is completed immediately
                payment.Complete("CASH_MANUAL", "Paid by Cash at Reception");
                order.Complete();
            }

            await _paymentRepository.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            string paymentUrl = string.Empty;

            if (!isCash)
            {
                // 3. Create PayOS Link
                var returnUrl = string.IsNullOrWhiteSpace(request.ReturnUrl) ? _payOSSettings.DefaultReturnUrl : request.ReturnUrl;
                var cancelUrl = string.IsNullOrWhiteSpace(request.CancelUrl) ? _payOSSettings.DefaultCancelUrl : request.CancelUrl;

                returnUrl = AppendQueryParam(returnUrl, "orderId", order.Id.ToString());
                cancelUrl = AppendQueryParam(cancelUrl, "orderId", order.Id.ToString());

                // PayOS has 25 char limit.
                string shortName = patientName.Split(' ').LastOrDefault() ?? "BN";
                string payosDisplayDesc = $"Thuoc {shortName} {visit.Id.ToString().Substring(0, 5)}";

                var (url, orderCode) = await _payOSService.CreatePaymentLinkAsync(
                    payment.Id,
                    totalAmount,
                    payosDisplayDesc,
                    returnUrl,
                    cancelUrl);

                paymentUrl = url;
                payment.SetPaymentLink(paymentUrl, orderCode);
                await _paymentRepository.UpdateAsync(payment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // 4. If cash, trigger visit completion logic (creates follow-up chat, etc.)
            if (isCash)
            {
                await _clinicVisitService.ProcessPaymentCompletionAsync(order, "Cash", cancellationToken);
            }

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
    private static string AppendQueryParam(string url, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(url)) return url;
        var separator = url.Contains('?') ? "&" : "?";
        return $"{url}{separator}{key}={value}";
    }
}
