using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.PlaceOrder;
using OrderService.Application.Commands.UpdateOrderStatus;
using OrderService.Application.DTOs;
using OrderService.Application.Queries.GetAllOrders;
using OrderService.Application.Queries.GetMyOrders;
using OrderService.Application.Queries.GetOrderById;
using OrderService.Domain.Enums;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator) => _mediator = mediator;

    // POST place order (Approved Client only)
    [HttpPost]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> PlaceOrder(
        [FromBody] PlaceOrderDto dto)
    {
        var status = User.FindFirstValue("status");
        if (status != "Approved")
            return Forbid();

        var clientId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var clientEmail = User.FindFirstValue(ClaimTypes.Email)!;
        var clientName = User.FindFirstValue(ClaimTypes.Name)!;
        var company = User.FindFirstValue("companyName") ?? "";

        var result = await _mediator.Send(new PlaceOrderCommand(
            clientId, clientEmail, clientName, company,
            dto.Items, dto.Notes, dto.DeliveryAddress, dto.City, dto.State, dto.PinCode));

        return CreatedAtAction(
            nameof(GetById), new { id = result.Id }, result);
    }

    // GET my orders (Client)
    [HttpGet("my")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> GetMyOrders()
    {
        var clientId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _mediator.Send(new GetMyOrdersQuery(clientId)));
    }

    // GET order by ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        Guid? clientId = null;

        if (role == "Client")
            clientId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(
            new GetOrderByIdQuery(id, clientId));

        return result is null ? NotFound() : Ok(result);
    }

    // GET all orders (Admin/SuperAdmin)
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] OrderStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20) =>
        Ok(await _mediator.Send(
            new GetAllOrdersQuery(status, page, pageSize)));

    // PUT update order status (Admin/SuperAdmin)
    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateStatus(
        Guid id, [FromBody] UpdateOrderStatusDto dto)
    {
        var result = await _mediator.Send(new UpdateOrderStatusCommand(
            id, dto.NewStatus, dto.Note, dto.TrackingNumber));

        return result is null ? NotFound() : Ok(result);
    }

    // POST create payment order (Client)
    [HttpPost("{id:guid}/pay")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> CreatePaymentOrder(Guid id)
    {
        var clientId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new Application.Commands.CreatePaymentOrder.CreatePaymentOrderCommand(id, clientId));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // POST verify payment (Client)
    [HttpPost("{id:guid}/verify-payment")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> VerifyPayment(Guid id, [FromBody] PaymentVerificationDto dto)
    {
        var clientId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new Application.Commands.VerifyPayment.VerifyPaymentCommand(id, clientId, dto.RazorpayPaymentId, dto.RazorpaySignature));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // POST cancel order (Client)
    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Client,SuperAdmin")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");
        var clientId = (role == "Client" || role == "client" || User.IsInRole("Client")) 
            ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) 
            : (Guid?)null;
        var result = await _mediator.Send(new Application.Commands.CancelOrder.CancelOrderCommand(id, clientId, role));
        return result is null ? BadRequest(new { Message = "Order cannot be cancelled or not found." }) : Ok(result);
    }
}