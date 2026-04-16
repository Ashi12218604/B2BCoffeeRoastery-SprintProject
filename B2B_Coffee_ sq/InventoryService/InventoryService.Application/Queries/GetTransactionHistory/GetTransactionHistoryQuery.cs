using InventoryService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace InventoryService.Application.Queries.GetTransactionHistory;

public record GetTransactionHistoryQuery(Guid ProductId)
    : IRequest<List<InventoryTransactionDto>>;