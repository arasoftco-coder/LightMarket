import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl = '/api/users';
  constructor(private http: HttpClient) {}
  getProfile(userId: number): Observable<any> { return this.http.get(`${this.apiUrl}/${userId}/profile`); }
  updateProfile(userId: number, data: any): Observable<any> { return this.http.put(`${this.apiUrl}/${userId}/profile`, data); }
  getAddresses(userId: number): Observable<any> { return this.http.get(`${this.apiUrl}/${userId}/addresses`); }
  addAddress(userId: number, address: any): Observable<any> { return this.http.post(`${this.apiUrl}/${userId}/addresses`, address); }
  updateAddress(addressId: number, address: any): Observable<any> { return this.http.put(`${this.apiUrl}/addresses/${addressId}`, address); }
  deleteAddress(addressId: number): Observable<any> { return this.http.delete(`${this.apiUrl}/addresses/${addressId}`); }
}
