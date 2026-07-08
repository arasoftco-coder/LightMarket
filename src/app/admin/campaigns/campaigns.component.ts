import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, Campaign } from '../../services/admin.service';
import { RouterModule } from '@angular/router';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-admin-campaigns',
  standalone: true,
  imports: [CommonModule, RouterModule, DialogModule, ButtonModule],
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
            <td>{{ formatDate(campaign.startDate) }}</td>
            <td>{{ formatDate(campaign.endDate) }}</td>
            <td>
              <span class="status-badge" [class.active]="campaign.isActive">
                {{ campaign.isActive ? 'فعال' : 'غیرفعال' }}
              </span>
            </td>
            <td>
              <button routerLink="/admin/campaigns/edit/{{campaign.id}}" class="btn-sm">ویرایش</button>
              <button (click)="viewReport(campaign.id)" class="btn-sm btn-secondary">گزارش</button>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Report Dialog -->
      <p-dialog header="گزارش عملکرد کمپین" [(visible)]="showReportDialog" [modal]="true" [style]="{ width: '500px' }">
        <div class="report-container" *ngIf="campaignReport" dir="rtl">
          <h3>{{ campaignReport.campaignName }}</h3>
          <hr />
          <div class="report-item">
            <strong>تعداد سفارشات نهایی:</strong>
            <span>{{ campaignReport.totalOrders | number }}</span>
          </div>
          <div class="report-item">
            <strong>تعداد خریداران یکتا:</strong>
            <span>{{ campaignReport.totalUsers | number }}</span>
          </div>
          <div class="report-item">
            <strong>مجموع درآمد (تومان):</strong>
            <span>{{ campaignReport.totalRevenue | number }} تومان</span>
          </div>
          <div class="report-item">
            <strong>مجموع تخفیف‌های داده‌شده:</strong>
            <span>{{ campaignReport.totalDiscount | number }} تومان</span>
          </div>
          <div class="report-item">
            <strong>تاریخ شروع:</strong>
            <span>{{ formatDate(campaignReport.startDate) }}</span>
          </div>
          <div class="report-item">
            <strong>تاریخ پایان:</strong>
            <span>{{ formatDate(campaignReport.endDate) }}</span>
          </div>
        </div>
        <ng-template pTemplate="footer">
          <button pButton type="button" label="بستن" class="p-button-text" (click)="showReportDialog = false"></button>
        </ng-template>
      </p-dialog>
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
    .report-container { padding: 15px; }
    .report-item { display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px dashed #e9ecef; }
    .report-item strong { color: #495057; }
  `]
})
export class AdminCampaignsComponent implements OnInit {
  campaigns: Campaign[] = [];
  showReportDialog = false;
  campaignReport: any = null;

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

  viewReport(campaignId: number): void {
    this.adminService.getCampaignReport(campaignId).subscribe({
      next: (data) => {
        this.campaignReport = data;
        this.showReportDialog = true;
      },
      error: (err) => console.error('Error loading report', err)
    });
  }

  formatDate(dateString: string): string {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('fa-IR');
  }
}
