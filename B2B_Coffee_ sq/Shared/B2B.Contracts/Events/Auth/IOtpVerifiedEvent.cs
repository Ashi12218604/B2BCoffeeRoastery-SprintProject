using System;

namespace B2B.Contracts.Events.Auth;

public interface IOtpVerifiedEvent
{
    Guid UserId { get; }
    string Email { get; }
    DateTime VerifiedAt { get; }
}