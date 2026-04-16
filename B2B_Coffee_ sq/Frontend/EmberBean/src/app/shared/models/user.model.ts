// ═══════════════════════════════════════════════════════════
// EMBER & BEAN — TypeScript Models
// ═══════════════════════════════════════════════════════════

// ── Auth Models ──────────────────────────────────────────
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  phoneNumber: string;
  companyName: string;
}

export interface VerifyOtpRequest {
  email: string;
  otpCode: string;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  token?: string;
}

export interface DecodedToken {
  sub: string;            // UserId (Guid)
  email: string;
  name: string;
  role: string;           // 'SuperAdmin' | 'Admin' | 'Client'
  status: string;         // 'Approved' | 'Pending' | 'PendingOtp' | 'Rejected'
  companyName?: string;
  exp: number;
  iss: string;
  aud: string;
}

export interface User {
  id: string;
  email: string;
  fullName: string;
  role: 'SuperAdmin' | 'Admin' | 'Client';
  status: 'PendingOtp' | 'Pending' | 'Approved' | 'Rejected';
  companyName?: string;
}

// ── Pending Client (Admin view) ─────────────────────────
export interface PendingClient {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  companyName: string;
  status: string;
  createdAt: string;
}

export interface ApproveClientRequest {
  approve: boolean;
  rejectionReason?: string;
}
