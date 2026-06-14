import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../services/admin.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard">
      <h2>داشبورد مدیریت</h2>
      <div class="stats-grid">
        <div class="stat-card" *ngFor="let stat of stats">
          <div class="stat-icon">{{ stat.icon }}</div>
          <div class="stat-info">
            <h3>{{ stat.value }}</h3>
            <p>{{ stat.label }}</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard h2 { margin-bottom: 30px; }
    .stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 20px; }
    .stat-card { display: flex; align-items: center; padding: 25px; background: #f8f9fa; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.05); }
    .stat-icon { font-size: 2.5rem; margin-left: 20px; }
    .stat-info h3 { margin: 0; font-size: 2rem; color: #333; }
    .stat-info p { margin: 5px 0 0; color: #6c757d; }
  `]
})
export class AdminDashboardComponent implements OnInit {
  stats: any[] = [];

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    // Dummy data for now - will be replaced with API call
    this.stats = [
      { icon: '📦', value: '۱۲۵', label: 'سفارشات جدید' },
      { icon: '💰', value: '۴۵,۰۰۰,۰۰۰', label: 'درآمد کل (تومان)' },
      { icon: '🎯', value: '۸', label: 'کمپین‌های فعال' },
      { icon: '⏳', value: '۱۲', label: 'در انتظار پرداخت' }
    ];
  }
}
