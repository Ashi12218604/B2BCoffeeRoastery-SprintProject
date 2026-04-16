using System;

namespace B2B.Common.Models;

public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // We only show this in development, never in production (Security)
    public string? DebugInfo { get; set; } 
}
