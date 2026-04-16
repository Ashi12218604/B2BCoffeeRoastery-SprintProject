using DeliveryService.Application.DTOs;
using DeliveryService.Application.Interfaces;
using DeliveryService.Application.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryService.Application.Queries.GetAllDeliveries;

public class GetAllDeliveriesQueryHandler
    : IRequestHandler<GetAllDeliveriesQuery, List<DeliveryDto>>
{
    private readonly IDeliveryDbContext _db;

    public GetAllDeliveriesQueryHandler(IDeliveryDbContext db) => _db = db;

    public async Task<List<DeliveryDto>> Handle(
        GetAllDeliveriesQuery request, CancellationToken ct)
    {
        var query = _db.Deliveries
            .Include(d => d.StatusHistory)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(d => d.Status == request.Status);

        return await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => d.ToDto())
            .ToListAsync(ct);
    }
}