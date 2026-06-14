import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../services/admin.service';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="products-page">
      <div class="page-header">
        <h2>مدیریت محصولات</h2>
        <div class="actions">
          <button (click)="openImportModal()" class="btn-secondary">واردات از اکسل</button>
          <button (click)="scrapeProducts()" class="btn-primary">اسکرپ از تأمین‌کننده</button>
        </div>
      </div>
      <table class="data-table">
        <thead>
          <tr>
            <th>نام محصول</th>
            <th>دسته‌بندی</th>
            <th>قیمت پایه</th>
            <th>عملیات</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let product of products">
            <td>{{ product.name }}</td>
            <td>{{ product.category }}</td>
            <td>{{ product.price | number }} تومان</td>
            <td>
              <button class="btn-sm">ویرایش</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px; flex-wrap: wrap; gap: 15px; }
    .actions button { padding: 10px 20px; margin-left: 10px; border: none; border-radius: 6px; cursor: pointer; }
    .btn-primary { background: #007bff; color: white; }
    .btn-secondary { background: #6c757d; color: white; }
    .data-table { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.05); }
    .data-table th, .data-table td { padding: 15px; text-align: right; border-bottom: 1px solid #e9ecef; }
    .data-table th { background: #f8f9fa; font-weight: 600; color: #495057; }
    .data-table tr:hover { background: #f8f9fa; }
    .btn-sm { padding: 6px 12px; border: none; border-radius: 4px; cursor: pointer; background: #007bff; color: white; }
  `]
})
export class AdminProductsComponent implements OnInit {
  products: any[] = [];

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.adminService.getProducts().subscribe({
      next: (data) => this.products = data,
      error: (err) => console.error('Error loading products', err)
    });
  }

  openImportModal(): void {
    // Navigate to import component or open modal
    alert('Navigate to Excel Import');
  }

  scrapeProducts(): void {
    const supplierId = prompt('شناسه تأمین‌کننده را وارد کنید:');
    if (supplierId) {
      this.adminService.scrapeProducts(+supplierId).subscribe({
        next: () => {
          alert('محصولات با موفقیت اسکرپ شدند');
          this.loadProducts();
        },
        error: (err) => console.error('Error scraping products', err)
      });
    }
  }
}
