import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, Supplier } from '../../services/admin.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-suppliers',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="suppliers-page">
      <div class="page-header">
        <h2>مدیریت تأمین‌کنندگان</h2>
        <button routerLink="/admin/suppliers/new" class="btn-primary">تأمین‌کننده جدید</button>
      </div>
      <table class="data-table">
        <thead>
          <tr>
            <th>نام تأمین‌کننده</th>
            <th>وب‌سایت</th>
            <th>اطلاعات تماس</th>
            <th>نیاز به کد رهگیری</th>
            <th>عملیات</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let supplier of suppliers">
            <td>{{ supplier.name }}</td>
            <td>{{ supplier.website }}</td>
            <td>{{ supplier.contactInfo }}</td>
            <td>{{ supplier.requiresTrackingCode ? 'بله' : 'خیر' }}</td>
            <td>
              <button routerLink="/admin/suppliers/edit/{{supplier.id}}" class="btn-sm">ویرایش</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px; }
    .data-table { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.05); }
    .data-table th, .data-table td { padding: 15px; text-align: right; border-bottom: 1px solid #e9ecef; }
    .data-table th { background: #f8f9fa; font-weight: 600; color: #495057; }
    .data-table tr:hover { background: #f8f9fa; }
    .btn-sm { padding: 6px 12px; border: none; border-radius: 4px; cursor: pointer; background: #007bff; color: white; }
  `]
})
export class AdminSuppliersComponent implements OnInit {
  suppliers: Supplier[] = [];

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadSuppliers();
  }

  loadSuppliers(): void {
    this.adminService.getSuppliers().subscribe({
      next: (data) => this.suppliers = data,
      error: (err) => console.error('Error loading suppliers', err)
    });
  }
}
