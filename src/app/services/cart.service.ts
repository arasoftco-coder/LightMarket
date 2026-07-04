import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class CartService {
  constructor(private http: HttpClient) {}

  getCart(): Observable<any> { 
    return this.http.get<any>('/api/cart').pipe(
      map(res => res.data)
    ); 
  }

  addToCart(productId: number, qty: number): Observable<any> { 
    return this.http.post<any>('/api/cart', { productId, qty }).pipe(
      map(res => res.data)
    ); 
  }

  updateQuantity(itemId: number, qty: number): Observable<any> { 
    return this.http.put<any>(`/api/cart/${itemId}`, { qty }).pipe(
      map(res => res.data)
    ); 
  }

  removeFromCart(itemId: number): Observable<any> { 
    return this.http.delete<any>(`/api/cart/${itemId}`).pipe(
      map(res => res.success)
    ); 
  }
}