import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CartService {
  constructor(private http: HttpClient) {}
  getCart(): Observable<any> { return this.http.get<any>('/api/cart'); }
  addToCart(productId: number, qty: number): Observable<any> { return this.http.post<any>('/api/cart', { productId, qty }); }
  updateQuantity(itemId: number, qty: number): Observable<any> { return this.http.put<any>(`/api/cart/${itemId}`, { qty }); }
  removeFromCart(itemId: number): Observable<any> { return this.http.delete<any>(`/api/cart/${itemId}`); }
}