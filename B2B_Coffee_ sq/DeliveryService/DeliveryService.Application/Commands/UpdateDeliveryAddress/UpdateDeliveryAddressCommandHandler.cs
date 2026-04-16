using DeliveryService.Application.DTOs;
using DeliveryService.Application.Interfaces;
using DeliveryService.Application.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryService.Application.Commands.UpdateDeliveryAddress;

public class UpdateDeliveryAddressCommandHandler
    : IRequestHandler<UpdateDeliveryAddressCommand, DeliveryDto?>
{
    private readonly IDeliveryDbContext _db;

    public UpdateDeliveryAddressCommandHandler(IDeliveryDbContext db) => _db = db;

    public async Task<DeliveryDto?> Handle(
        UpdateDeliveryAddressCommand request, CancellationToken ct)
    {
        var delivery = await _db.Deliveries
            .Include(d => d.StatusHistory)
            .FirstOrDefaultAsync(d => d.Id == request.DeliveryId, ct);

        if (delivery is null) return null;

        delivery.DeliveryAddress = request.DeliveryAddress;
        delivery.City = request.City;
        delivery.State = request.State;
        delivery.PinCode = request.PinCode;
        delivery.UpdatedAt = System.DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return delivery.ToDto();
    }
}
