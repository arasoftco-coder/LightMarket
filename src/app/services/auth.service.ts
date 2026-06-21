import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private http: HttpClient) {}
  
  sendOtp(phone: string): Observable<any> { 
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const body = { phoneNumber: phone };
    console.log('Sending OTP request with body:', body);
    return this.http.post<any>('/api/auth/send-otp', body, { headers }); 
  }
  
  verifyOtp(phone: string, code: string): Observable<any> { 
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const body = { phoneNumber: phone, code: code };
    console.log('Verifying OTP with body:', body);
    return this.http.post<any>('/api/auth/verify-otp', body, { headers }); 
  }
  
  register(data: any): Observable<any> { 
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    console.log('Registering with data:', data);
    return this.http.post<any>('/api/auth/register', data, { headers }); 
  }
  
  logout(): void { localStorage.removeItem('token'); }
  isAuthenticated(): boolean { return !!localStorage.getItem('token'); }
}