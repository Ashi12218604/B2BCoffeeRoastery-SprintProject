using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Application.Queries.GetTransactionHistory;

public class GetTransactionHistoryQueryHandler
    : IRequestHandler<GetTransactionHistoryQuery,
        List<InventoryTransactionDto>>
{
    private readonly IInventoryDbContext _db;

    public GetTransactionHistoryQueryHandler(IInventoryDbContext db)
        => _db = db;

    public async Task<List<InventoryTransactionDto>> Handle(
        GetTransactionHistoryQuery request, CancellationToken ct)
    {
        var item = await _db.InventoryItems
            .FirstOrDefaultAsync(
                i => i.ProductId == request.ProductId, ct);

        if (item is null) return new();

        return await _db.InventoryTransactions
            .Where(t => t.InventoryItemId == item.Id)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new InventoryTransactionDto(
                t.Id, t.OrderId, t.Type, t.Quantity,
                t.QuantityBefore, t.QuantityAfter,
                t.Reason, t.CreatedAt))
            .ToListAsync(ct);
    }
}