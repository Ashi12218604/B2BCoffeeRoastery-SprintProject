using DeliveryService.Application.DTOs;
using DeliveryService.Application.Interfaces;
using DeliveryService.Application.Mappings;
using DeliveryService.Domain.Entities;
using DeliveryService.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryService.Application.Commands.AssignAgent;

public class AssignAgentCommandHandler
    : IRequestHandler<AssignAgentCommand, DeliveryDto?>
{
    private readonly IDeliveryDbContext _db;

    public AssignAgentCommandHandler(IDeliveryDbContext db) => _db = db;

    public async Task<DeliveryDto?> Handle(
        AssignAgentCommand request, CancellationToken ct)
    {
        var delivery = await _db.Deliveries
            .Include(d => d.StatusHistory)
            .FirstOrDefaultAsync(d => d.Id == request.DeliveryId, ct);

        if (delivery is null) return null;

        delivery.AssignedAgent = request.AgentName;
        delivery.AgentPhone = request.AgentPhone;
        delivery.Status = DeliveryStatus.Assigned;
        delivery.UpdatedAt = DateTime.UtcNow;

        delivery.StatusHistory.Add(new DeliveryStatusHistory
        {
            Status = DeliveryStatus.Assigned,
            Note = $"Agent {request.AgentName} assigned."
        });

        await _db.SaveChangesAsync(ct);
        return delivery.ToDto();
    }
}