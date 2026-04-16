using InventoryService.API.DTOs;
using InventoryService.Application.Commands.RestockInventory;
using InventoryService.Application.Commands.UpsertInventory;
using InventoryService.Application.DTOs;
using InventoryService.Application.Queries.GetAllInventory;
using InventoryService.Application.Queries.GetInventoryByProduct;
using InventoryService.Application.Queries.GetTransactionHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InventoryService.API.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator) => _mediator = mediator;

    // GET all inventory (Admin/SuperAdmin)
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? lowStockOnly) =>
        Ok(await _mediator.Send(
            new GetAllInventoryQuery(lowStockOnly)));

    // GET status for chatbot integration (Anonymous)
    [HttpGet("status")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStatus() =>
        Ok(await _mediator.Send(new GetAllInventoryQuery(null)));

    // GET inventory for a specific product
    [HttpGet("product/{productId:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var result = await _mediator.Send(
            new GetInventoryByProductQuery(productId));
        return result is null ? NotFound() : Ok(result);
    }

    // GET transaction history for a product
    [HttpGet("product/{productId:guid}/transactions")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetTransactions(Guid productId) =>
        Ok(await _mediator.Send(
            new GetTransactionHistoryQuery(productId)));

    // POST upsert inventory (SuperAdmin Only)
    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Upsert(
        [FromBody] UpsertInventoryDto dto)
    {
        var result = await _mediator.Send(new UpsertInventoryCommand(
            dto.ProductId, dto.ProductName, dto.SKU,
            dto.QuantityAvailable, dto.LowStockThreshold));
        return Ok(result);
    }

    // POST restock (SuperAdmin Only)
    [HttpPost("product/{productId:guid}/restock")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Restock(
        Guid productId, [FromBody] RestockDto dto)
    {
        var result = await _mediator.Send(new RestockInventoryCommand(
            productId, dto.Quantity, dto.Reason));
        return result is null
            ? NotFound(new { message = "Product not in inventory." })
            : Ok(result);
    }
}