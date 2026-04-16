using B2B.Contracts.Events.Inventory;
using B2B.Contracts.Events.Order;
using MassTransit;
using OrderService.Domain.Entities;
using System.Text.Json;

namespace OrderService.Infrastructure.Saga;

// ── Order Fulfillment Saga ────────────────────────────────────────────────
// Flow:
//   OrderPlaced → [wait for inventory]
//     InventoryDeducted → OrderConfirmed (publish)
//     InventoryFailed   → OrderRejected  (publish + compensate)
public class OrderFulfillmentSaga
    : MassTransitStateMachine<OrderSagaState>
{
    // States
    public State WaitingForInventory { get; private set; } = null!;
    public State OrderConfirmedState { get; private set; } = null!;
    public State OrderRejectedState { get; private set; } = null!;

    // Events
    public Event<IOrderPlacedEvent> OrderPlaced { get; private set; }
        = null!;
    public Event<IInventoryDeductedEvent> InventoryDeducted { get; private set; }
        = null!;
    public Event<IInventoryDeductionFailedEvent> InventoryFailed { get; private set; }
        = null!;

    public OrderFulfillmentSaga()
    {
        InstanceState(x => x.CurrentState);

        // Correlate all events by OrderId
        Event(() => OrderPlaced,
            x => x.CorrelateBy(
                state => state.OrderId.ToString(),
                ctx => ctx.Message.OrderId.ToString())
            .SelectId(ctx => NewId.NextGuid()));

        Event(() => InventoryDeducted,
            x => x.CorrelateBy(
                state => state.OrderId.ToString(),
                ctx => ctx.Message.OrderId.ToString()));

        Event(() => InventoryFailed,
            x => x.CorrelateBy(
                state => state.OrderId.ToString(),
                ctx => ctx.Message.OrderId.ToString()));

        // ── Initial → WaitingForInventory ────────────────────────────────
        Initially(
            When(OrderPlaced)
                .Then(ctx =>
                {
                    ctx.Saga.OrderId = ctx.Message.OrderId;
                    ctx.Saga.ClientId = ctx.Message.ClientId;
                    ctx.Saga.ClientEmail = ctx.Message.ClientEmail;
                    ctx.Saga.ClientName = ctx.Message.ClientName;
                    ctx.Saga.TotalAmount = ctx.Message.TotalAmount;
                    ctx.Saga.ProductNamesJson = JsonSerializer.Serialize(ctx.Message.Items.Select(i => i.ProductName).ToList());
                    ctx.Saga.DeliveryAddress = ctx.Message.DeliveryAddress;
                    ctx.Saga.City = ctx.Message.City;
                    ctx.Saga.State = ctx.Message.State;
                    ctx.Saga.PinCode = ctx.Message.PinCode;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(WaitingForInventory));

        // ── WaitingForInventory → Confirmed ───────────────────────────────
        During(WaitingForInventory,
            When(InventoryDeducted)
                .Then(ctx => ctx.Saga.UpdatedAt = DateTime.UtcNow)
                .PublishAsync(ctx => ctx.Init<IOrderConfirmedEvent>(new
                {
                    OrderId = ctx.Saga.OrderId,
                    ClientId = ctx.Saga.ClientId,
                    ClientEmail = ctx.Saga.ClientEmail,
                    ClientName = ctx.Saga.ClientName,
                    TotalAmount = ctx.Saga.TotalAmount,
                    AdminName = ctx.Saga.AdminName,
                    ProductNames = JsonSerializer.Deserialize<List<string>>(ctx.Saga.ProductNamesJson) ?? new List<string>(),
                    DeliveryAddress = ctx.Saga.DeliveryAddress,
                    City = ctx.Saga.City,
                    State = ctx.Saga.State,
                    PinCode = ctx.Saga.PinCode,
                    ConfirmedAt = DateTime.UtcNow
                }))
                .TransitionTo(OrderConfirmedState),

            // ── WaitingForInventory → Rejected (Compensating Transaction)
            When(InventoryFailed)
                .Then(ctx =>
                {
                    ctx.Saga.FailureReason = ctx.Message.Reason;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .PublishAsync(ctx => ctx.Init<IOrderRejectedEvent>(new
                {
                    OrderId = ctx.Saga.OrderId,
                    ClientId = ctx.Saga.ClientId,
                    ClientEmail = ctx.Saga.ClientEmail,
                    ClientName = ctx.Saga.ClientName,
                    Reason = ctx.Message.Reason,
                    RejectedAt = DateTime.UtcNow
                }))
                .TransitionTo(OrderRejectedState));

        // Mark as final so saga instance is cleaned up
        SetCompletedWhenFinalized();
    }
}