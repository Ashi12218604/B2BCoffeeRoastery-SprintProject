using System;

namespace AuthService.Domain.Entities;

/// <summary>
/// Stored in DB so Gmail credentials can be updated without app restart.
/// </summary>
public class SmtpConfig
{
    public int Id { get; set; }
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;  // Gmail App Password
    public bool EnableSsl { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}