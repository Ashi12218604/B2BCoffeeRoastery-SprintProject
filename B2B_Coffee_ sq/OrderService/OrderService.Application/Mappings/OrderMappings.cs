using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using System.Linq;

namespace OrderService.Application.Mappings;

public static class OrderMappings
{
    public static OrderDto ToDto(this Order o) => new(
        o.Id,
        o.ClientId,
        o.ClientEmail,
        o.ClientName,
        o.CompanyName,
        o.Status.ToString(),
        o.TotalAmount,
        o.RejectionReason,
        o.Notes,
        o.PaymentStatus.ToString(),
        o.RazorpayOrderId,
        o.RazorpayPaymentId,
        o.PaidAt,
        o.Items.Select(i => new OrderItemDto(
            i.Id, i.ProductId, i.ProductName,
            i.SKU, i.Quantity, i.UnitPrice, i.Subtotal)).ToList(),
        o.StatusHistory
            .OrderBy(h => h.ChangedAt)
            .Select(h => new OrderStatusHistoryDto(
                h.Status.ToString(), h.Note, h.ChangedAt)).ToList(),
        o.PlacedAt,
        o.UpdatedAt
    );
}