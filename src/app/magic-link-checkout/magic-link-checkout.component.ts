import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-magic-link-checkout',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="magic-checkout-container">
      <h2>پرداخت امن</h2>
      <p>در حال بارگذاری اطلاعات سفارش...</p>
    </div>
  `,
  styles: [`
    .magic-checkout-container {
      padding: 50px;
      text-align: center;
    }
    h2 { color: var(--text-color); margin-bottom: 16px; }
    p { color: var(--text-light); }
  `]
})
export class MagicLinkCheckoutComponent {
  constructor(private route: ActivatedRoute) {
    const token = this.route.snapshot.paramMap.get('token');
    console.log('Magic link token:', token);
  }
}
