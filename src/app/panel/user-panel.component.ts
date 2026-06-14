import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-user-panel',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  template: `
    <div class="panel-container">
      <aside class="panel-sidebar">
        <h3>پنل کاربری</h3>
        <nav>
          <a routerLink="profile" routerLinkActive="active">پروفایل</a>
          <a routerLink="addresses" routerLinkActive="active">آدرس‌ها</a>
          <a routerLink="orders" routerLinkActive="active">سفارش‌ها</a>
          <a routerLink="tickets" routerLinkActive="active">تیکت‌ها</a>
        </nav>
      </aside>
      <main class="panel-content">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styleUrls: ['./user-panel.component.css']
})
export class UserPanelComponent {}
