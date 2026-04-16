using DeliveryService.Application.DTOs;
using DeliveryService.Application.Interfaces;
using DeliveryService.Application.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryService.Application.Queries.GetDeliveryByOrder;

public class GetDeliveryByOrderQueryHandler
    : IRequestHandler<GetDeliveryByOrderQuery, DeliveryDto?>
{
    private readonly IDeliveryDbContext _db;

    public GetDeliveryByOrderQueryHandler(IDeliveryDbContext db)
        => _db = db;

    public async Task<DeliveryDto?> Handle(
        GetDeliveryByOrderQuery request, CancellationToken ct)
    {
        var delivery = await _db.Deliveries
            .Include(d => d.StatusHistory)
            .FirstOrDefaultAsync(d => d.OrderId == request.OrderId, ct);

        return delivery?.ToDto();
    }
}