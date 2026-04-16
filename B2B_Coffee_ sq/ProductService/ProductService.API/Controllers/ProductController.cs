using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.AddReview;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Application.Commands.DeleteProduct;
using ProductService.Application.Commands.UpdateProduct;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.GetAllProducts;
using ProductService.Application.Queries.GetProductById;
using ProductService.Application.Queries.GetProductReviews;
using ProductService.Domain.Enums;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]   // JWT required for all endpoints
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator) => _mediator = mediator;

    // ── GET all products (paginated + filtered) ───────────────────────────
    // Accessible by: Approved Clients, Admin, SuperAdmin
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] ProductCategory? category,
        [FromQuery] RoastLevel? roastLevel,
        [FromQuery] bool? isFeatured,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // Block unapproved clients
        var status = User.FindFirstValue("status");
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (role == "Client" && status != "Approved")
            return Forbid();

        var result = await _mediator.Send(new GetAllProductsQuery(
            search, category, roastLevel, isFeatured,
            minPrice, maxPrice, page, pageSize));

        return Ok(result);
    }

    // ── GET product by ID ─────────────────────────────────────────────────
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var status = User.FindFirstValue("status");
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (role == "Client" && status != "Approved")
            return Forbid();

        var product = await _mediator.Send(new GetProductByIdQuery(id));
        return product is null ? NotFound() : Ok(product);
    }

    // ── CREATE product (Admin/SuperAdmin only) ────────────────────────────
    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var adminId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(new CreateProductCommand(
            dto.Name, dto.Description, dto.SKU,
            dto.Price, dto.DiscountedPrice, dto.Origin,
            dto.Category, dto.RoastLevel, dto.WeightInGrams,
            dto.ImageUrl, dto.IsFeatured,
            dto.MinimumOrderQuantity, adminId));

        return CreatedAtAction(
            nameof(GetById), new { id = result.Id }, result);
    }

    // ── UPDATE product (Admin/SuperAdmin only) ────────────────────────────
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateProductDto dto)
    {
        var result = await _mediator.Send(new UpdateProductCommand(
            id, dto.Name, dto.Description, dto.SKU,
            dto.Price, dto.DiscountedPrice, dto.Origin,
            dto.Category, dto.RoastLevel, dto.WeightInGrams,
            dto.ImageUrl, dto.IsFeatured,
            dto.IsActive, dto.MinimumOrderQuantity));

        return result is null ? NotFound() : Ok(result);
    }

    // ── SOFT DELETE product (Admin/SuperAdmin only) ───────────────────────
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id));
        return result
            ? Ok(new { message = "Product deactivated successfully." })
            : NotFound();
    }

    // ── GET product reviews ───────────────────────────────────────────────
    [HttpGet("{id:guid}/reviews")]
    public async Task<IActionResult> GetReviews(Guid id)
    {
        var reviews = await _mediator.Send(
            new GetProductReviewsQuery(id));
        return Ok(reviews);
    }

    // ── ADD review (Approved Client only) ────────────────────────────────
    [HttpPost("{id:guid}/reviews")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> AddReview(
        Guid id, [FromBody] AddReviewDto dto)
    {
        var status = User.FindFirstValue("status");
        if (status != "Approved")
            return Forbid();

        var clientId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var clientName = User.FindFirstValue(ClaimTypes.Name)!;

        var result = await _mediator.Send(new AddReviewCommand(
            id, clientId, clientName, dto.Rating, dto.Comment));

        return result is null
            ? NotFound(new { message = "Product not found." })
            : Ok(result);
    }
}