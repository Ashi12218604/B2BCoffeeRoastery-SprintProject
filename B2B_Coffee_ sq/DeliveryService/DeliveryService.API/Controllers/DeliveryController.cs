using DeliveryService.Application.Commands.AssignAgent;
using DeliveryService.Application.Commands.CreateDelivery;
using DeliveryService.Application.Commands.UpdateDeliveryStatus;
using DeliveryService.Application.DTOs;
using DeliveryService.Application.Queries.GetAllDeliveries;
using DeliveryService.Application.Queries.GetDeliveryById;
using DeliveryService.Application.Queries.GetDeliveryByOrder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DeliveryService.API.Controllers;

[ApiController]
[Route("api/deliveries")]
[Authorize]
public class DeliveryController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeliveryController(IMediator mediator) => _mediator = mediator;

    // GET all deliveries (Admin/SuperAdmin)
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] DeliveryService.Domain.Enums.DeliveryStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20) =>
        Ok(await _mediator.Send(
            new GetAllDeliveriesQuery(status, page, pageSize)));

    // GET delivery by ID
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetDeliveryByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    // GET delivery by order ID (Client can check their own delivery)
    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var result = await _mediator.Send(
            new GetDeliveryByOrderQuery(orderId));
        return result is null ? NotFound() : Ok(result);
    }

    // POST manually create delivery (Admin)
    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryDto dto)
    {
        var result = await _mediator.Send(new CreateDeliveryCommand(
            dto.OrderId, dto.ClientId, dto.ClientEmail, dto.ClientName,
            dto.DeliveryAddress, dto.City, dto.State, dto.PinCode,
            null, null,
            dto.EstimatedDeliveryDate, dto.Notes));

        return CreatedAtAction(
            nameof(GetById), new { id = result.Id }, result);
    }

    // PUT update delivery status (Admin/SuperAdmin)
    // This triggers real Gmail emails via RabbitMQ
    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateStatus(
        Guid id, [FromBody] UpdateDeliveryStatusDto dto)
    {
        var result = await _mediator.Send(new UpdateDeliveryStatusCommand(
            id, dto.NewStatus, dto.Note, dto.Location,
            dto.TrackingNumber, dto.AssignedAgent, dto.AgentPhone));

        return result is null ? NotFound() : Ok(result);
    }

    // PUT assign agent (Admin/SuperAdmin)
    [HttpPut("{id:guid}/assign-agent")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> AssignAgent(
        Guid id, [FromBody] AssignAgentDto dto)
    {
        var result = await _mediator.Send(new AssignAgentCommand(
            id, dto.AgentName, dto.AgentPhone));

        return result is null ? NotFound() : Ok(result);
    }

    // PUT update address (Admin/SuperAdmin)
    [HttpPut("{id:guid}/address")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateAddress(
        Guid id, [FromBody] UpdateDeliveryAddressDto dto)
    {
        var result = await _mediator.Send(new Application.Commands.UpdateDeliveryAddress.UpdateDeliveryAddressCommand(
            id, dto.DeliveryAddress, dto.City, dto.State, dto.PinCode));

        return result is null ? NotFound() : Ok(result);
    }
}