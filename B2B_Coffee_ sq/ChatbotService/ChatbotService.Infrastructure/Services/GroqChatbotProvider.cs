using ChatbotService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatbotService.Infrastructure.Services;

public class GroqChatbotProvider : IGroqChatbotProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public GroqChatbotProvider(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["Groq:ApiKey"] ?? "";
        _model = config["Groq:Model"] ?? "llama3-8b-8192";
    }


    public async Task<string> GetChatResponseAsync(string message, CancellationToken ct = default)
    {
        string inventoryInfo = await FetchInventoryStatusAsync(ct);

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = $@"You are the official Coffee Assistant for Ember & Bean B2B Roastery.
Be professional, helpful, and concise.

Current REAL-TIME Inventory & Pricing:
{inventoryInfo}

STRICT RULES:
1. Always use INR (₹) and 1000g. 
2. Never use USD ($) or weight in pounds (lb).
3. If an item is low stock or out of stock, mention that clearly.
4. Keep the introduction short and professional." },
                new { role = "user", content = message }
            },
            temperature = 0.2,
            max_tokens = 400
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
        requestMessage.Headers.Add("Authorization", $"Bearer {_apiKey}");
        requestMessage.Content = content;

        var response = await _httpClient.SendAsync(requestMessage, ct);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            Console.WriteLine($"[Groq Error] Status: {response.StatusCode}, Content: {errorContent}");
            return "Sorry, I am currently unable to process your request.";
        }

        var responseString = await response.Content.ReadAsStringAsync(ct);
        using var jsonDoc = JsonDocument.Parse(responseString);
        var choices = jsonDoc.RootElement.GetProperty("choices");
        
        if (choices.GetArrayLength() > 0)
        {
            var messageContent = choices[0].GetProperty("message").GetProperty("content").GetString();
            return messageContent ?? string.Empty;
        }

        return string.Empty;
    }

    private async Task<string> FetchInventoryStatusAsync(CancellationToken ct)
    {
        try
        {
            // Internal call to InventoryService (using 127.0.0.1 to avoid localhost resolution delays/issues)
            var response = await _httpClient.GetAsync("http://127.0.0.1:5003/api/inventory/status", ct);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Inventory Fetch Warning] Status: {response.StatusCode}");
                return "Inventory data temporarily unavailable.";
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            using var jsonDoc = JsonDocument.Parse(content);
            
            // Handle both wrapped { "value": [] } and direct [] responses
            var items = jsonDoc.RootElement.ValueKind == JsonValueKind.Array 
                ? jsonDoc.RootElement 
                : jsonDoc.RootElement.GetProperty("value");
            
            var sb = new StringBuilder();
            foreach (var item in items.EnumerateArray())
            {
                var name = item.GetProperty("productName").GetString();
                var stock = item.GetProperty("quantityAvailable").GetInt32();
                var isLow = item.GetProperty("isLowStock").GetBoolean();
                
                sb.AppendLine($"- {name}: {stock} units available {(isLow ? "[LOW STOCK]" : "")}");
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Inventory Fetch Error] {ex.GetType().Name}: {ex.Message}");
            return "Inventory data temporarily unavailable.";
        }
    }
}
