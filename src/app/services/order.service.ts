import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class OrderService {
  constructor(private http: HttpClient) {}

  createOrder(data: any): Observable<any> { 
    return this.http.post<any>('/api/orders', data).pipe(
      map(res => res.data)
    ); 
  }

  getUserOrders(): Observable<any> { 
    return this.http.get<any>('/api/orders').pipe(
      map(res => res.data)
    ); 
  }

  getOrderDetails(orderId: number): Observable<any> { 
    return this.http.get<any>(`/api/orders/${orderId}`).pipe(
      map(res => res.data)
    ); 
  }

  validateMagicLink(token: string): Observable<any> { 
    return this.http.get<any>(`/api/orders/magic-link/${token}`).pipe(
      map(res => res.data)
    ); 
  }

  createPaymentRequest(orderId: number, amount: number): Observable<any> {
    return this.http.post<any>('/api/payment/create-request', { orderId, amount });
  }

  verifyPayment(authority: string, status: string): Observable<any> {
    return this.http.post<any>('/api/payment/verify', { authority, status });
  }

  confirmPayment(orderId: number, trackingCode: string): Observable<any> {
    return this.http.post<any>(`/api/orders/${orderId}/confirm-payment`, { trackingCode }).pipe(
      map(res => res.data)
    );
  }

  getPublicShippingMethods(addressId: number, campaignId: number): Observable<any> {
    return this.http.get<any>(`/api/admin/shipping-methods/public?addressId=${addressId}&campaignId=${campaignId}`).pipe(
      map(res => res.data)
    );
  }
}