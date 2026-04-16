using AuthService.Application.Commands.Login;
using AuthService.Application.Commands.RegisterClient;
using AuthService.Application.Commands.VerifyOtp;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Client Registration — Step 1 (sends OTP to Gmail)</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterClientDto dto)
    {
        var result = await _mediator.Send(new RegisterClientCommand(
            dto.FullName, dto.Email, dto.Password, dto.PhoneNumber, dto.CompanyName));

        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>OTP Verification — Step 2</summary>
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
    {
        var result = await _mediator.Send(new VerifyOtpCommand(dto.Email, dto.OtpCode));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Login — returns JWT token</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _mediator.Send(new LoginCommand(dto.Email, dto.Password));
        return result.Success ? Ok(result) : Unauthorized(result);
    }
}

// Request DTOs
public record RegisterClientDto(string FullName, string Email, string Password, string PhoneNumber, string CompanyName);
public record VerifyOtpDto(string Email, string OtpCode);
public record LoginDto(string Email, string Password);