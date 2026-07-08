import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../services/admin.service';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {
  currentYear = new Date().getFullYear();
  footerLinks: any[] = [];
  contactInfo = '';

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadFooterData();
  }

  loadFooterData(): void {
    this.adminService.getPublicSettings().subscribe({
      next: (data) => {
        if (data) {
          this.footerLinks = data.footerLinks || [];
          this.contactInfo = data.contactInfo || '';
        }
      },
      error: (err) => {
        console.error('Error loading footer public settings', err);
        // Fallback default links
        this.footerLinks = [
          { title: 'درباره ما', url: '#' },
          { title: 'تماس با ما', url: '#' },
          { title: 'قوانین و مقررات', url: '#' },
          { title: 'حریم خصوصی', url: '#' }
        ];
        this.contactInfo = 'تلفن: 021-12345678 | ایمیل: info@lightmarket.ir';
      }
    });
  }
}
