import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderService } from '../services/order.service';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-payment-verify',
  standalone: true,
  imports: [CommonModule, ButtonModule, CardModule],
  template: `
    <div class="verify-container" dir="rtl">
      <!-- Loading State -->
      <div class="state-card loading-card" *ngIf="loading">
        <i class="pi pi-spin pi-spinner" style="font-size: 3rem; color: #3b82f6;"></i>
        <h3>در حال تایید و ثبت نهایی تراکنش...</h3>
        <p>لطفاً شکیبا باشید و صفحه را نبندید.</p>
      </div>

      <!-- Success State -->
      <div class="state-card success-card" *ngIf="!loading && success">
        <i class="pi pi-check-circle text-success" style="font-size: 4rem;"></i>
        <h2>پرداخت با موفقیت انجام شد</h2>
        <p class="message">{{ message }}</p>

        <div class="receipt-details" *ngIf="authority">
          <div class="receipt-row">
            <span>شناسه پرداخت:</span>
            <strong>{{ authority }}</strong>
          </div>
        </div>

        <div class="actions">
          <button pButton type="button" label="مشاهده سفارش‌ها" icon="pi pi-list" 
                  class="p-button-success" (click)="goToOrders()"></button>
          
          <button pButton type="button" label="بازگشت به صفحه اصلی" icon="pi pi-home" 
                  class="p-button-secondary p-button-text" (click)="goToHome()"></button>
        </div>
      </div>

      <!-- Error State -->
      <div class="state-card error-card" *ngIf="!loading && !success">
        <i class="pi pi-times-circle text-error" style="font-size: 4rem;"></i>
        <h2>خطا در پردازش یا تایید پرداخت</h2>
        <p class="message">{{ message }}</p>

        <div class="actions">
          <button pButton type="button" label="بازگشت به سبد خرید" icon="pi pi-shopping-cart" 
                  class="p-button-danger" (click)="goToCart()"></button>
          
          <button pButton type="button" label="بازگشت به صفحه اصلی" icon="pi pi-home" 
                  class="p-button-secondary p-button-text" (click)="goToHome()"></button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .verify-container {
      max-width: 500px;
      margin: 4rem auto;
      padding: 0 1rem;
    }
    .state-card {
      background: white;
      border: 1px solid #e5e7eb;
      border-radius: 12px;
      padding: 2.5rem 1.5rem;
      text-align: center;
      box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.05);
      display: flex;
      flex-direction: column;
      align-items: center;
    }
    .state-card h2 {
      margin-top: 1.5rem;
      margin-bottom: 0.5rem;
      font-size: 1.4rem;
    }
    .state-card h3 {
      margin-top: 1.5rem;
      font-size: 1.15rem;
      color: #374151;
    }
    .state-card p {
      color: #6b7280;
      margin: 0;
    }
    .state-card p.message {
      margin: 1rem 0;
      color: #4b5563;
      font-size: 0.95rem;
    }
    .text-success {
      color: #10b981;
    }
    .text-error {
      color: #ef4444;
    }
    .receipt-details {
      background-color: #f9fafb;
      border: 1px solid #f3f4f6;
      border-radius: 6px;
      padding: 1rem;
      width: 100%;
      margin: 1rem 0;
    }
    .receipt-row {
      display: flex;
      justify-content: space-between;
      font-size: 0.9rem;
    }
    .receipt-row strong {
      font-family: monospace;
    }
    .actions {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
      width: 100%;
      margin-top: 1.5rem;
    }
    .actions button {
      width: 100%;
      padding: 0.75rem;
      font-weight: bold;
    }
  `]
})
export class PaymentVerifyComponent implements OnInit {
  loading: boolean = true;
  success: boolean = false;
  message: string = '';
  authority: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.authority = params['Authority'] || params['authority'] || '';
      const status = params['Status'] || params['status'] || '';

      if (!this.authority) {
        this.loading = false;
        this.success = false;
        this.message = 'اطلاعات تراکنش یافت نشد.';
        return;
      }

      this.verifyPayment(this.authority, status);
    });
  }

  verifyPayment(authority: string, status: string): void {
    this.orderService.verifyPayment(authority, status).subscribe({
      next: (res: any) => {
        this.loading = false;
        this.success = res.success;
        this.message = res.message || (res.success ? 'پرداخت شما با موفقیت تأیید شد.' : 'تراکنش ناموفق بود.');
      },
      error: (err: any) => {
        this.loading = false;
        this.success = false;
        this.message = err.error?.message || 'خطا در برقراری ارتباط با سرور جهت تأیید تراکنش.';
        console.error('Verify payment error:', err);
      }
    });
  }

  goToOrders(): void {
    this.router.navigate(['/panel/orders']);
  }

  goToCart(): void {
    this.router.navigate(['/cart']);
  }

  goToHome(): void {
    this.router.navigate(['/']);
  }
}
