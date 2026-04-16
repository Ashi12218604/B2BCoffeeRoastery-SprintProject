import { createReducer, on } from '@ngrx/store';
import { AuthState } from '../app.state';
import * as AuthActions from './auth.actions';

export const initialAuthState: AuthState = {
  user: null,
  token: null,
  loading: false,
  error: null,
  otpEmail: null,
  registrationSuccess: false,
  otpVerified: false
};

export const authReducer = createReducer(
  initialAuthState,

  // Login
  on(AuthActions.login, (state) => ({
    ...state, loading: true, error: null
  })),
  on(AuthActions.loginSuccess, (state, { token, user }) => ({
    ...state, token, user, loading: false, error: null
  })),
  on(AuthActions.loginFailure, (state, { error }) => ({
    ...state, loading: false, error
  })),

  // Register
  on(AuthActions.register, (state) => ({
    ...state, loading: true, error: null, registrationSuccess: false
  })),
  on(AuthActions.registerSuccess, (state, { email }) => ({
    ...state, loading: false, otpEmail: email, registrationSuccess: true
  })),
  on(AuthActions.registerFailure, (state, { error }) => ({
    ...state, loading: false, error, registrationSuccess: false
  })),

  // Verify OTP
  on(AuthActions.verifyOtp, (state) => ({
    ...state, loading: true, error: null
  })),
  on(AuthActions.verifyOtpSuccess, (state) => ({
    ...state, loading: false, otpVerified: true
  })),
  on(AuthActions.verifyOtpFailure, (state, { error }) => ({
    ...state, loading: false, error
  })),

  // Logout
  on(AuthActions.logout, () => initialAuthState),

  // Auto Login
  on(AuthActions.autoLoginSuccess, (state, { token, user }) => ({
    ...state, token, user
  })),
  on(AuthActions.autoLoginFailure, () => initialAuthState),

  // Clear Error
  on(AuthActions.clearAuthError, (state) => ({
    ...state, error: null
  }))
);
