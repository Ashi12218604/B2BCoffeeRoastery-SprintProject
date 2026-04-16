using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces;
using InventoryService.Application.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Application.Queries.GetInventoryByProduct;

public class GetInventoryByProductQueryHandler
    : IRequestHandler<GetInventoryByProductQuery, InventoryItemDto?>
{
    private readonly IInventoryDbContext _db;

    public GetInventoryByProductQueryHandler(IInventoryDbContext db)
        => _db = db;

    public async Task<InventoryItemDto?> Handle(
        GetInventoryByProductQuery request, CancellationToken ct)
    {
        var item = await _db.InventoryItems
            .FirstOrDefaultAsync(
                i => i.ProductId == request.ProductId, ct);
        return item?.ToDto();
    }
}