import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../services/admin.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-admin-campaign-edit',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="campaign-edit-page">
      <h2>{{ isEdit ? 'ویرایش کمپین' : 'ایجاد کمپین جدید' }}</h2>
      <form (ngSubmit)="saveCampaign()" class="campaign-form">
        <div class="form-group">
          <label>نام کمپین:</label>
          <input type="text" [(ngModel)]="campaign.name" name="name" required />
        </div>
        <div class="form-group">
          <label>تأمین‌کننده:</label>
          <select [(ngModel)]="campaign.supplierId" name="supplierId">
            <option *ngFor="let supplier of suppliers" [value]="supplier.id">{{ supplier.name }}</option>
          </select>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label>تاریخ شروع:</label>
            <input type="date" [(ngModel)]="campaign.startDate" name="startDate" />
          </div>
          <div class="form-group">
            <label>تاریخ پایان:</label>
            <input type="date" [(ngModel)]="campaign.endDate" name="endDate" />
          </div>
        </div>
        <div class="form-actions">
          <button type="submit" class="btn-primary">{{ isEdit ? 'ذخیره تغییرات' : 'ایجاد کمپین' }}</button>
          <button type="button" routerLink="/admin/campaigns" class="btn-secondary">انصراف</button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .campaign-form { max-width: 600px; background: #f8f9fa; padding: 30px; border-radius: 10px; }
    .form-group { margin-bottom: 20px; }
    .form-group label { display: block; margin-bottom: 8px; font-weight: 600; }
    .form-group input, .form-group select { width: 100%; padding: 10px; border: 1px solid #ced4da; border-radius: 6px; font-family: inherit; }
    .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
    .form-actions { display: flex; gap: 10px; margin-top: 30px; }
    .btn-primary, .btn-secondary { padding: 12px 24px; border: none; border-radius: 6px; cursor: pointer; font-family: inherit; }
    .btn-primary { background: #007bff; color: white; }
    .btn-secondary { background: #6c757d; color: white; }
  `]
})
export class AdminCampaignEditComponent implements OnInit {
  campaign: any = {
    name: '',
    supplierId: null,
    startDate: '',
    endDate: ''
  };
  suppliers: any[] = [];
  isEdit = false;
  campaignId: number | null = null;

  constructor(
    private adminService: AdminService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSuppliers();
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.campaignId = +id;
      this.loadCampaign(this.campaignId);
    }
  }

  loadSuppliers(): void {
    this.adminService.getSuppliers().subscribe({
      next: (data) => this.suppliers = data,
      error: (err) => console.error('Error loading suppliers', err)
    });
  }

  loadCampaign(id: number): void {
    // In real app, fetch campaign details
    this.campaign = { name: 'کمپین نمونه', supplierId: 1, startDate: '2024-01-01', endDate: '2024-12-31' };
  }

  saveCampaign(): void {
    if (this.isEdit && this.campaignId) {
      this.adminService.updateCampaign(this.campaignId, this.campaign).subscribe({
        next: () => this.router.navigate(['/admin/campaigns']),
        error: (err) => console.error('Error updating campaign', err)
      });
    } else {
      this.adminService.createCampaign(this.campaign).subscribe({
        next: () => this.router.navigate(['/admin/campaigns']),
        error: (err) => console.error('Error creating campaign', err)
      });
    }
  }
}
