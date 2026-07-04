import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class UserService {
  constructor(private http: HttpClient) {}

  getProfile(): Observable<any> { 
    return this.http.get<any>('/api/user/profile').pipe(
      map(res => res.data)
    ); 
  }

  updateProfile(data: any): Observable<any> { 
    return this.http.put<any>('/api/user/profile', data).pipe(
      map(res => res.data)
    ); 
  }

  getAddresses(): Observable<any> { 
    return this.http.get<any>('/api/user/addresses').pipe(
      map(res => res.data)
    ); 
  }

  addAddress(data: any): Observable<any> { 
    return this.http.post<any>('/api/user/addresses', data).pipe(
      map(res => res.data)
    ); 
  }

  updateAddress(id: number, data: any): Observable<any> { 
    return this.http.put<any>(`/api/user/addresses/${id}`, data).pipe(
      map(res => res.data)
    ); 
  }

  deleteAddress(id: number): Observable<any> { 
    return this.http.delete<any>(`/api/user/addresses/${id}`).pipe(
      map(res => res.success)
    ); 
  }
}