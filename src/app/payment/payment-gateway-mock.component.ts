import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-payment-gateway-mock',
  standalone: true,
  imports: [CommonModule, ButtonModule, CardModule],
  template: `
    <div class="gateway-container" dir="rtl">
      <div class="header-banner">
        <h2>درگاه پرداخت اینترنتی به پرداخت ملت (شبیه‌ساز)</h2>
        <p>محیط تست و شبیه‌سازی تراکنش‌های لایت مارکت</p>
      </div>

      <p-card styleClass="payment-card">
        <div class="merchant-info">
          <div class="info-item">
            <span class="label">پذیرنده:</span>
            <span class="val">لایت مارکت (LightMarket)</span>
          </div>
          <div class="info-item" *ngIf="amount">
            <span class="label">مبلغ قابل پرداخت:</span>
            <span class="val price">{{ amount | number:'1.0-0' }} تومان</span>
          </div>
          <div class="info-item" *ngIf="authority">
            <span class="label">شناسه خرید (Authority):</span>
            <span class="val code">{{ authority }}</span>
          </div>
        </div>

        <div class="warning-alert">
          <i class="pi pi-exclamation-triangle"></i>
          این یک صفحه پرداخت واقعی نیست. پول واقعی جابجا نخواهد شد. لطفاً یکی از گزینه‌های زیر را انتخاب کنید:
        </div>

        <div class="btn-group">
          <button pButton type="button" label="پرداخت موفقیت‌آمیز" icon="pi pi-check-circle" 
                  class="p-button-success" (click)="completePayment(true)"></button>
          
          <button pButton type="button" label="انصراف از پرداخت / ناموفق" icon="pi pi-times-circle" 
                  class="p-button-danger p-button-outlined" (click)="completePayment(false)"></button>
        </div>
      </p-card>
    </div>
  `,
  styles: [`
    .gateway-container {
      max-width: 550px;
      margin: 3rem auto;
      padding: 0 1rem;
    }
    .header-banner {
      text-align: center;
      background: linear-gradient(135deg, #1e3a8a 0%, #3b82f6 100%);
      color: white;
      padding: 1.5rem;
      border-radius: 12px 12px 0 0;
      box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    }
    .header-banner h2 {
      margin: 0 0 0.5rem 0;
      font-size: 1.3rem;
    }
    .header-banner p {
      margin: 0;
      font-size: 0.9rem;
      opacity: 0.9;
    }
    ::ng-deep .payment-card {
      border-radius: 0 0 12px 12px !important;
      border: 1px solid #e5e7eb;
      border-top: none;
      box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.05) !important;
    }
    .merchant-info {
      background-color: #f9fafb;
      border-radius: 8px;
      padding: 1rem;
      margin-bottom: 1.5rem;
      border: 1px solid #f3f4f6;
    }
    .info-item {
      display: flex;
      justify-content: space-between;
      margin-bottom: 0.6rem;
      font-size: 0.95rem;
    }
    .info-item:last-child {
      margin-bottom: 0;
    }
    .info-item .label {
      color: #6b7280;
    }
    .info-item .val {
      font-weight: bold;
      color: #1f2937;
    }
    .info-item .val.price {
      color: #b91c1c;
      font-size: 1.1rem;
    }
    .info-item .val.code {
      font-family: monospace;
      font-size: 0.9rem;
    }
    .warning-alert {
      background-color: #fffbeb;
      border-right: 4px solid #d97706;
      padding: 1rem;
      color: #92400e;
      border-radius: 4px;
      margin-bottom: 1.5rem;
      font-size: 0.9rem;
      line-height: 1.5;
    }
    .warning-alert i {
      margin-left: 0.5rem;
    }
    .btn-group {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }
    .btn-group button {
      width: 100%;
      padding: 0.75rem;
      font-weight: bold;
    }
  `]
})
export class PaymentGatewayMockComponent implements OnInit {
  authority: string = '';
  amount: string = '';

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.authority = params['authority'] || '';
      this.amount = params['amount'] || '';
    });
  }

  completePayment(success: boolean): void {
    const status = success ? 'OK' : 'NOK';
    this.router.navigate(['/payment/verify'], {
      queryParams: {
        Authority: this.authority,
        Status: status
      }
    });
  }
}
