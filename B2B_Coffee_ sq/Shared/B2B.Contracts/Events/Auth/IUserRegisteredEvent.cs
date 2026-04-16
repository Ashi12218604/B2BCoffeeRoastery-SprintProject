using System;

namespace B2B.Contracts.Events.Auth;

public interface IUserRegisteredEvent
{
    Guid UserId { get; }
    string Email { get; }
    string FullName { get; }
    string OtpCode { get; }
    DateTime RegisteredAt { get; }
}