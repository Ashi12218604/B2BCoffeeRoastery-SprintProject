using InventoryService.Application.DTOs;
using MediatR;
using System;

namespace InventoryService.Application.Queries.GetInventoryByProduct;

public record GetInventoryByProductQuery(Guid ProductId)
    : IRequest<InventoryItemDto?>;