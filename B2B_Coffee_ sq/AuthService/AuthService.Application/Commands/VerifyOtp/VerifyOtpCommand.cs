using MediatR;

namespace AuthService.Application.Commands.VerifyOtp;

public record VerifyOtpCommand(string Email, string OtpCode) : IRequest<VerifyOtpResult>;
public record VerifyOtpResult(bool Success, string Message);