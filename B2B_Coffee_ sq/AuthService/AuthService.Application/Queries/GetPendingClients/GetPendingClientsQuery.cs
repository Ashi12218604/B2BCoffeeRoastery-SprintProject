using MediatR;
using System;
using System.Collections.Generic;

namespace AuthService.Application.Queries.GetPendingClients;

public record GetPendingClientsQuery : IRequest<List<PendingClientDto>>;

public record PendingClientDto(
    Guid Id,
    string FullName,
    string Email,
    string CompanyName,
    string PhoneNumber,
    DateTime CreatedAt
);