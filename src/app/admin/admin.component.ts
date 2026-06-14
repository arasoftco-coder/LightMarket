import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="admin-layout">
      <aside class="sidebar">
        <div class="logo">پنل مدیریت</div>
        <nav>
          <a routerLink="/admin/dashboard" routerLinkActive="active">داشبورد</a>
          <a routerLink="/admin/campaigns" routerLinkActive="active">کمپین‌ها</a>
          <a routerLink="/admin/suppliers" routerLinkActive="active">تأمین‌کنندگان</a>
          <a routerLink="/admin/orders" routerLinkActive="active">سفارشات</a>
          <a routerLink="/admin/products" routerLinkActive="active">محصولات</a>
          <a routerLink="/admin/settings" routerLinkActive="active">تنظیمات</a>
        </nav>
      </aside>
      <main class="content">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [`
    .admin-layout { display: flex; min-height: 100vh; direction: rtl; }
    .sidebar { width: 250px; background: #f8f9fa; padding: 20px; border-left: 1px solid #e9ecef; }
    .logo { font-size: 1.5rem; font-weight: bold; margin-bottom: 30px; color: #333; }
    nav a { display: block; padding: 12px 15px; margin-bottom: 5px; text-decoration: none; color: #495057; border-radius: 6px; transition: all 0.2s; }
    nav a:hover { background: #e9ecef; }
    nav a.active { background: #007bff; color: white; }
    .content { flex: 1; padding: 30px; background: #fff; }
  `]
})
export class AdminComponent {}
