import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../services/admin.service';

@Component({
  selector: 'app-admin-excel-import',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="excel-import-page">
      <h2>واردات محصولات از اکسل</h2>
      
      <div class="upload-section">
        <input type="file" (change)="onFileSelected($event)" accept=".xlsx,.xls" />
        <button (click)="uploadFile()" [disabled]="!selectedFile" class="btn-primary">آپلود و پردازش</button>
      </div>

      <div class="preview-section" *ngIf="matchResults.length > 0">
        <h3>پیش‌نمایش همسان‌سازی</h3>
        <table class="data-table">
          <thead>
            <tr>
              <th>نام در اکسل</th>
              <th>محصول پیشنهادی</th>
              <th>درصد اطمینان</th>
              <th>تأیید</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let match of matchResults">
              <td>{{ match.excelName }}</td>
              <td>{{ match.matchedProduct || 'بدون تطابق' }}</td>
              <td>
                <span [class.low-confidence]="match.confidence < 70">
                  {{ match.confidence }}%
                </span>
              </td>
              <td>
                <input type="checkbox" [(ngModel)]="match.confirmed" [disabled]="match.confidence < 70" />
              </td>
            </tr>
          </tbody>
        </table>
        <button (click)="confirmImport()" class="btn-success">تأیید نهایی و واردات</button>
      </div>
    </div>
  `,
  styles: [`
    .upload-section { background: #f8f9fa; padding: 30px; border-radius: 10px; margin-bottom: 30px; display: flex; gap: 15px; align-items: center; }
    .preview-section { background: white; padding: 25px; border-radius: 10px; box-shadow: 0 2px 8px rgba(0,0,0,0.05); }
    .data-table { width: 100%; border-collapse: collapse; margin: 20px 0; }
    .data-table th, .data-table td { padding: 12px; text-align: right; border-bottom: 1px solid #e9ecef; }
    .data-table th { background: #f8f9fa; font-weight: 600; }
    .low-confidence { color: #dc3545; font-weight: bold; }
    .btn-primary, .btn-success { padding: 12px 24px; border: none; border-radius: 6px; cursor: pointer; color: white; }
    .btn-primary { background: #007bff; }
    .btn-primary:disabled { background: #ccc; cursor: not-allowed; }
    .btn-success { background: #28a745; margin-top: 20px; }
  `]
})
export class ExcelImportComponent {
  selectedFile: File | null = null;
  matchResults: any[] = [];

  constructor(private adminService: AdminService) {}

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  uploadFile(): void {
    if (!this.selectedFile) return;

    this.adminService.importExcel(this.selectedFile).subscribe({
      next: (data) => {
        this.matchResults = data.matches || [];
      },
      error: (err) => console.error('Error importing Excel', err)
    });
  }

  confirmImport(): void {
    const confirmedItems = this.matchResults.filter(m => m.confirmed);
    this.adminService.confirmImport(confirmedItems).subscribe({
      next: (res) => {
        alert(`${confirmedItems.length} مورد با موفقیت وارد و ثبت شد.`);
        this.matchResults = [];
      },
      error: (err) => {
        console.error('Error confirming import', err);
        alert('خطا در ثبت نهایی کالاها: ' + (err.error?.message || err.message));
      }
    });
  }
}
