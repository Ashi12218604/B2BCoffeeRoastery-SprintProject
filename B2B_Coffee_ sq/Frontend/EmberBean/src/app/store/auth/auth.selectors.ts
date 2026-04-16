import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AuthState } from '../app.state';

export const selectAuthState = createFeatureSelector<AuthState>('auth');

export const selectUser = createSelector(
  selectAuthState,
  (state) => state.user
);

export const selectToken = createSelector(
  selectAuthState,
  (state) => state.token
);

export const selectIsAuthenticated = createSelector(
  selectAuthState,
  (state) => !!state.token && !!state.user
);

export const selectAuthLoading = createSelector(
  selectAuthState,
  (state) => state.loading
);

export const selectAuthError = createSelector(
  selectAuthState,
  (state) => state.error
);

export const selectOtpEmail = createSelector(
  selectAuthState,
  (state) => state.otpEmail
);

export const selectRegistrationSuccess = createSelector(
  selectAuthState,
  (state) => state.registrationSuccess
);

export const selectOtpVerified = createSelector(
  selectAuthState,
  (state) => state.otpVerified
);

export const selectUserRole = createSelector(
  selectUser,
  (user) => user?.role ?? null
);

export const selectIsAdmin = createSelector(
  selectUser,
  (user) => user?.role === 'Admin' || user?.role === 'SuperAdmin'
);

export const selectIsClient = createSelector(
  selectUser,
  (user) => user?.role === 'Client'
);
