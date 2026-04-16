using MediatR;
using System;
using System.Collections.Generic;

namespace InventoryService.Application.Commands.DeductInventory;

public record DeductInventoryCommand(
    Guid OrderId,
    Guid CorrelationId,
    List<DeductInventoryItemRequest> Items
) : IRequest<DeductInventoryResult>;

public record DeductInventoryItemRequest(Guid ProductId, int Quantity);
public record DeductInventoryResult(bool Success, string Reason = "");