import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { CardModule } from 'primeng/card';

interface CartItem {
  id: number;
  productId: number;
  productName: string;
  productImage: string;
  price: number;
  quantity: number;
  totalPrice: number;
}

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, InputNumberModule, CardModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cartItems: CartItem[] = [];
  totalAmount: number = 0;
  loading: boolean = false;

  constructor(
    private cartService: CartService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.loading = true;
    // Assuming campaignId is stored in localStorage or retrieved from route
    const campaignId = 1; // TODO: Get from actual context
    this.cartService.getCart(campaignId).subscribe({
      next: (data: any) => {
        this.cartItems = data.items || [];
        this.totalAmount = data.totalAmount || 0;
        this.loading = false;
      },
      error: (err: any) => {
        console.error('Error loading cart:', err);
        this.loading = false;
      }
    });
  }

  updateQuantity(item: CartItem, newQuantity: number): void {
    if (newQuantity < 1) return;
    
    this.cartService.updateQuantity(item.id, newQuantity).subscribe({
      next: () => {
        item.quantity = newQuantity;
        item.totalPrice = item.price * newQuantity;
        this.calculateTotal();
      },
      error: (err: any) => {
        console.error('Error updating quantity:', err);
      }
    });
  }

  removeFromCart(itemId: number): void {
    if (!confirm('آیا از حذف این محصول مطمئن هستید؟')) return;
    
    this.cartService.removeFromCart(itemId).subscribe({
      next: () => {
        this.cartItems = this.cartItems.filter(item => item.id !== itemId);
        this.calculateTotal();
      },
      error: (err: any) => {
        console.error('Error removing item:', err);
      }
    });
  }

  calculateTotal(): void {
    this.totalAmount = this.cartItems.reduce((sum, item) => sum + item.totalPrice, 0);
  }

  proceedToCheckout(): void {
    if (this.cartItems.length === 0) {
      alert('سبد خرید شما خالی است.');
      return;
    }
    this.router.navigate(['/checkout']);
  }
}
