import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../services/admin.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-admin-order-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="order-detail-page">
      <h2>جزئیات سفارش #{{ orderId }}</h2>
      
      <div class="order-section">
        <h3>اطلاعات سفارش</h3>
        <div class="info-grid">
          <div><strong>وضعیت:</strong> {{ order?.status }}</div>
          <div><strong>مبلغ کل:</strong> {{ order?.totalAmount | number }} تومان</div>
          <div><strong>تاریخ ثبت:</strong> {{ order?.createdAt | date:'full' }}</div>
        </div>
      </div>

      <div class="order-section">
        <h3>اقلام سفارش</h3>
        <table class="items-table">
          <thead>
            <tr>
              <th>محصول</th>
              <th>تعداد</th>
              <th>قیمت واحد</th>
              <th>جمع</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let item of orderItems">
              <td>{{ item.name }}</td>
              <td>{{ item.quantity }}</td>
              <td>{{ item.price | number }} تومان</td>
              <td>{{ item.quantity * item.price | number }} تومان</td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="order-section" *ngIf="order?.status === 'PaymentPending'">
        <h3>تأیید پرداخت</h3>
        <div class="action-form">
          <input type="text" [(ngModel)]="trackingCode" placeholder="کد رهگیری پرداخت" />
          <button (click)="confirmPayment()" class="btn-primary">تأیید پرداخت</button>
        </div>
      </div>

      <div class="order-section" *ngIf="order?.status === 'PaymentConfirmed'">
        <h3>اطلاعات ارسال</h3>
        <div class="action-form">
          <input type="text" [(ngModel)]="shippingCompany" placeholder="شرکت پستی" />
          <input type="text" [(ngModel)]="trackingNumber" placeholder="کد رهگیری مرسوله" />
          <button (click)="updateShipping()" class="btn-primary">ثبت اطلاعات ارسال</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .order-detail-page { max-width: 900px; }
    .order-section { background: #f8f9fa; padding: 25px; border-radius: 10px; margin-bottom: 25px; }
    .order-section h3 { margin-top: 0; margin-bottom: 20px; color: #495057; }
    .info-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px; }
    .items-table { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; }
    .items-table th, .items-table td { padding: 12px; text-align: right; border-bottom: 1px solid #e9ecef; }
    .items-table th { background: #e9ecef; font-weight: 600; }
    .action-form { display: flex; gap: 10px; flex-wrap: wrap; }
    .action-form input { flex: 1; min-width: 200px; padding: 10px; border: 1px solid #ced4da; border-radius: 6px; }
    .btn-primary { padding: 12px 24px; background: #007bff; color: white; border: none; border-radius: 6px; cursor: pointer; }
  `]
})
export class AdminOrderDetailComponent implements OnInit {
  orderId: number | null = null;
  order: any = null;
  orderItems: any[] = [];
  trackingCode: string = '';
  shippingCompany: string = '';
  trackingNumber: string = '';

  constructor(
    private adminService: AdminService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.orderId = +id;
      this.loadOrder();
    }
  }

  loadOrder(): void {
    if (this.orderId) {
      this.adminService.getOrderDetail(this.orderId).subscribe({
        next: (data) => {
          this.order = data;
          this.orderItems = data.items || [];
        },
        error: (err) => console.error('Error loading order', err)
      });
    }
  }

  confirmPayment(): void {
    if (this.orderId && this.trackingCode) {
      this.adminService.confirmPayment(this.orderId, this.trackingCode).subscribe({
        next: () => {
          alert('پرداخت تأیید شد');
          this.loadOrder();
        },
        error: (err) => console.error('Error confirming payment', err)
      });
    }
  }

  updateShipping(): void {
    if (this.orderId && this.trackingNumber) {
      this.adminService.updateOrderStatus(this.orderId, 'Shipped').subscribe({
        next: () => {
          alert('اطلاعات ارسال ثبت شد');
          this.loadOrder();
        },
        error: (err) => console.error('Error updating shipping', err)
      });
    }
  }
}
