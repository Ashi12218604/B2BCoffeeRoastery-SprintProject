using ChatbotService.Application.DTOs;
using MediatR;

namespace ChatbotService.Application.Commands;

public record AskChatbotCommand(string Message) : IRequest<ChatResponseDto>;
