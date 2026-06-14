import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, Order } from '../../services/admin.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="orders-page">
      <div class="page-header">
        <h2>مدیریت سفارشات</h2>
        <div class="filters">
          <button (click)="filterByStatus('')" [class.active]="!currentFilter">همه</button>
          <button (click)="filterByStatus('PaymentPending')" [class.active]="currentFilter === 'PaymentPending'">در انتظار پرداخت</button>
          <button (click)="filterByStatus('PaymentConfirmed')" [class.active]="currentFilter === 'PaymentConfirmed'">پرداخت شده</button>
          <button (click)="filterByStatus('Shipped')" [class.active]="currentFilter === 'Shipped'">ارسال شده</button>
        </div>
      </div>
      <table class="data-table">
        <thead>
          <tr>
            <th>شماره سفارش</th>
            <th>کاربر</th>
            <th>کمپین</th>
            <th>مبلغ کل</th>
            <th>وضعیت</th>
            <th>تاریخ</th>
            <th>عملیات</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let order of orders">
            <td>#{{ order.id }}</td>
            <td>کاربر {{ order.userId }}</td>
            <td>کمپین {{ order.campaignId }}</td>
            <td>{{ order.totalAmount | number }} تومان</td>
            <td>
              <span class="status-badge" [class]="'status-' + order.status">
                {{ getStatusLabel(order.status) }}
              </span>
            </td>
            <td>{{ order.createdAt | date:'1403/02/01' }}</td>
            <td>
              <button routerLink="/admin/orders/{{order.id}}" class="btn-sm">مشاهده</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px; flex-wrap: wrap; gap: 15px; }
    .filters button { padding: 8px 16px; margin-left: 5px; border: 1px solid #ced4da; background: white; border-radius: 6px; cursor: pointer; }
    .filters button.active { background: #007bff; color: white; border-color: #007bff; }
    .data-table { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.05); }
    .data-table th, .data-table td { padding: 15px; text-align: right; border-bottom: 1px solid #e9ecef; }
    .data-table th { background: #f8f9fa; font-weight: 600; color: #495057; }
    .data-table tr:hover { background: #f8f9fa; }
    .status-badge { padding: 5px 12px; border-radius: 20px; font-size: 0.85rem; }
    .status-PaymentPending { background: #fff3cd; color: #856404; }
    .status-PaymentConfirmed { background: #d4edda; color: #155724; }
    .status-Shipped { background: #cce5ff; color: #004085; }
    .btn-sm { padding: 6px 12px; border: none; border-radius: 4px; cursor: pointer; background: #007bff; color: white; }
  `]
})
export class AdminOrdersComponent implements OnInit {
  orders: Order[] = [];
  currentFilter: string = '';

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.adminService.getOrders(this.currentFilter).subscribe({
      next: (data) => this.orders = data,
      error: (err) => console.error('Error loading orders', err)
    });
  }

  filterByStatus(status: string): void {
    this.currentFilter = status;
    this.loadOrders();
  }

  getStatusLabel(status: string): string {
    const labels: any = {
      'PaymentPending': 'در انتظار پرداخت',
      'PaymentConfirmed': 'پرداخت شده',
      'Shipped': 'ارسال شده'
    };
    return labels[status] || status;
  }
}
