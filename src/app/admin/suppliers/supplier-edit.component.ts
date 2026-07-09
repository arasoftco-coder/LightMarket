import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../services/admin.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-supplier-edit',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="supplier-edit-page">
      <h2>{{ isEdit ? 'ویرایش تأمین‌کننده' : 'ایجاد تأمین‌کننده جدید' }}</h2>
      <form (ngSubmit)="saveSupplier()" class="supplier-form">
        <div class="form-group">
          <label>نام تأمین‌کننده:</label>
          <input type="text" [(ngModel)]="supplier.name" name="name" required />
        </div>
        <div class="form-group">
          <label>وب‌سایت:</label>
          <input type="url" [(ngModel)]="supplier.website" name="website" />
        </div>
        <div class="form-group">
          <label>اطلاعات تماس:</label>
          <textarea [(ngModel)]="supplier.contactInfo" name="contactInfo" rows="3"></textarea>
        </div>
        <div class="form-actions">
          <button type="submit" class="btn-primary">{{ isEdit ? 'ذخیره تغییرات' : 'ایجاد تأمین‌کننده' }}</button>
          <button type="button" routerLink="/admin/suppliers" class="btn-secondary">انصراف</button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .supplier-form { max-width: 600px; background: #f8f9fa; padding: 30px; border-radius: 10px; }
    .form-group { margin-bottom: 20px; }
    .form-group label { display: block; margin-bottom: 8px; font-weight: 600; }
    .form-group input, .form-group textarea { width: 100%; padding: 10px; border: 1px solid #ced4da; border-radius: 6px; font-family: inherit; }
    .checkbox-group label { display: flex; align-items: center; gap: 10px; cursor: pointer; }
    .checkbox-group input { width: auto; }
    .form-actions { display: flex; gap: 10px; margin-top: 30px; }
    .btn-primary, .btn-secondary { padding: 12px 24px; border: none; border-radius: 6px; cursor: pointer; font-family: inherit; }
    .btn-primary { background: #007bff; color: white; }
    .btn-secondary { background: #6c757d; color: white; }
  `]
})
export class AdminSupplierEditComponent implements OnInit {
  supplier: any = {
    name: '',
    website: '',
    contactInfo: '',
    requiresTrackingCode: false
  };
  isEdit = false;
  supplierId: number | null = null;

  constructor(
    private adminService: AdminService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.supplierId = +id;
      this.loadSupplier(this.supplierId);
    }
  }

  loadSupplier(id: number): void {
    this.adminService.getSupplierById(id).subscribe({
      next: (data) => this.supplier = data,
      error: (err) => console.error('Error loading supplier', err)
    });
  }

  saveSupplier(): void {
    if (this.isEdit && this.supplierId) {
      this.adminService.updateSupplier(this.supplierId, this.supplier).subscribe({
        next: () => this.router.navigate(['/admin/suppliers']),
        error: (err) => console.error('Error updating supplier', err)
      });
    } else {
      this.adminService.createSupplier(this.supplier).subscribe({
        next: () => this.router.navigate(['/admin/suppliers']),
        error: (err) => console.error('Error creating supplier', err)
      });
    }
  }
}
