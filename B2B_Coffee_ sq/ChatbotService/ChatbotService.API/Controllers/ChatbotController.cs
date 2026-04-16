using ChatbotService.Application.Commands;
using ChatbotService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatbotService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatbotController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AskChatbot([FromBody] ChatRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Message cannot be empty.");

        var response = await _mediator.Send(new AskChatbotCommand(request.Message));
        return Ok(response);
    }
}
