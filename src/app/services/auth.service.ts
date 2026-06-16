import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = '/api/auth';
  constructor(private http: HttpClient) {}
  sendOtp(phone: string): Observable<any> { return this.http.post(`${this.apiUrl}/send-otp`, { phoneNumber: phone }); }
  verifyOtp(phone: string, code: string): Observable<any> { return this.http.post(`${this.apiUrl}/verify-otp`, { phoneNumber: phone, code }).pipe(tap((response: any) => { if (response.token) { localStorage.setItem('jwt_token', response.token); } })); }
  register(phone: string, fullName: string): Observable<any> { return this.http.post(`${this.apiUrl}/register`, { phoneNumber: phone, fullName }); }
  logout(): void { localStorage.removeItem('jwt_token'); }
  isAuthenticated(): boolean { return !!localStorage.getItem('jwt_token'); }
}
