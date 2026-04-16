using MediatR;
using System.Collections.Generic;

namespace AuthService.Application.Queries.GetAdmins;

public record GetAdminsQuery() : IRequest<List<AdminDto>>;

public record AdminDto(
    System.Guid Id, 
    string FullName, 
    string Email, 
    string PhoneNumber, 
    System.DateTime CreatedAt);
