using MediatR;
using System;
using System.Collections.Generic;

namespace InventoryService.Application.Commands.RestoreInventory;

public record RestoreInventoryCommand(
    Guid OrderId,
    List<InventoryRestoreItem> Items,
    string Reason = "Order Cancelled"
) : IRequest<bool>;

public record InventoryRestoreItem(Guid ProductId, int Quantity);
