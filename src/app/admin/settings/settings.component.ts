import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../services/admin.service';
import { DialogModule } from 'primeng/dialog';

@Component({
  selector: 'app-admin-settings',
  standalone: true,
  imports: [CommonModule, FormsModule, DialogModule],
  template: `
    <div class="settings-page">
      <h2>تنظیمات سیستم</h2>

      <div class="tabs-header">
        <button type="button" (click)="activeTab = 'general'" [class.active]="activeTab === 'general'" class="tab-btn">تنظیمات عمومی</button>
        <button type="button" (click)="activeTab = 'payments'" [class.active]="activeTab === 'payments'" class="tab-btn">روش‌های پرداخت پویا</button>
      </div>
      
      <form *ngIf="activeTab === 'general'" (ngSubmit)="saveSettings()" class="settings-form">
        <div class="settings-section">
          <h3>تنظیمات پیامک</h3>
          <div class="form-group">
            <label>API Key:</label>
            <input type="text" [(ngModel)]="settings.smsApiKey" name="smsApiKey" />
          </div>
          <div class="form-group">
            <label>شناسه فرستنده (Sender ID):</label>
            <input type="text" [(ngModel)]="settings.senderId" name="senderId" />
          </div>
        </div>

        <div class="settings-section">
          <h3>تنظیمات پیش‌فرض درگاه پرداخت (فالبک)</h3>
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
            <label style="margin-bottom: 12px; display: block;">لینک‌های فوتر:</label>
            <div *ngFor="let link of settings.footerLinks; let i = index" class="link-item">
              <input type="text" [(ngModel)]="link.title" placeholder="عنوان لینک (مثال: تماس با ما)" name="link_title_{{i}}" />
              <input type="text" [(ngModel)]="link.url" placeholder="آدرس لینک (مثال: /contact)" name="link_url_{{i}}" />
              <button type="button" (click)="removeLink(i)" class="btn-danger-sm">حذف</button>
            </div>
            <button type="button" (click)="addLink()" class="btn-secondary-sm">افزودن لینک جدید</button>
          </div>
          <div class="form-group" style="margin-top: 25px;">
            <label>اطلاعات تماس:</label>
            <textarea [(ngModel)]="settings.contactInfo" name="contactInfo" rows="3"></textarea>
          </div>
        </div>

        <button type="submit" class="btn-primary">ذخیره تنظیمات</button>
      </form>

      <div *ngIf="activeTab === 'payments'" class="payments-tab">
        <div class="section-header" style="margin-top: 10px;">
          <h3>مدیریت روش‌های پرداخت پویا</h3>
          <button type="button" (click)="openAddPaymentModal()" class="btn-success-sm">ثبت روش پرداخت جدید</button>
        </div>

        <table class="data-table">
          <thead>
            <tr>
              <th>نام روش</th>
              <th>نوع</th>
              <th>درگاه / بانک</th>
              <th>وضعیت</th>
              <th style="width: 150px; text-align: center;">عملیات</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let method of paymentMethods">
              <td>{{ method.name }}</td>
              <td>{{ method.type === 'Gateway' ? 'درگاه آنلاین' : 'کارت به کارت' }}</td>
              <td>{{ getGatewayLabel(method.gatewayName) }}</td>
              <td>
                <span [class.badge-active]="method.isActive" [class.badge-inactive]="!method.isActive">
                  {{ method.isActive ? 'فعال' : 'غیرفعال' }}
                </span>
              </td>
              <td style="text-align: center;">
                <button type="button" (click)="openEditPaymentModal(method)" class="btn-edit-sm">ویرایش</button>
                <button type="button" (click)="deletePaymentMethod(method.id)" class="btn-delete-sm">حذف</button>
              </td>
            </tr>
            <tr *ngIf="paymentMethods.length === 0">
              <td colspan="5" style="text-align: center; padding: 20px; color: #888;">هیچ روش پرداختی تعریف نشده است.</td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Payment Method Dialog -->
      <p-dialog [header]="isEditingPayment ? 'ویرایش روش پرداخت' : 'افزودن روش پرداخت'" [(visible)]="showPaymentModal" [modal]="true" [style]="{ width: '500px' }">
        <div class="payment-method-form" dir="rtl">
          <div class="form-group">
            <label>نام نمایشی روش پرداخت:</label>
            <input type="text" [(ngModel)]="currentPaymentMethod.name" placeholder="مثال: درگاه پاسارگاد یا کارت به کارت بانک پاسارگاد" class="w-full input-control" required />
          </div>
          <div class="form-group">
            <label>نوع پرداخت:</label>
            <select [(ngModel)]="currentPaymentMethod.type" (change)="onPaymentTypeChange()" class="w-full select-control">
              <option value="Gateway">درگاه آنلاین بانکی</option>
              <option value="CardToCard">کارت به کارت دستی</option>
            </select>
          </div>
          
          <!-- Gateway Fields -->
          <div *ngIf="currentPaymentMethod.type === 'Gateway'">
            <div class="form-group">
              <label>نام درگاه بانکی:</label>
              <select [(ngModel)]="currentPaymentMethod.gatewayName" class="w-full select-control">
                <option value="ZarinPal">زرین‌پال (ZarinPal)</option>
                <option value="Pasargad">بانک پاسارگاد (Pasargad)</option>
                <option value="Mellat">بانک ملت (Mellat)</option>
                <option value="Saman">بانک سامان (Saman)</option>
              </select>
            </div>
            
            <div class="form-group">
              <label>شناسه پذیرنده (Merchant ID):</label>
              <input type="text" [(ngModel)]="tempConfig.merchantId" class="w-full input-control" />
            </div>

            <div class="form-group" *ngIf="currentPaymentMethod.gatewayName === 'Pasargad' || currentPaymentMethod.gatewayName === 'Mellat'">
              <label>شناسه ترمینال (Terminal ID):</label>
              <input type="text" [(ngModel)]="tempConfig.terminalId" class="w-full input-control" />
            </div>

            <div class="form-group" *ngIf="currentPaymentMethod.gatewayName === 'Mellat'">
              <label>نام کاربری درگاه (Username):</label>
              <input type="text" [(ngModel)]="tempConfig.userName" class="w-full input-control" />
            </div>

            <div class="form-group" *ngIf="currentPaymentMethod.gatewayName === 'Mellat' || currentPaymentMethod.gatewayName === 'Saman'">
              <label>رمز عبور درگاه (Password):</label>
              <input type="password" [(ngModel)]="tempConfig.password" class="w-full input-control" />
            </div>

            <div class="form-group" *ngIf="currentPaymentMethod.gatewayName === 'Pasargad'">
              <label>کد یا گواهی دیجیتال (Certificate Code):</label>
              <textarea [(ngModel)]="tempConfig.certificateCode" rows="3" class="w-full input-control" placeholder="کد امضا یا گواهی دیجیتال اختصاصی بانک"></textarea>
            </div>

            <div class="form-group">
              <label>آدرس پایه پرداخت (Gateway Payment URL):</label>
              <input type="text" [(ngModel)]="tempConfig.gatewayUrl" class="w-full input-control" placeholder="مثال: https://pep.shaparak.ir/payment.aspx" />
            </div>

            <div class="form-group">
              <label>آدرس برگشت (Callback URL):</label>
              <input type="text" [(ngModel)]="tempConfig.callbackUrl" class="w-full input-control" placeholder="https://yourdomain.com/payment/callback" />
            </div>
          </div>

          <!-- CardToCard Fields -->
          <div *ngIf="currentPaymentMethod.type === 'CardToCard'">
            <div class="form-group">
              <label>شماره کارت ۱۶ رقمی:</label>
              <input type="text" [(ngModel)]="tempConfig.cardNumber" placeholder="مثال: ۶۰۳۷۹۹..." class="w-full input-control" />
            </div>
            <div class="form-group">
              <label>نام صاحب حساب / کارت:</label>
              <input type="text" [(ngModel)]="tempConfig.holderName" placeholder="مثال: علی محمدی" class="w-full input-control" />
            </div>
            <div class="form-group">
              <label>نام بانک:</label>
              <input type="text" [(ngModel)]="tempConfig.bankName" placeholder="مثال: بانک پاسارگاد" class="w-full input-control" />
            </div>
          </div>

          <div class="form-group">
            <label>توضیحات روش پرداخت:</label>
            <textarea [(ngModel)]="currentPaymentMethod.description" rows="2" class="w-full input-control" placeholder="توضیحاتی که به خریدار نمایش داده می‌شود"></textarea>
          </div>

          <div class="form-group checkbox-group" style="display: flex; align-items: center; gap: 8px; margin-top: 15px;">
            <input type="checkbox" [(ngModel)]="currentPaymentMethod.isActive" id="is_payment_active" />
            <label for="is_payment_active" style="margin-bottom: 0; cursor: pointer; font-weight: normal;">این روش پرداخت فعال باشد</label>
          </div>

          <button (click)="savePaymentMethod()" class="btn-success w-full" style="margin-top: 20px;">ذخیره روش پرداخت</button>
        </div>
      </p-dialog>
    </div>
  `,
  styles: [`
    .tabs-header { display: flex; gap: 10px; margin-bottom: 25px; border-bottom: 2px solid #dee2e6; padding-bottom: 10px; }
    .tab-btn { padding: 10px 20px; border: none; background: none; font-size: 1rem; font-family: inherit; cursor: pointer; color: #495057; font-weight: 500; }
    .tab-btn.active { color: #007bff; border-bottom: 3px solid #007bff; font-weight: 700; margin-bottom: -13px; }
    .settings-form { max-width: 800px; }
    .settings-section { background: #f8f9fa; padding: 25px; border-radius: 10px; margin-bottom: 25px; }
    .settings-section h3 { margin-top: 0; margin-bottom: 20px; color: #495057; border-bottom: 2px solid #e9ecef; padding-bottom: 10px; }
    .form-group { margin-bottom: 20px; }
    .form-group label { display: block; margin-bottom: 8px; font-weight: 600; }
    .form-group input, .form-group textarea, .form-group select { width: 100%; padding: 10px; border: 1px solid #ced4da; border-radius: 6px; font-family: inherit; }
    .link-item { display: flex; gap: 10px; margin-bottom: 10px; align-items: center; }
    .link-item input { flex: 1; }
    .btn-danger-sm { padding: 8px 15px; background: #dc3545; color: white; border: none; border-radius: 4px; cursor: pointer; }
    .btn-secondary-sm { padding: 8px 15px; background: #28a745; color: white; border: none; border-radius: 4px; cursor: pointer; margin-top: 5px; }
    .btn-primary { padding: 12px 30px; background: #007bff; color: white; border: none; border-radius: 6px; cursor: pointer; font-size: 1rem; }
    .section-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
    .btn-success-sm { padding: 8px 16px; background: #28a745; color: white; border: none; border-radius: 6px; cursor: pointer; font-size: 0.9rem; }
    .data-table { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.05); margin-top: 15px; }
    .data-table th, .data-table td { padding: 12px 15px; text-align: right; border-bottom: 1px solid #e9ecef; }
    .data-table th { background: #f8f9fa; font-weight: 600; }
    .badge-active { background: #d4edda; color: #155724; padding: 4px 8px; border-radius: 4px; font-size: 0.85rem; }
    .badge-inactive { background: #f8d7da; color: #721c24; padding: 4px 8px; border-radius: 4px; font-size: 0.85rem; }
    .btn-edit-sm { padding: 6px 12px; background: #ffc107; color: #212529; border: none; border-radius: 4px; cursor: pointer; margin-left: 5px; }
    .btn-delete-sm { padding: 6px 12px; background: #dc3545; color: white; border: none; border-radius: 4px; cursor: pointer; }
    .payment-method-form { padding: 10px; }
    .w-full { width: 100%; }
    .input-control, .select-control { padding: 10px; border: 1px solid #ced4da; border-radius: 6px; font-family: inherit; margin-top: 5px; }
    .btn-success { background: #28a745; color: white; padding: 12px; border: none; border-radius: 6px; cursor: pointer; font-size: 1rem; }
  `]
})
export class AdminSettingsComponent implements OnInit {
  activeTab: string = 'general';
  paymentMethods: any[] = [];
  showPaymentModal: boolean = false;
  isEditingPayment: boolean = false;
  currentPaymentMethod: any = { id: null, name: '', type: 'Gateway', gatewayName: 'ZarinPal', isActive: true, description: '' };
  tempConfig: any = {};

  settings: any = {
    smsApiKey: '',
    senderId: '',
    paymentMerchantId: '',
    callbackUrl: '',
    magicLinkExpiry: 30,
    footerLinks: [],
    contactInfo: ''
  };

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadSettings();
    this.loadPaymentMethods();
  }

  loadSettings(): void {
    this.adminService.getSettings().subscribe({
      next: (data) => {
        this.settings = data || {
          smsApiKey: '',
          senderId: '',
          paymentMerchantId: '',
          callbackUrl: '',
          magicLinkExpiry: 30,
          footerLinks: [],
          contactInfo: ''
        };
      },
      error: (err) => console.error('Error loading settings', err)
    });
  }

  loadPaymentMethods(): void {
    this.adminService.getPaymentMethods().subscribe({
      next: (data) => this.paymentMethods = data || [],
      error: (err) => console.error('Error loading payment methods', err)
    });
  }

  addLink(): void {
    if (!this.settings.footerLinks) {
      this.settings.footerLinks = [];
    }
    this.settings.footerLinks.push({ title: '', url: '' });
  }

  removeLink(index: number): void {
    this.settings.footerLinks.splice(index, 1);
  }

  saveSettings(): void {
    this.adminService.saveSettings(this.settings).subscribe({
      next: (res: any) => {
        alert(res.message || 'تنظیمات با موفقیت ذخیره شد');
        this.loadSettings();
      },
      error: (err) => {
        console.error('Error saving settings', err);
        alert('خطا در ذخیره تنظیمات');
      }
    });
  }

  onPaymentTypeChange(): void {
    if (this.currentPaymentMethod.type === 'CardToCard') {
      this.currentPaymentMethod.gatewayName = 'CardToCard';
      this.tempConfig = { cardNumber: '', holderName: '', bankName: '' };
    } else {
      this.currentPaymentMethod.gatewayName = 'ZarinPal';
      this.tempConfig = { merchantId: '', gatewayUrl: '', callbackUrl: '' };
    }
  }

  openAddPaymentModal(): void {
    this.isEditingPayment = false;
    this.currentPaymentMethod = { id: null, name: '', type: 'Gateway', gatewayName: 'ZarinPal', isActive: true, description: '' };
    this.tempConfig = { merchantId: '', gatewayUrl: '', callbackUrl: '' };
    this.showPaymentModal = true;
  }

  openEditPaymentModal(method: any): void {
    this.isEditingPayment = true;
    this.currentPaymentMethod = { ...method };
    try {
      this.tempConfig = JSON.parse(method.gatewayConfig || '{}');
    } catch {
      this.tempConfig = {};
    }
    this.showPaymentModal = true;
  }

  savePaymentMethod(): void {
    if (!this.currentPaymentMethod.name) {
      alert('لطفاً نام روش پرداخت را وارد کنید.');
      return;
    }
    this.currentPaymentMethod.gatewayConfig = JSON.stringify(this.tempConfig);

    if (this.isEditingPayment) {
      this.adminService.updatePaymentMethod(this.currentPaymentMethod.id, this.currentPaymentMethod).subscribe({
        next: () => {
          alert('روش پرداخت با موفقیت ویرایش شد.');
          this.showPaymentModal = false;
          this.loadPaymentMethods();
        },
        error: (err) => alert('خطا در ویرایش روش پرداخت: ' + err.message)
      });
    } else {
      this.adminService.createPaymentMethod(this.currentPaymentMethod).subscribe({
        next: () => {
          alert('روش پرداخت با موفقیت ثبت شد.');
          this.showPaymentModal = false;
          this.loadPaymentMethods();
        },
        error: (err) => alert('خطا در ثبت روش پرداخت: ' + err.message)
      });
    }
  }

  deletePaymentMethod(id: number): void {
    if (confirm('آیا از حذف این روش پرداخت اطمینان دارید؟')) {
      this.adminService.deletePaymentMethod(id).subscribe({
        next: () => {
          alert('روش پرداخت با موفقیت حذف شد.');
          this.loadPaymentMethods();
        },
        error: (err) => alert('خطا در حذف روش پرداخت: ' + err.message)
      });
    }
  }

  getGatewayLabel(name: string): string {
    switch (name) {
      case 'ZarinPal': return 'زرین‌پال';
      case 'Pasargad': return 'پاسارگاد';
      case 'Mellat': return 'ملت';
      case 'Saman': return 'سامان';
      case 'CardToCard': return 'کارت به کارت';
      default: return name;
    }
  }
}
