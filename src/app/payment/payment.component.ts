import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderService } from '../services/order.service';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { RadioButtonModule } from 'primeng/radiobutton';

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, CardModule, InputTextModule, RadioButtonModule],
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']
})
export class PaymentComponent implements OnInit {
  orderId!: number;
  order: any = null;
  loading: boolean = true;
  submitting: boolean = false;
  paymentMethod: string = 'online'; // 'online' or 'cardToCard'
  
  // Card to Card fields
  cardNumber: string = '6037-9911-2233-4455';
  cardHolderName: string = 'فروشگاه لایت مارکت (تأمین‌کننده)';
  trackingCode: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.orderId = +idParam;
      this.loadOrderDetails();
    } else {
      this.router.navigate(['/']);
    }
  }

  loadOrderDetails(): void {
    this.orderService.getOrderDetails(this.orderId).subscribe({
      next: (data) => {
        this.order = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading order:', err);
        alert('خطا در بارگذاری اطلاعات سفارش.');
        this.loading = false;
        this.router.navigate(['/']);
      }
    });
  }

  processPayment(): void {
    if (this.paymentMethod === 'online') {
      this.submitting = true;
      this.orderService.createPaymentRequest(this.orderId, this.order.totalAmount).subscribe({
        next: (res) => {
          this.submitting = false;
          if (res.success && res.paymentUrl) {
            // Parse Mock Payment URL params: authority and amount
            const url = new URL(res.paymentUrl);
            const authority = url.searchParams.get('authority') || '';
            const amount = url.searchParams.get('amount') || '';
            
            // Redirect internally to the mock bank page
            this.router.navigate(['/payment/gateway'], { queryParams: { authority, amount } });
          } else {
            alert('خطا در ایجاد درخواست پرداخت.');
          }
        },
        error: (err) => {
          this.submitting = false;
          console.error('Error creating payment request:', err);
          alert('خطا در ارتباط با درگاه پرداخت.');
        }
      });
    } else {
      if (!this.trackingCode.trim()) {
        alert('لطفاً شماره پیگیری فیش واریزی را وارد کنید.');
        return;
      }
      this.submitting = true;
      this.orderService.confirmPayment(this.orderId, this.trackingCode).subscribe({
        next: () => {
          this.submitting = false;
          alert('رسید پرداخت شما ثبت شد و در انتظار تایید مدیریت است.');
          this.router.navigate(['/panel/orders']);
        },
        error: (err) => {
          this.submitting = false;
          console.error('Error confirming card-to-card payment:', err);
          alert('خطا در ثبت شماره پیگیری.');
        }
      });
    }
  }
}
