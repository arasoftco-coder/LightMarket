import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule],
  template: `<h2>سفارش‌ها</h2><p>این صفحه در فاز بعدی پیاده‌سازی خواهد شد.</p>`,
  styles: [`h2 { color: var(--text-color); margin-bottom: 16px; } p { color: var(--text-light); }`]
})
export class OrdersComponent {}
