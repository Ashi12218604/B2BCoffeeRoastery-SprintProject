using DeliveryService.Domain.Enums;
using System;
using System.Collections.Generic;

namespace DeliveryService.Domain.Entities;

public class Delivery
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public string ClientEmail { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PinCode { get; set; } = string.Empty;
    public string? ApprovedByAdminName { get; set; }
    public string? ProductNames { get; set; }

    
    public DeliveryStatus Status { get; set; }
    public string? TrackingNumber { get; set; }
    public string? AssignedAgent { get; set; }
    public string? AgentPhone { get; set; }
    public string? Notes { get; set; }
    
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<DeliveryStatusHistory> StatusHistory { get; set; } = new List<DeliveryStatusHistory>();
}
