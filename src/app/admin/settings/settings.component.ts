import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../services/admin.service';

@Component({
  selector: 'app-admin-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="settings-page">
      <h2>تنظیمات سیستم</h2>
      
      <form (ngSubmit)="saveSettings()" class="settings-form">
        <div class="settings-section">
          <h3>تنظیمات پیامک</h3>
          <div class="form-group">
            <label>API Key:</label>
            <input type="text" [(ngModel)]="settings.smsApiKey" name="smsApiKey" />
          </div>
          <div class="form-group">
            <label>شناسه فرستنده:</label>
            <input type="text" [(ngModel)]="settings.senderId" name="senderId" />
          </div>
        </div>

        <div class="settings-section">
          <h3>تنظیمات درگاه پرداخت</h3>
          <div class="form-group">
            <label>Merchant ID:</label>
            <input type="text" [(ngModel)]="settings.paymentMerchantId" name="paymentMerchantId" />
          </div>
          <div class="form-group">
            <label>Callback URL:</label>
            <input type="text" [(ngModel)]="settings.callbackUrl" name="callbackUrl" />
          </div>
        </div>

        <div class="settings-section">
          <h3>لینک جادویی پرداخت</h3>
          <div class="form-group">
            <label>مدت اعتبار (دقیقه):</label>
            <input type="number" [(ngModel)]="settings.magicLinkExpiry" name="magicLinkExpiry" />
          </div>
        </div>

        <div class="settings-section">
          <h3>محتوای فوتر</h3>
          <div class="form-group">
            <label>لینک‌ها:</label>
            <textarea [(ngModel)]="settings.footerLinks" name="footerLinks" rows="3"></textarea>
          </div>
          <div class="form-group">
            <label>اطلاعات تماس:</label>
            <textarea [(ngModel)]="settings.contactInfo" name="contactInfo" rows="3"></textarea>
          </div>
        </div>

        <button type="submit" class="btn-primary">ذخیره تنظیمات</button>
      </form>
    </div>
  `,
  styles: [`
    .settings-form { max-width: 800px; }
    .settings-section { background: #f8f9fa; padding: 25px; border-radius: 10px; margin-bottom: 25px; }
    .settings-section h3 { margin-top: 0; margin-bottom: 20px; color: #495057; border-bottom: 2px solid #e9ecef; padding-bottom: 10px; }
    .form-group { margin-bottom: 20px; }
    .form-group label { display: block; margin-bottom: 8px; font-weight: 600; }
    .form-group input, .form-group textarea { width: 100%; padding: 10px; border: 1px solid #ced4da; border-radius: 6px; font-family: inherit; }
    .btn-primary { padding: 12px 30px; background: #007bff; color: white; border: none; border-radius: 6px; cursor: pointer; font-size: 1rem; }
  `]
})
export class AdminSettingsComponent implements OnInit {
  settings: any = {
    smsApiKey: '',
    senderId: '',
    paymentMerchantId: '',
    callbackUrl: '',
    magicLinkExpiry: 30,
    footerLinks: '',
    contactInfo: ''
  };

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadSettings();
  }

  loadSettings(): void {
    // Load settings from API
    this.settings = {
      smsApiKey: 'YOUR_SMS_API_KEY',
      senderId: 'LightMarket',
      paymentMerchantId: 'YOUR_MERCHANT_ID',
      callbackUrl: 'https://lightmarket.ir/payment/callback',
      magicLinkExpiry: 30,
      footerLinks: 'تماس با ما | قوانین و مقررات | حریم خصوصی',
      contactInfo: 'تلفن: 021-12345678 | ایمیل: info@lightmarket.ir'
    };
  }

  saveSettings(): void {
    alert('تنظیمات با موفقیت ذخیره شد');
  }
}
