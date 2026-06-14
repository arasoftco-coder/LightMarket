import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { OrderService } from '../../services/order.service';
import { CartService } from '../../services/cart.service';
import { ButtonModule } from 'primeng/button';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DropdownModule } from 'primeng/dropdown';
import { CardModule } from 'primeng/card';
import { RadioButtonModule } from 'primeng/radiobutton';

interface Address {
  id: number;
  province: string;
  city: string;
  street: string;
  postalCode: string;
  fullAddress: string;
}

interface ShippingMethod {
  id: number;
  name: string;
  cost: number;
  estimatedDays: string;
}

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, InputTextareaModule, DropdownModule, CardModule, RadioButtonModule],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit {
  step: number = 1;
  addresses: Address[] = [];
  selectedAddressId: number | null = null;
  shippingMethods: ShippingMethod[] = [
    { id: 1, name: 'پست پیشتاز', cost: 50000, estimatedDays: '۲-۳ روز کاری' },
    { id: 2, name: 'پیک موتوری', cost: 80000, estimatedDays: '۱ روز کاری' },
    { id: 3, name: 'ارسال رایگان', cost: 0, estimatedDays: '۳-۵ روز کاری' }
  ];
  selectedShippingMethodId: number | null = null;
  
  cartItems: any[] = [];
  subtotal: number = 0;
  shippingCost: number = 0;
  totalAmount: number = 0;
  
  loading: boolean = false;
  showNewAddressForm: boolean = false;
  newAddress: Partial<Address> = {};

  constructor(
    private userService: UserService,
    private cartService: CartService,
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAddresses();
    this.loadCartSummary();
  }

  loadAddresses(): void {
    this.userService.getAddresses().subscribe({
      next: (data) => {
        this.addresses = data;
        if (data.length > 0) {
          this.selectedAddressId = data[0].id;
        }
      },
      error: (err) => console.error('Error loading addresses:', err)
    });
  }

  loadCartSummary(): void {
    const campaignId = 1; // TODO: Get from context
    this.cartService.getCart(campaignId).subscribe({
      next: (data) => {
        this.cartItems = data.items || [];
        this.subtotal = data.totalAmount || 0;
        this.calculateTotal();
      },
      error: (err) => console.error('Error loading cart:', err)
    });
  }

  calculateTotal(): void {
    this.totalAmount = this.subtotal + this.shippingCost;
  }

  onShippingMethodChange(): void {
    const method = this.shippingMethods.find(m => m.id === this.selectedShippingMethodId);
    this.shippingCost = method ? method.cost : 0;
    this.calculateTotal();
  }

  addNewAddress(): void {
    if (!this.newAddress.province || !this.newAddress.city || !this.newAddress.street) {
      alert('لطفاً تمام فیلدها را پر کنید.');
      return;
    }

    const addressData = {
      ...this.newAddress,
      fullAddress: `${this.newAddress.province}, ${this.newAddress.city}, ${this.newAddress.street}`
    };

    this.userService.addAddress(addressData).subscribe({
      next: (newAddr) => {
        this.addresses.push(newAddr);
        this.selectedAddressId = newAddr.id;
        this.showNewAddressForm = false;
        this.newAddress = {};
      },
      error: (err) => console.error('Error adding address:', err)
    });
  }

  createOrder(): void {
    if (!this.selectedAddressId) {
      alert('لطفاً یک آدرس انتخاب کنید.');
      return;
    }
    if (!this.selectedShippingMethodId) {
      alert('لطفاً روش ارسال را انتخاب کنید.');
      return;
    }

    const orderData = {
      addressId: this.selectedAddressId,
      shippingMethodId: this.selectedShippingMethodId,
      paymentMethod: 'Online'
    };

    this.loading = true;
    this.orderService.createOrder(orderData).subscribe({
      next: (order) => {
        this.loading = false;
        // Redirect to payment or success page
        this.router.navigate(['/payment', order.id]);
      },
      error: (err) => {
        this.loading = false;
        console.error('Error creating order:', err);
        alert('خطا در ثبت سفارش. لطفاً دوباره تلاش کنید.');
      }
    });
  }

  goToStep(step: number): void {
    if (step < this.step) {
      this.step = step;
    }
  }
}
