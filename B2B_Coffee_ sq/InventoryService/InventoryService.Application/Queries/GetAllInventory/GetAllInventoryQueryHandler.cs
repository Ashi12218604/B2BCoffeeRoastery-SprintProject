using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces;
using InventoryService.Application.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Application.Queries.GetAllInventory;

public class GetAllInventoryQueryHandler
    : IRequestHandler<GetAllInventoryQuery, List<InventoryItemDto>>
{
    private readonly IInventoryDbContext _db;

    public GetAllInventoryQueryHandler(IInventoryDbContext db) => _db = db;

    public async Task<List<InventoryItemDto>> Handle(
        GetAllInventoryQuery request, CancellationToken ct)
    {
        var query = _db.InventoryItems.AsQueryable();

        if (request.LowStockOnly == true)
            query = query.Where(i =>
                i.QuantityAvailable <= i.LowStockThreshold);

        return await query
            .OrderBy(i => i.ProductName)
            .Select(i => i.ToDto())
            .ToListAsync(ct);
    }
}