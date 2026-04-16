using DeliveryService.Application.DTOs;
using DeliveryService.Application.Interfaces;
using DeliveryService.Application.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryService.Application.Queries.GetDeliveryById;

public class GetDeliveryByIdQueryHandler
    : IRequestHandler<GetDeliveryByIdQuery, DeliveryDto?>
{
    private readonly IDeliveryDbContext _db;

    public GetDeliveryByIdQueryHandler(IDeliveryDbContext db) => _db = db;

    public async Task<DeliveryDto?> Handle(
        GetDeliveryByIdQuery request, CancellationToken ct)
    {
        var delivery = await _db.Deliveries
            .Include(d => d.StatusHistory)
            .FirstOrDefaultAsync(d => d.Id == request.Id, ct);

        return delivery?.ToDto();
    }
}