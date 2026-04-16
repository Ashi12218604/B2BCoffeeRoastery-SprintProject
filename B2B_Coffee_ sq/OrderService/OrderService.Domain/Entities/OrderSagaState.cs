using MassTransit;
using System;

namespace OrderService.Domain.Entities;

// Persisted state machine for the Order Fulfillment Saga
public class OrderSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public string ClientEmail { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? AdminName { get; set; }
    public string ProductNamesJson { get; set; } = "[]"; // Store as JSON string for simplicity
    public string? FailureReason { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}