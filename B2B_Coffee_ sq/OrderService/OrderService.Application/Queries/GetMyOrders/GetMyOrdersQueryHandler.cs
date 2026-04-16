using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Application.Mappings;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Application.Queries.GetMyOrders;

public class GetMyOrdersQueryHandler
    : IRequestHandler<GetMyOrdersQuery, List<OrderDto>>
{
    private readonly IOrderDbContext _db;

    public GetMyOrdersQueryHandler(IOrderDbContext db) => _db = db;

    public async Task<List<OrderDto>> Handle(
        GetMyOrdersQuery request, CancellationToken ct) =>
        await _db.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .Where(o => o.ClientId == request.ClientId)
            .OrderByDescending(o => o.PlacedAt)
            .Select(o => o.ToDto())
            .ToListAsync(ct);
}