import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { OrderService } from '../services/order.service';
import { CartService } from '../services/cart.service';
import { CampaignService } from '../services/campaign.service';
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
  shippingMethods: any[] = [];
  selectedShippingMethodId: number | null = null;
  
  cartItems: any[] = [];
  subtotal: number = 0;
  shippingCost: number = 0;
  totalAmount: number = 0;
  campaignId: number | null = null;
  activeCampaign: any = null;
  
  loading: boolean = false;
  showNewAddressForm: boolean = false;
  newAddress: Partial<Address> = {};

  provinces: string[] = ['تهران', 'اصفهان', 'خراسان رضوی', 'فارس', 'آذربایجان شرقی', 'مازندران', 'گیلان', 'البرز'];
  cities: string[] = [];
  citiesMap: { [key: string]: string[] } = {
    'تهران': ['تهران', 'ری', 'شمیرانات', 'اسلامشهر', 'شهریار', 'ورامین', 'دماوند', 'پاکدشت'],
    'اصفهان': ['اصفهان', 'کاشان', 'خمینی‌شهر', 'نجف‌آباد', 'شاهین‌شهر', 'شهرضا', 'گلپایگان'],
    'خراسان رضوی': ['مشهد', 'نیشابور', 'سبزوار', 'تربت حیدریه', 'قوچان', 'کاشمر', 'گناباد'],
    'فارس': ['شیراز', 'مرودشت', 'جهرم', 'فسا', 'کازرون', 'لارستان', 'داراب'],
    'آذربایجان شرقی': ['تبریز', 'مراغه', 'مرند', 'میانه', 'اهر', 'بناب'],
    'مازندران': ['ساری', 'بابل', 'آمل', 'قائم‌شهر', 'بهشهر', 'چالوس', 'تنکابن', 'رامسر'],
    'گیلان': ['رشت', 'بندرانزلی', 'لاهیجان', 'لنگرود', 'تالش', 'آستارا', 'رودسر'],
    'البرز': ['کرج', 'فردیس', 'نظرآباد', 'هشتگرد', 'طالقان']
  };

  onProvinceChange(): void {
    const prov = this.newAddress.province;
    this.cities = prov ? this.citiesMap[prov] || [] : [];
    this.newAddress.city = '';
  }

  constructor(
    private userService: UserService,
    private cartService: CartService,
    private orderService: OrderService,
    private campaignService: CampaignService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAddresses();
    this.loadCartSummary();
    this.loadActiveCampaign();
  }

  loadActiveCampaign(): void {
    this.campaignService.getActiveCampaign().subscribe({
      next: (campaign) => {
        this.activeCampaign = campaign;
      },
      error: (err) => console.error('Error loading active campaign:', err)
    });
  }

  isAddressAllowed(address: Address): boolean {
    if (!this.activeCampaign) return true;
    
    const allowedProvinces = this.activeCampaign.allowedProvinces;
    const allowedCities = this.activeCampaign.allowedCities;

    if (allowedProvinces) {
      const provinces = allowedProvinces.split(',').map((p: string) => p.trim());
      if (!provinces.includes(address.province)) {
        return false;
      }
    }

    if (allowedCities) {
      const cities = allowedCities.split(',').map((c: string) => c.trim());
      if (!cities.includes(address.city)) {
        return false;
      }
    }

    return true;
  }

  loadAddresses(): void {
    this.userService.getAddresses().subscribe({
      next: (data: any) => {
        this.addresses = data;
        if (data.length > 0) {
          // Select first allowed address
          const allowed = data.find((a: Address) => this.isAddressAllowed(a));
          this.selectedAddressId = allowed ? allowed.id : data[0].id;
          this.loadShippingMethods();
        }
      },
      error: (err: any) => console.error('Error loading addresses:', err)
    });
  }

  selectAddress(addressId: number): void {
    this.selectedAddressId = addressId;
    this.loadShippingMethods();
  }

  loadCartSummary(): void {
    this.cartService.getCart().subscribe({
      next: (data: any) => {
        this.cartItems = data.items || [];
        this.subtotal = data.totalAmount || 0;
        this.campaignId = data.campaignId;
        this.calculateTotal();
        this.loadShippingMethods();
      },
      error: (err: any) => console.error('Error loading cart:', err)
    });
  }

  loadShippingMethods(): void {
    if (!this.selectedAddressId || !this.campaignId) return;

    this.orderService.getPublicShippingMethods(this.selectedAddressId, this.campaignId).subscribe({
      next: (methods: any) => {
        this.shippingMethods = methods || [];
        const allowedMethods = this.shippingMethods.filter(m => m.isAllowed);
        if (allowedMethods.length > 0) {
          this.selectedShippingMethodId = allowedMethods[0].id;
          this.shippingCost = allowedMethods[0].cost;
        } else {
          this.selectedShippingMethodId = null;
          this.shippingCost = 0;
        }
        this.calculateTotal();
      },
      error: (err: any) => console.error('Error loading shipping methods:', err)
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

  selectShippingMethod(method: any): void {
    if (method.isAllowed) {
      this.selectedShippingMethodId = method.id;
      this.onShippingMethodChange();
    }
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
      next: (newAddr: any) => {
        this.addresses.push(newAddr);
        this.selectedAddressId = newAddr.id;
        this.showNewAddressForm = false;
        this.newAddress = {};
        this.loadShippingMethods();
      },
      error: (err: any) => console.error('Error adding address:', err)
    });
  }

  createOrder(): void {
    if (!this.selectedAddressId) {
      alert('لطفاً یک آدرس انتخاب کنید.');
      return;
    }
    
    const selectedAddress = this.addresses.find(a => a.id === this.selectedAddressId);
    if (selectedAddress && !this.isAddressAllowed(selectedAddress)) {
      alert('آدرس انتخاب شده در محدوده پوشش‌دهی جغرافیایی این کمپین نیست.');
      return;
    }

    if (!this.selectedShippingMethodId) {
      alert('لطفاً روش ارسال را انتخاب کنید.');
      return;
    }

    const selectedMethod = this.shippingMethods.find(m => m.id === this.selectedShippingMethodId);
    
    const orderData = {
      cartId: this.campaignId, // cartId in request is actually map to OrderId or CartId on backend
      addressId: this.selectedAddressId,
      shippingMethod: selectedMethod ? selectedMethod.name : 'نامشخص',
      paymentMethod: 'Online'
    };

    this.loading = true;
    this.orderService.createOrder(orderData).subscribe({
      next: (order: any) => {
        this.loading = false;
        this.router.navigate(['/payment', order.id]);
      },
      error: (err: any) => {
        this.loading = false;
        console.error('Error creating order:', err);
        alert(err.error?.message || 'خطا در ثبت سفارش. لطفاً دوباره تلاش کنید.');
      }
    });
  }

  goToStep(step: number): void {
    if (step < this.step) {
      this.step = step;
    }
  }
}
