using ChatbotService.Application.DTOs;
using ChatbotService.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ChatbotService.Application.Commands;

public class AskChatbotCommandHandler : IRequestHandler<AskChatbotCommand, ChatResponseDto>
{
    private readonly IGroqChatbotProvider _chatbotProvider;

    public AskChatbotCommandHandler(IGroqChatbotProvider chatbotProvider)
    {
        _chatbotProvider = chatbotProvider;
    }

    public async Task<ChatResponseDto> Handle(AskChatbotCommand request, CancellationToken cancellationToken)
    {
        var response = await _chatbotProvider.GetChatResponseAsync(request.Message, cancellationToken);
        return new ChatResponseDto(response);
    }
}
