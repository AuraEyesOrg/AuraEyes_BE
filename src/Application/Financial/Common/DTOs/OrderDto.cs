using Domain.Enums;

namespace Application.Financial.Common.DTOs;

public record OrderDto(
    Guid Id,
    Guid UserId,
    decimal TotalAmount,
    decimal? DepositAmount,
    string? PatientName,
    string? Description,
    OrderStatus Status,
    DateTime CreatedAt,
    List<PaymentDto> Payments);

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    PaymentStatus Status,
    PaymentMethod Method,
    DateTime? PaidAt,
    string? PaymentUrl,
    string? Description);
