using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Application.Mappings;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Application.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler
    : IRequestHandler<GetAllOrdersQuery, List<OrderDto>>
{
    private readonly IOrderDbContext _db;

    public GetAllOrdersQueryHandler(IOrderDbContext db) => _db = db;

    public async Task<List<OrderDto>> Handle(
        GetAllOrdersQuery request, CancellationToken ct)
    {
        var query = _db.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(o => o.Status == request.Status);

        return await query
            .OrderByDescending(o => o.PlacedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => o.ToDto())
            .ToListAsync(ct);
    }
}