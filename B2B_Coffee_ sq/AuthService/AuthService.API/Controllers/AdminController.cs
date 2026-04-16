using AuthService.Application.Commands.ApproveClient;
using AuthService.Application.Commands.CreateAdmin;
using AuthService.Application.Queries.GetPendingClients;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator) => _mediator = mediator;

    /// <summary>Get all clients awaiting approval</summary>
    [HttpGet("pending-clients")]
    public async Task<IActionResult> GetPendingClients() =>
        Ok(await _mediator.Send(new GetPendingClientsQuery()));

    /// <summary>Get all approved clients</summary>
    [HttpGet("clients")]
    public async Task<IActionResult> GetApprovedClients() =>
        Ok(await _mediator.Send(new AuthService.Application.Queries.GetApprovedClients.GetApprovedClientsQuery()));

    /// <summary>Get all admins</summary>
    [HttpGet("admins")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAdmins() =>
        Ok(await _mediator.Send(new AuthService.Application.Queries.GetAdmins.GetAdminsQuery()));

    /// <summary>Approve or Reject a client</summary>
    [HttpPut("approve-client/{id}")]
    public async Task<IActionResult> ApproveClient(Guid id, [FromBody] ApproveClientDto dto)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new ApproveClientCommand(id, adminId, dto.Approve, dto.RejectionReason));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Terminate an approved client</summary>
    [HttpPut("terminate-client/{id}")]
    public async Task<IActionResult> TerminateClient(Guid id)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new AuthService.Application.Commands.TerminateClient.TerminateClientCommand(id, adminId));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Create an Admin account (Role=Admin, Status=Approved, no OTP needed)</summary>
    [HttpPost("create-admin")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto dto)
    {
        var result = await _mediator.Send(new CreateAdminCommand(
            dto.FullName, dto.Email, dto.Password, dto.PhoneNumber));

        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Delete an Admin account</summary>
    [HttpDelete("admins/{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeleteAdmin(Guid id)
    {
        var result = await _mediator.Send(new AuthService.Application.Commands.DeleteAdmin.DeleteAdminCommand(id));
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

public record ApproveClientDto(bool Approve, string? RejectionReason = null);
public record CreateAdminDto(string FullName, string Email, string Password, string PhoneNumber);