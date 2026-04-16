using B2B.Contracts.Events.Order;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Application.Mappings;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Application.Commands.PlaceOrder;

public class PlaceOrderCommandHandler
    : IRequestHandler<PlaceOrderCommand, OrderDto>
{
    private readonly IOrderDbContext _db;
    private readonly IPublishEndpoint _publish;

    public PlaceOrderCommandHandler(
        IOrderDbContext db, IPublishEndpoint publish)
    {
        _db = db;
        _publish = publish;
    }

    public async Task<OrderDto> Handle(
        PlaceOrderCommand request, CancellationToken ct)
    {
        var order = new Order
        {
            ClientId = request.ClientId,
            ClientEmail = request.ClientEmail,
            ClientName = request.ClientName,
            CompanyName = request.CompanyName,
            Status = OrderStatus.Pending,
            Notes = request.Notes,
            DeliveryAddress = request.DeliveryAddress ?? "Address Pending",
            City = request.City ?? "Pending",
            State = request.State ?? "Pending",
            PinCode = request.PinCode ?? "000000",
            Items = request.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                SKU = i.SKU,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

        order.StatusHistory.Add(new OrderStatusHistory
        {
            Status = OrderStatus.Pending,
            Note = "Order placed by client."
        });

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);

        // Publish → InventoryService deducts, NotificationService emails
        await _publish.Publish<IOrderPlacedEvent>(new
        {
            OrderId = order.Id,
            order.ClientId,
            order.ClientEmail,
            order.ClientName,
            Items = order.Items.Select(i => new
            {
                i.ProductId,
                i.ProductName,
                i.Quantity,
                i.UnitPrice
            }).ToList(),
            TotalAmount = order.TotalAmount,
            PlacedAt = DateTime.UtcNow,
            DeliveryAddress = order.DeliveryAddress,
            City = order.City,
            State = order.State,
            PinCode = order.PinCode
        }, ct);

        return order.ToDto();
    }
}