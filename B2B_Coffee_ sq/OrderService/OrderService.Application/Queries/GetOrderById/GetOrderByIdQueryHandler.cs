using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Application.Mappings;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Application.Queries.GetOrderById;

public class GetOrderByIdQueryHandler
    : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderDbContext _db;

    public GetOrderByIdQueryHandler(IOrderDbContext db) => _db = db;

    public async Task<OrderDto?> Handle(
        GetOrderByIdQuery request, CancellationToken ct)
    {
        var query = _db.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .Where(o => o.Id == request.OrderId);

        if (request.ClientId.HasValue)
            query = query.Where(o => o.ClientId == request.ClientId);

        var order = await query.FirstOrDefaultAsync(ct);
        return order?.ToDto();
    }
}