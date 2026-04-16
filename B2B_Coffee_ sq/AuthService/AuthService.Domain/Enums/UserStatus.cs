namespace AuthService.Domain.Enums;

public enum UserStatus
{
    PendingOtp = 0,      // Registered, OTP not yet verified
    Pending = 1,          // OTP verified, waiting Admin approval
    Approved = 2,
    Rejected = 3
}