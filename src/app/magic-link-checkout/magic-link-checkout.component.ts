import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';

interface Invoice {
  id: number;
  orderNumber: string;
  items: any[];
  subtotal: number;
  shippingCost: number;
  totalAmount: number;
  status: string;
}

@Component({
  selector: 'app-magic-link-checkout',
  standalone: true,
  imports: [CommonModule, ButtonModule, CardModule],
  templateUrl: './magic-link-checkout.component.html',
  styleUrls: ['./magic-link-checkout.component.css']
})
export class MagicLinkCheckoutComponent implements OnInit {
  token: string | null = null;
  invoice: Invoice | null = null;
  loading: boolean = true;
  error: string | null = null;
  processing: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.paramMap.get('token');
    
    if (!this.token) {
      this.error = 'لینک پرداخت نامعتبر است.';
      this.loading = false;
      return;
    }

    this.validateAndLoadInvoice();
  }

  validateAndLoadInvoice(): void {
    this.orderService.validateMagicLink(this.token!).subscribe({
      next: (data) => {
        this.invoice = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error validating magic link:', err);
        if (err.status === 401 || err.status === 403) {
          this.error = 'لینک پرداخت منقضی شده یا نامعتبر است.';
        } else {
          this.error = 'خطا در بارگذاری اطلاعات سفارش.';
        }
        this.loading = false;
      }
    });
  }

  proceedToPayment(): void {
    if (!this.invoice) return;
    
    this.processing = true;
    
    // TODO: Call payment API
    // For now, simulate redirect to payment gateway
    setTimeout(() => {
      this.processing = false;
      alert('در حال انتقال به درگاه پرداخت...');
      // this.router.navigate(['/payment/gateway', this.invoice.id]);
    }, 1000);
  }
}
