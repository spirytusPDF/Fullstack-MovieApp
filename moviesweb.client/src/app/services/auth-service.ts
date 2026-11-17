import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, finalize, of, tap } from 'rxjs';
import { AuthResponse, UserLogin, UserRegister, UserResponse } from '../../DTOs/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = '/api/auth';
  private readonly tokenStorageKey = 'movies.auth.token';
  currentUser = signal<UserResponse | null>(null);

  private http = inject(HttpClient);

  signUp(contract: UserRegister): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/signup`, contract);
  }

  login(contract: UserLogin): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, contract).pipe(
      tap(({ token }) => this.saveToken(token))
    );
  }

  logout(): Observable<void> {
    const userId = this.currentUser()?.id;
    if (!userId) {
      this.clearSession();
      return of(void 0);
    }

    return this.http.post<void>(`${this.apiUrl}/logout`, userId).pipe(
      finalize(() => this.clearSession())
    );
  }

  checkAuth(): Observable<UserResponse | null> {
    const token = localStorage.getItem(this.tokenStorageKey);
    const user = token ? this.userFromToken(token) : null;
    if (user) {
      this.currentUser.set(user);
    } else {
      this.clearSession();
    }

    return of(user);
  }

  getAccessToken(): string | null {
    const token = localStorage.getItem(this.tokenStorageKey);
    if (!token || !this.userFromToken(token)) {
      this.clearSession();
      return null;
    }

    return token;
  }

  private saveToken(token: string): void {
    const user = this.userFromToken(token);
    if (!user) {
      throw new Error('The authentication response did not contain a valid JWT.');
    }

    localStorage.setItem(this.tokenStorageKey, token);
    this.currentUser.set(user);
  }

  private clearSession(): void {
    localStorage.removeItem(this.tokenStorageKey);
    this.currentUser.set(null);
  }

  private userFromToken(token: string): UserResponse | null {
    try {
      const payload = token.split('.')[1];
      if (!payload) return null;

      const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
      const paddedBase64 = base64.padEnd(Math.ceil(base64.length / 4) * 4, '=');
      const bytes = Uint8Array.from(atob(paddedBase64), (character) => character.charCodeAt(0));
      const claims: unknown = JSON.parse(new TextDecoder().decode(bytes));
      if (typeof claims !== 'object' || claims === null) return null;

      const tokenClaims = claims as Record<string, unknown>;
      const id = tokenClaims['sub'];
      const login = tokenClaims['login'];
      const email = tokenClaims['email'];
      const roleClaim = tokenClaims['role'];
      const expiresAt = tokenClaims['exp'];
      if (
        typeof id !== 'string' ||
        typeof login !== 'string' ||
        typeof email !== 'string' ||
        typeof expiresAt !== 'number' ||
        expiresAt <= Math.floor(Date.now() / 1000)
      ) {
        return null;
      }
      
      const roles = typeof roleClaim === 'string'
        ? [roleClaim]
        : Array.isArray(roleClaim)
          ? roleClaim.filter((role): role is string => typeof role === 'string')
          : [];

      return { id, login, email, roles };
    } catch {
      return null;
    }
  }
}

export function getAuthErrorMessage(error: HttpErrorResponse, fallback: string): string {
  const body = error.error as { errors?: unknown; message?: unknown } | null;
  if (Array.isArray(body?.errors)) {
    const messages = body.errors
      .map((item) => {
        if (typeof item === 'string') return item;
        if (typeof item === 'object' && item !== null && 'errorMessage' in item) {
          const message = item.errorMessage;
          return typeof message === 'string' ? message : '';
        }
        return '';
      })
      .filter((message): message is string => message.length > 0);

    if (messages.length > 0) return messages.join(' ');
  }

  return typeof body?.message === 'string' ? body.message : fallback;
}