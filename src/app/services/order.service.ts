import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = '/api/orders';

  constructor(private http: HttpClient) {}

  createOrder(cartId: number, addressId: number, shippingMethod: string, paymentMethod: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/create`, { cartId, addressId, shippingMethod, paymentMethod });
  }

  getUserOrders(userId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/user/${userId}`);
  }

  getOrderDetails(orderId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${orderId}`);
  }
}
