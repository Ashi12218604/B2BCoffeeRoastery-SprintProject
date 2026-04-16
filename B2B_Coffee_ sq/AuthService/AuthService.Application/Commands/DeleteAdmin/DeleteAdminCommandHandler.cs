using AuthService.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.DeleteAdmin;

public class DeleteAdminCommandHandler : IRequestHandler<DeleteAdminCommand, DeleteAdminResult>
{
    private readonly IAuthDbContext _db;

    public DeleteAdminCommandHandler(IAuthDbContext db)
    {
        _db = db;
    }

    public async Task<DeleteAdminResult> Handle(DeleteAdminCommand request, CancellationToken ct)
    {
        var admin = await _db.Users.FindAsync(new object[] { request.AdminId }, ct);

        if (admin is null)
            return new DeleteAdminResult(false, "Admin not found.");

        if (admin.Role == AuthService.Domain.Enums.UserRole.SuperAdmin)
            return new DeleteAdminResult(false, "Cannot delete SuperAdmin.");

        _db.Users.Remove(admin);
        await _db.SaveChangesAsync(ct);

        return new DeleteAdminResult(true, "Admin deleted successfully.");
    }
}
