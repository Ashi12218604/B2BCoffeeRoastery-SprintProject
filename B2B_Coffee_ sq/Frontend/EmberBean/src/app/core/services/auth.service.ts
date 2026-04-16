import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  LoginRequest, RegisterRequest, VerifyOtpRequest,
  AuthResponse, PendingClient, ApproveClientRequest
} from '../../shared/models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = 'http://localhost:7101/api';

  constructor(private http: HttpClient) {}

  // ── Public endpoints ────────────────────────────────────
  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/login`, request);
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/register`, request);
  }

  verifyOtp(request: VerifyOtpRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/verify-otp`, request);
  }

  // ── Admin endpoints ─────────────────────────────────────
  getPendingClients(): Observable<PendingClient[]> {
    return this.http.get<PendingClient[]>(`${this.baseUrl}/admin/pending-clients`);
  }

  approveClient(clientId: string, request: ApproveClientRequest): Observable<AuthResponse> {
    return this.http.put<AuthResponse>(
      `${this.baseUrl}/admin/approve-client/${clientId}`, request
    );
  }

  createAdmin(request: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/admin/create-admin`, request);
  }

  // ── Token management ────────────────────────────────────
  storeToken(token: string): void {
    localStorage.setItem('eb_token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('eb_token');
  }

  removeToken(): void {
    localStorage.removeItem('eb_token');
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }
}
