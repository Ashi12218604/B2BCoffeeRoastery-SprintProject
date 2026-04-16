using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces;
using InventoryService.Application.Mappings;
using InventoryService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Application.Commands.UpsertInventory;

public class UpsertInventoryCommandHandler
    : IRequestHandler<UpsertInventoryCommand, InventoryItemDto>
{
    private readonly IInventoryDbContext _db;

    public UpsertInventoryCommandHandler(IInventoryDbContext db)
        => _db = db;

    public async Task<InventoryItemDto> Handle(
        UpsertInventoryCommand request, CancellationToken ct)
    {
        var item = await _db.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == request.ProductId, ct);

        if (item is null)
        {
            item = new InventoryItem
            {
                ProductId = request.ProductId,
                ProductName = request.ProductName,
                SKU = request.SKU,
                QuantityAvailable = request.QuantityAvailable,
                LowStockThreshold = request.LowStockThreshold
            };
            _db.InventoryItems.Add(item);
        }
        else
        {
            item.ProductName = request.ProductName;
            item.SKU = request.SKU;
            item.QuantityAvailable = request.QuantityAvailable;
            item.LowStockThreshold = request.LowStockThreshold;
            item.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        return item.ToDto();
    }
}