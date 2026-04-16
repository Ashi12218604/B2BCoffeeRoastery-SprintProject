using System.Threading;
using System.Threading.Tasks;

namespace ChatbotService.Application.Interfaces;

public interface IGroqChatbotProvider
{
    Task<string> GetChatResponseAsync(string message, CancellationToken ct = default);
}
