import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl = '/api/cart';

  constructor(private http: HttpClient) {}

  getCart(campaignId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${campaignId}`);
  }

  addToCart(campaignId: number, productId: number, quantity: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/add`, { campaignId, productId, quantity });
  }

  updateQuantity(cartItemId: number, quantity: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/update/${cartItemId}`, { quantity });
  }

  removeFromCart(cartItemId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/remove/${cartItemId}`);
  }
}
