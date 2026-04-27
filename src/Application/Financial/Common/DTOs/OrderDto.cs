using Domain.Enums;

namespace Application.Financial.Common.DTOs;

public record OrderDto(
    Guid Id,
    Guid UserId,
    decimal TotalAmount,
    decimal? DepositAmount,
    string? PatientName,
    string? Description,
    string Status,
    DateTime CreatedAt,
    decimal PaidAmount,
    List<PaymentDto> Payments);

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    PaymentStatus Status,
    PaymentMethod Method,
    DateTime? PaidAt,
    string? PaymentUrl,
    string? Description,
    string? PaymentOrderCode = null);
