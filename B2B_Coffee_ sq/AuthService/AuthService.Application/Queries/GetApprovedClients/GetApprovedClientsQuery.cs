using MediatR;
using System.Collections.Generic;

namespace AuthService.Application.Queries.GetApprovedClients;

public record GetApprovedClientsQuery() : IRequest<List<ApprovedClientDto>>;

public record ApprovedClientDto(
    Guid Id,
    string FullName,
    string Email,
    string? CompanyName,
    string PhoneNumber,
    DateTime ApprovedAt
);
