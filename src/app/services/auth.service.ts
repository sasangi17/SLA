
import {
  Injectable,
  Inject,
  PLATFORM_ID
} from '@angular/core';

import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface LoginRequest {
  email: string;
  password: string;
  selectedRole?: string;
}

export interface LoginResponse {
  requiresRoleSelection?: boolean;
  roles?: string[];
  token?: string;
  id?: number;
  staffId?: number;
  email?: string;
  fullName?: string;
  role?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7297/api/auth';

  private logoutTimer: ReturnType<typeof setTimeout> | null = null;

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    if (isPlatformBrowser(this.platformId)) {
      this.checkTokenExpiration();
    }
  }

  login(data: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/login`, data)
      .pipe(
        tap(response => {
          // Multiple-role accounts may need to select a role first.
          // Do not save an incomplete login response.
          if (
            response.requiresRoleSelection ||
            !response.token ||
            !response.role
          ) {
            return;
          }

          if (isPlatformBrowser(this.platformId)) {
            const loggedInUser = {
              userId: response.id,
              staffId: response.staffId,
              email: response.email,
              fullName: response.fullName,
              role: response.role,
              roles: response.roles ?? [response.role]
            };

            localStorage.setItem(
              'loggedInUser',
              JSON.stringify(loggedInUser)
            );

            localStorage.setItem('token', response.token);
          }

          this.startLogoutTimer(response.token!);
        })
      );
  }

  getCurrentRole(): string | null {
    const user = this.getLoggedInUser();
    const role = user?.role ?? user?.Role;

    return typeof role === 'string' && role.trim()
      ? role.trim()
      : null;
  }

  hasRole(role: string): boolean {
    return this.getCurrentRole()?.toLowerCase() ===
      role.toLowerCase();
  }

  getLoggedInUser(): any {
    if (!isPlatformBrowser(this.platformId)) {
      return null;
    }

    const user = localStorage.getItem('loggedInUser');

    if (!user) {
      return null;
    }

    try {
      return JSON.parse(user);
    } catch {
      return null;
    }
  }

  getUserId(): number | null {
    const user = this.getLoggedInUser();
    return user?.userId ?? user?.id ?? null;
  }

  getToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) {
      return null;
    }

    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    const token = this.getToken();

    if (!token) {
      return false;
    }

    try {
      const payload = this.decodeToken(token);

      return !!payload.exp &&
        payload.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }

  logout(): void {
    if (this.logoutTimer) {
      clearTimeout(this.logoutTimer);
      this.logoutTimer = null;
    }

    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('loggedInUser');
      localStorage.removeItem('token');
    }
  }

  private checkTokenExpiration(): void {
    const token = this.getToken();

    if (token) {
      this.startLogoutTimer(token);
    }
  }

  private startLogoutTimer(token: string): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    if (this.logoutTimer) {
      clearTimeout(this.logoutTimer);
      this.logoutTimer = null;
    }

    try {
      const payload = this.decodeToken(token);

      if (!payload.exp) {
        this.logout();
        return;
      }

      const timeRemaining = payload.exp * 1000 - Date.now();

      if (timeRemaining <= 0) {
        this.logout();
        window.location.href = '/';
        return;
      }

      this.logoutTimer = setTimeout(() => {
        this.logout();
        window.location.href = '/';
      }, timeRemaining);
    } catch (error) {
      console.error('Invalid JWT:', error);
      this.logout();
      window.location.href = '/';
    }
  }

  private decodeToken(token: string): any {
    const parts = token.split('.');

    if (parts.length !== 3) {
      throw new Error('Invalid JWT format.');
    }

    const base64 = parts[1]
      .replace(/-/g, '+')
      .replace(/_/g, '/');

    const padded = base64 +
      '='.repeat((4 - base64.length % 4) % 4);

    return JSON.parse(atob(padded));
  }
}
