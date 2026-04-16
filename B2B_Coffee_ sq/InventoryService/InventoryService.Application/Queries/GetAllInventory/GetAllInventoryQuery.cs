using InventoryService.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace InventoryService.Application.Queries.GetAllInventory;

public record GetAllInventoryQuery(bool? LowStockOnly = null)
    : IRequest<List<InventoryItemDto>>;