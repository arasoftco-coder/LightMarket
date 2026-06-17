import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserService {
  constructor(private http: HttpClient) {}
  getProfile(): Observable<any> { return this.http.get<any>('/api/user/profile'); }
  updateProfile(data: any): Observable<any> { return this.http.put<any>('/api/user/profile', data); }
  getAddresses(): Observable<any> { return this.http.get<any>('/api/user/addresses'); }
  addAddress(data: any): Observable<any> { return this.http.post<any>('/api/user/addresses', data); }
  updateAddress(id: number, data: any): Observable<any> { return this.http.put<any>(`/api/user/addresses/${id}`, data); }
  deleteAddress(id: number): Observable<any> { return this.http.delete<any>(`/api/user/addresses/${id}`); }
}