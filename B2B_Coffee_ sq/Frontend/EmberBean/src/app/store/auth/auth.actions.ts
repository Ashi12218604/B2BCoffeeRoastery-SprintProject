import { createAction, props } from '@ngrx/store';
import { LoginRequest, RegisterRequest, VerifyOtpRequest, User, AuthResponse, PendingClient, ApproveClientRequest } from '../../shared/models/user.model';

// ── Login ─────────────────────────────────────────────────
export const login = createAction(
  '[Auth] Login',
  props<{ request: LoginRequest }>()
);

export const loginSuccess = createAction(
  '[Auth] Login Success',
  props<{ token: string; user: User }>()
);

export const loginFailure = createAction(
  '[Auth] Login Failure',
  props<{ error: string }>()
);

// ── Register ──────────────────────────────────────────────
export const register = createAction(
  '[Auth] Register',
  props<{ request: RegisterRequest }>()
);

export const registerSuccess = createAction(
  '[Auth] Register Success',
  props<{ email: string; message: string }>()
);

export const registerFailure = createAction(
  '[Auth] Register Failure',
  props<{ error: string }>()
);

// ── Verify OTP ────────────────────────────────────────────
export const verifyOtp = createAction(
  '[Auth] Verify OTP',
  props<{ request: VerifyOtpRequest }>()
);

export const verifyOtpSuccess = createAction(
  '[Auth] Verify OTP Success',
  props<{ message: string }>()
);

export const verifyOtpFailure = createAction(
  '[Auth] Verify OTP Failure',
  props<{ error: string }>()
);

// ── Logout ────────────────────────────────────────────────
export const logout = createAction('[Auth] Logout');

// ── Auto-login (from stored token) ───────────────────────
export const autoLogin = createAction('[Auth] Auto Login');

export const autoLoginSuccess = createAction(
  '[Auth] Auto Login Success',
  props<{ token: string; user: User }>()
);

export const autoLoginFailure = createAction('[Auth] Auto Login Failure');

// ── Clear Error ───────────────────────────────────────────
export const clearAuthError = createAction('[Auth] Clear Error');
