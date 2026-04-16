using System;

namespace B2B.Contracts.Events.Auth;

public interface IUserApprovedEvent
{
    Guid UserId { get; }
    string Email { get; }
    string FullName { get; }
    DateTime ApprovedAt { get; }
}