import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OrderService {
  constructor(private http: HttpClient) {}
  createOrder(data: any): Observable<any> { return this.http.post<any>('/api/orders', data); }
  getUserOrders(): Observable<any> { return this.http.get<any>('/api/orders'); }
  validateMagicLink(token: string): Observable<any> { return this.http.get<any>(`/api/orders/magic-link/${token}`); }
}