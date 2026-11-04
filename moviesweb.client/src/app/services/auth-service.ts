import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, switchMap, tap } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = '/api/auth';
  currentUser = signal<UserResponse | null>(null);

  private http = inject(HttpClient);
  signUp(contract: UserRegister): Observable<any> {
    return this.http.post(`${this.apiUrl}/signup`, contract, { withCredentials: true });
  }

  login(contract: UserLogin): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, contract, { withCredentials: true }).pipe(
      tap(() => this.checkAuth().subscribe()) 
    );
  }

  logout(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/logout`, {}, { withCredentials: true }).pipe(
      tap(() => this.currentUser.set(null))
    );
  }

  checkAuth(): Observable<UserResponse> {
    return this.http.get<UserResponse>(`${this.apiUrl}/me`, { withCredentials: true }).pipe(
      tap({
        next: (user) => this.currentUser.set(user),
        error: () => this.currentUser.set(null)
      })
    );
    }
}