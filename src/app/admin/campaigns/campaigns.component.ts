import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, Campaign } from '../../services/admin.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-campaigns',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="campaigns-page">
      <div class="page-header">
        <h2>مدیریت کمپین‌ها</h2>
        <button routerLink="/admin/campaigns/new" class="btn-primary">کمپین جدید</button>
      </div>
      <table class="data-table">
        <thead>
          <tr>
            <th>نام کمپین</th>
            <th>تأمین‌کننده</th>
            <th>تاریخ شروع</th>
            <th>تاریخ پایان</th>
            <th>وضعیت</th>
            <th>عملیات</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let campaign of campaigns">
            <td>{{ campaign.name }}</td>
            <td>تأمین‌کننده {{ campaign.supplierId }}</td>
            <td>{{ campaign.startDate | date:'1403/02/01' }}</td>
            <td>{{ campaign.endDate | date:'1403/03/01' }}</td>
            <td>
              <span class="status-badge" [class.active]="campaign.status === 'Active'">
                {{ campaign.status === 'Active' ? 'فعال' : 'غیرفعال' }}
              </span>
            </td>
            <td>
              <button routerLink="/admin/campaigns/edit/{{campaign.id}}" class="btn-sm">ویرایش</button>
              <button class="btn-sm btn-secondary">گزارش</button>
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
    .status-badge { padding: 5px 12px; border-radius: 20px; font-size: 0.85rem; background: #e9ecef; color: #495057; }
    .status-badge.active { background: #d4edda; color: #155724; }
    .btn-sm { padding: 6px 12px; margin-left: 5px; border: none; border-radius: 4px; cursor: pointer; background: #007bff; color: white; }
    .btn-secondary { background: #6c757d; }
  `]
})
export class AdminCampaignsComponent implements OnInit {
  campaigns: Campaign[] = [];

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadCampaigns();
  }

  loadCampaigns(): void {
    this.adminService.getCampaigns().subscribe({
      next: (data) => this.campaigns = data,
      error: (err) => console.error('Error loading campaigns', err)
    });
  }
}
