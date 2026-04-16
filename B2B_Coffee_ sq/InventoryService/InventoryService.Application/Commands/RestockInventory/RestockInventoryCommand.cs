using InventoryService.Application.DTOs;
using MediatR;
using System;

namespace InventoryService.Application.Commands.RestockInventory;

public record RestockInventoryCommand(
    Guid ProductId,
    int Quantity,
    string Reason = "Restock"
) : IRequest<InventoryItemDto?>;