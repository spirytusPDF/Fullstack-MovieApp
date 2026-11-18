import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, finalize, of, tap } from 'rxjs';
import { AuthResponse, UserLogin, UserProfile, UserRegister, UserResponse, UserUpdate } from '../../DTOs/user.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = '/api/auth';
  private readonly tokenStorageKey = 'movies.auth.token';
  private readonly loginOverrideStoragePrefix = 'movies.auth.login.';
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

  getCurrentProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/me`);
  }

  updateAccount(contract: UserUpdate): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/update`, contract).pipe(
      tap(() => {
        const user = this.currentUser();
        if (user && contract.NewLogin) {
          localStorage.setItem(`${this.loginOverrideStoragePrefix}${user.id}`, contract.NewLogin);
          this.currentUser.set({ ...user, login: contract.NewLogin });
        }
      })
    );
  }

  deleteAccount(contract: UserLogin): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/delete`, { body: contract }).pipe(
      tap(() => this.clearSession())
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
    const user = this.userFromToken(token, false);
    if (!user) {
      throw new Error('The authentication response did not contain a valid JWT.');
    }

    localStorage.removeItem(`${this.loginOverrideStoragePrefix}${user.id}`);
    localStorage.setItem(this.tokenStorageKey, token);
    this.currentUser.set(user);
  }

  private clearSession(): void {
    const userId = this.currentUser()?.id;
    if (userId) {
      localStorage.removeItem(`${this.loginOverrideStoragePrefix}${userId}`);
    }
    localStorage.removeItem(this.tokenStorageKey);
    this.currentUser.set(null);
  }

  private userFromToken(token: string, useLoginOverride = true): UserResponse | null {
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

      return {
        id,
        login: useLoginOverride
          ? localStorage.getItem(`${this.loginOverrideStoragePrefix}${id}`) ?? login
          : login,
        email,
        roles
      };
    } catch {
      return null;
    }
  }
}

export function getAuthErrorMessage(error: HttpErrorResponse, fallback: string): string {
  let body: unknown = error.error;
  if (typeof body === 'string') {
    const textBody = body;
    try {
      body = JSON.parse(textBody);
    } catch {
      return textBody.trim() || fallback;
    }
  }

  if (typeof body === 'object' && body !== null) {
    const response = body as Record<string, unknown>;
    const errors = response['errors'] ?? response['Errors'];
    const errorItems = Array.isArray(errors)
      ? errors
      : typeof errors === 'object' && errors !== null
        ? Object.values(errors)
        : [];
    const messages = errorItems.flatMap((item) => {
      if (typeof item === 'string') return [item];
      if (Array.isArray(item)) return item.filter((message): message is string => typeof message === 'string');
      if (typeof item !== 'object' || item === null) return [];

      const errorItem = item as Record<string, unknown>;
      const message = errorItem['errorMessage'] ?? errorItem['ErrorMessage'] ?? errorItem['message'];
      return typeof message === 'string' ? [message] : [];
    }).filter((message) => message.length > 0);

    if (messages.length > 0) return messages.join(' ');

    for (const key of ['message', 'detail', 'title', 'Message', 'Detail', 'Title']) {
      const message = response[key];
      if (typeof message === 'string' && message.length > 0) return message;
    }
  }

  if (error.status === 0) return 'Could not contact the server. Check your connection and try again.';
  if (error.status === 401) return 'Your session is no longer authorized. Sign in again and retry.';
  return fallback;
}