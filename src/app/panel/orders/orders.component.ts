import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../services/order.service';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';

interface Order {
  id: number;
  orderNumber: string;
  date: string;
  totalAmount: number;
  status: string;
  statusFa: string;
  items?: any[];
}

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, TableModule, TagModule, ButtonModule],
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css']
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];
  selectedOrder: Order | null = null;
  showDetails: boolean = false;

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.orderService.getUserOrders().subscribe({
      next: (data: any) => {
        this.orders = data.map((order: any) => ({
          ...order,
          statusFa: this.getStatusFa(order.status)
        }));
      },
      error: (err: any) => console.error('Error loading orders:', err)
    });
  }

  getStatusFa(status: string): string {
    const map: { [key: string]: string } = {
      'PaymentPending': 'در انتظار پرداخت',
      'PaymentConfirmed': 'پرداخت شده',
      'Processing': 'در حال پردازش',
      'Shipped': 'ارسال شده',
      'Delivered': 'تحویل داده شده',
      'Cancelled': 'لغو شده'
    };
    return map[status] || status;
  }

  getSeverity(status: string): 'success' | 'info' | 'warning' | 'danger' | 'secondary' | 'contrast' | undefined {
    const map: { [key: string]: 'success' | 'info' | 'warning' | 'danger' | 'secondary' } = {
      'PaymentPending': 'warning',
      'PaymentConfirmed': 'success',
      'Processing': 'info',
      'Shipped': 'info',
      'Delivered': 'success',
      'Cancelled': 'danger'
    };
    return map[status] || 'secondary';
  }

  viewDetails(order: Order): void {
    this.selectedOrder = order;
    this.showDetails = true;
  }

  closeDetails(): void {
    this.showDetails = false;
    this.selectedOrder = null;
  }
}
