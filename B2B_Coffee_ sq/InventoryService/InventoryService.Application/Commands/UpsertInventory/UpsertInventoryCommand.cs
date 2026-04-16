using InventoryService.Application.DTOs;
using MediatR;
using System;

namespace InventoryService.Application.Commands.UpsertInventory;

public record UpsertInventoryCommand(
    Guid ProductId,
    string ProductName,
    string SKU,
    int QuantityAvailable,
    int LowStockThreshold
) : IRequest<InventoryItemDto>;