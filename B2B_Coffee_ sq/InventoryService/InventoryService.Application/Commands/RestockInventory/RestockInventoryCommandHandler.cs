using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces;
using InventoryService.Application.Mappings;
using InventoryService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Application.Commands.RestockInventory;

public class RestockInventoryCommandHandler
    : IRequestHandler<RestockInventoryCommand, InventoryItemDto?>
{
    private readonly IInventoryDbContext _db;

    public RestockInventoryCommandHandler(IInventoryDbContext db)
        => _db = db;

    public async Task<InventoryItemDto?> Handle(
        RestockInventoryCommand request, CancellationToken ct)
    {
        var item = await _db.InventoryItems
            .FirstOrDefaultAsync(
                i => i.ProductId == request.ProductId, ct);

        if (item is null) return null;

        var before = item.QuantityAvailable;
        item.QuantityAvailable += request.Quantity;
        item.UpdatedAt = DateTime.UtcNow;

        _db.InventoryTransactions.Add(new InventoryTransaction
        {
            InventoryItemId = item.Id,
            Type = "Restock",
            Quantity = request.Quantity,
            QuantityBefore = before,
            QuantityAfter = item.QuantityAvailable,
            Reason = request.Reason
        });

        await _db.SaveChangesAsync(ct);
        return item.ToDto();
    }
}