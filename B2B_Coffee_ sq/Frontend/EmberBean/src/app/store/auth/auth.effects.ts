import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { map, exhaustMap, catchError, tap } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';
import * as AuthActions from './auth.actions';
import { User } from '../../shared/models/user.model';

@Injectable()
export class AuthEffects {
  private actions$ = inject(Actions);
  private authService = inject(AuthService);
  private router = inject(Router);

  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.login),
      exhaustMap(({ request }) =>
        this.authService.login(request).pipe(
          map(response => {
            if (response.success && response.token) {
              this.authService.storeToken(response.token);
              const user = this.decodeTokenToUser(response.token);
              return AuthActions.loginSuccess({ token: response.token, user });
            }
            return AuthActions.loginFailure({ error: response.message || 'Login failed' });
          }),
          catchError(err => of(AuthActions.loginFailure({
            error: err.error?.message || 'Login failed. Please check your credentials.'
          })))
        )
      )
    )
  );

  loginRedirect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loginSuccess),
      tap(({ user }) => {
        if (user.role === 'Admin' || user.role === 'SuperAdmin') {
          this.router.navigate(['/admin']);
        } else {
          this.router.navigate(['/client']);
        }
      })
    ), { dispatch: false }
  );

  register$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.register),
      exhaustMap(({ request }) =>
        this.authService.register(request).pipe(
          map(response => {
            if (response.success) {
              return AuthActions.registerSuccess({
                email: request.email,
                message: response.message
              });
            }
            return AuthActions.registerFailure({ error: response.message });
          }),
          catchError(err => of(AuthActions.registerFailure({
            error: err.error?.message || 'Registration failed.'
          })))
        )
      )
    )
  );

  registerRedirect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.registerSuccess),
      tap(() => this.router.navigate(['/verify-otp']))
    ), { dispatch: false }
  );

  verifyOtp$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.verifyOtp),
      exhaustMap(({ request }) =>
        this.authService.verifyOtp(request).pipe(
          map(response => {
            if (response.success) {
              return AuthActions.verifyOtpSuccess({ message: response.message });
            }
            return AuthActions.verifyOtpFailure({ error: response.message });
          }),
          catchError(err => of(AuthActions.verifyOtpFailure({
            error: err.error?.message || 'OTP verification failed.'
          })))
        )
      )
    )
  );

  otpVerifiedRedirect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.verifyOtpSuccess),
      tap(() => this.router.navigate(['/login']))
    ), { dispatch: false }
  );

  logout$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.logout),
      tap(() => {
        this.authService.removeToken();
        this.router.navigate(['/login']);
      })
    ), { dispatch: false }
  );

  autoLogin$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.autoLogin),
      map(() => {
        const token = this.authService.getToken();
        if (token && this.authService.isLoggedIn()) {
          const user = this.decodeTokenToUser(token);
          return AuthActions.autoLoginSuccess({ token, user });
        }
        return AuthActions.autoLoginFailure();
      })
    )
  );

  // ── Helpers ─────────────────────────────────────────────
  private decodeTokenToUser(token: string): User {
    const payload = JSON.parse(atob(token.split('.')[1])) as any;

    // .NET JWT uses various claim keys
    const id = payload.sub || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || '';
    const email = payload.email || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || '';
    const name = payload.name || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || '';
    const role = payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'Client';

    return {
      id,
      email,
      fullName: name,
      role: role as any,
      status: payload.status || 'Approved',
      companyName: payload.companyName
    };
  }
}
