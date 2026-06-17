import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private http: HttpClient) {}
  sendOtp(phone: string): Observable<any> { return this.http.post<any>('/api/auth/send-otp', { phone }); }
  verifyOtp(phone: string, code: string): Observable<any> { return this.http.post<any>('/api/auth/verify', { phone, code }); }
  register(data: any): Observable<any> { return this.http.post<any>('/api/auth/register', data); }
  logout(): void { localStorage.removeItem('token'); }
  isAuthenticated(): boolean { return !!localStorage.getItem('token'); }
}