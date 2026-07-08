import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';

const TOKEN_KEY = 'jwt_token';
const USER_KEY = 'current_user';

export interface AuthUser {
  id: number;
  phoneNumber: string;
  fullName: string;
  role?: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());
  private currentUser = new BehaviorSubject<AuthUser | null>(this.readUser());

  isLoggedIn$: Observable<boolean> = this.loggedIn.asObservable();
  currentUser$: Observable<AuthUser | null> = this.currentUser.asObservable();

  constructor(private http: HttpClient) {}

  private jsonHeaders(): HttpHeaders {
    return new HttpHeaders({ 'Content-Type': 'application/json' });
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(TOKEN_KEY);
  }

  private readUser(): AuthUser | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) as AuthUser : null;
  }

  sendOtp(phone: string): Observable<any> {
    return this.http.post<any>('/api/auth/send-otp', { phoneNumber: phone }, { headers: this.jsonHeaders() });
  }

  verifyOtp(phone: string, code: string): Observable<any> {
    return this.http.post<any>('/api/auth/verify-otp', { phoneNumber: phone, code }, { headers: this.jsonHeaders() })
      .pipe(tap(res => this.handleAuthSuccess(res)));
  }

  loginWithPassword(phone: string, password: string): Observable<any> {
    return this.http.post<any>('/api/auth/login-password', { phoneNumber: phone, password }, { headers: this.jsonHeaders() })
      .pipe(tap(res => this.handleAuthSuccess(res)));
  }

  register(data: { phoneNumber: string; fullName: string }): Observable<any> {
    return this.http.post<any>('/api/auth/register', data, { headers: this.jsonHeaders() })
      .pipe(tap(res => this.handleAuthSuccess(res)));
  }

  setPassword(userId: number, password: string): Observable<any> {
    return this.http.post<any>('/api/auth/set-password', { userId, password }, { headers: this.jsonHeaders() });
  }

  private handleAuthSuccess(res: any): void {
    if (res && res.token) {
      localStorage.setItem(TOKEN_KEY, res.token);
      if (res.user) {
        localStorage.setItem(USER_KEY, JSON.stringify(res.user));
        this.currentUser.next(res.user as AuthUser);
      }
      this.loggedIn.next(true);
    }
  }

  getCurrentUser(): AuthUser | null {
    return this.currentUser.value;
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.currentUser.next(null);
    this.loggedIn.next(false);
  }

  isAuthenticated(): boolean {
    return this.hasToken();
  }
}
