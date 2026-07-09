import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminService } from '../../services/admin.service';
import { DialogModule } from 'primeng/dialog';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule, DialogModule],
  template: `
    <div class="products-page">
      <div class="page-header">
        <h2>مدیریت محصولات</h2>
        <div class="actions">
          <button (click)="openAddModal()" class="btn-primary" style="background: #28a745;">کالای جدید</button>
          <button (click)="openImportModal()" class="btn-secondary">واردات از اکسل</button>
          <button (click)="openScrapeModal()" class="btn-primary">اسکرپ از تأمین‌کننده</button>
        </div>
      </div>
      <table class="data-table">
        <thead>
          <tr>
            <th>نام محصول</th>
            <th>قیمت پایه (تومان)</th>
            <th>توضیحات</th>
            <th style="width: 180px;">عملیات</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let product of products">
            <td>{{ product.name }}</td>
            <td>{{ product.basePrice | number }} تومان</td>
            <td>{{ product.description || '---' }}</td>
            <td>
              <button (click)="openEditModal(product)" class="btn-sm btn-edit">ویرایش</button>
              <button (click)="deleteProduct(product.id)" class="btn-sm btn-delete">حذف</button>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Manual Product Edit Dialog -->
      <p-dialog [header]="isProductEdit ? 'ویرایش کالا' : 'افزودن کالا'" [(visible)]="showProductModal" [modal]="true" [style]="{ width: '450px' }">
        <div class="product-form" dir="rtl">
          <div class="form-group">
            <label>نام کالا:</label>
            <input type="text" [(ngModel)]="currentProduct.name" class="w-full input-control" required />
          </div>
          <div class="form-group">
            <label>قیمت پایه (تومان):</label>
            <input type="number" [(ngModel)]="currentProduct.basePrice" class="w-full input-control" required />
          </div>
          <div class="form-group">
            <label>توضیحات:</label>
            <textarea [(ngModel)]="currentProduct.description" class="w-full input-control" rows="3"></textarea>
          </div>
          <div class="form-group">
            <label>آدرس تصویر (URL):</label>
            <input type="text" [(ngModel)]="currentProduct.imageUrl" class="w-full input-control" />
          </div>
          <button (click)="saveProduct()" class="btn-success w-full" style="margin-top: 15px;">ذخیره کالا</button>
        </div>
      </p-dialog>

      <!-- Scrape Dialog -->
      <p-dialog header="اسکرپ محصولات از سایت تأمین‌کننده" [(visible)]="showScrapeDialog" [modal]="true" [style]="{ width: '800px' }">
        <div class="scrape-form" dir="rtl">
          <div class="form-group">
            <label>انتخاب تأمین‌کننده:</label>
            <select [(ngModel)]="scrapeConfig.supplierId" class="w-full select-control">
              <option *ngFor="let supplier of suppliers" [value]="supplier.id">{{ supplier.name }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>آدرس صفحه محصولات تأمین‌کننده:</label>
            <input type="text" [(ngModel)]="scrapeConfig.url" class="w-full input-control" placeholder="https://supplier.com/products" />
          </div>
          
          <div class="selectors-grid">
            <div class="form-group">
              <label>سلکتور نگهدارنده کالا (Container):</label>
              <input type="text" [(ngModel)]="scrapeConfig.containerSelector" class="w-full input-control" />
            </div>
            <div class="form-group">
              <label>سلکتور نام کالا (Name):</label>
              <input type="text" [(ngModel)]="scrapeConfig.nameSelector" class="w-full input-control" />
            </div>
            <div class="form-group">
              <label>سلکتور قیمت کالا (Price):</label>
              <input type="text" [(ngModel)]="scrapeConfig.priceSelector" class="w-full input-control" />
            </div>
            <div class="form-group">
              <label>سلکتور موجودی کالا (Stock):</label>
              <input type="text" [(ngModel)]="scrapeConfig.stockSelector" class="w-full input-control" />
            </div>
          </div>

          <button (click)="runScraper()" class="btn-primary w-full scrape-btn" [disabled]="isScraping">
            {{ isScraping ? 'در حال اسکرپ و پردازش...' : 'شروع اسکرپ محصولات' }}
          </button>

          <!-- Preview Scrape Results -->
          <div class="preview-section" *ngIf="scrapedProducts.length > 0">
            <h3>پیش‌نمایش محصولات اسکرپ شده</h3>
            <table class="data-table">
              <thead>
                <tr>
                  <th>نام کالا</th>
                  <th>قیمت</th>
                  <th>موجودی</th>
                  <th style="width: 100px; text-align: center;">انتخاب</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let item of scrapedProducts">
                  <td>{{ item.name }}</td>
                  <td>{{ item.price | number }} تومان</td>
                  <td>{{ item.stock }}</td>
                  <td style="text-align: center;">
                    <input type="checkbox" [(ngModel)]="item.selected" />
                  </td>
                </tr>
              </tbody>
            </table>
            <button (click)="confirmScrapeImport()" class="btn-success w-full">تأیید و واردات به کاتالوگ</button>
          </div>
        </div>
      </p-dialog>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px; flex-wrap: wrap; gap: 15px; }
    .actions button { padding: 10px 20px; margin-left: 10px; border: none; border-radius: 6px; cursor: pointer; }
    .btn-primary { background: #007bff; color: white; }
    .btn-secondary { background: #6c757d; color: white; }
    .data-table { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.05); }
    .data-table th, .data-table td { padding: 15px; text-align: right; border-bottom: 1px solid #e9ecef; }
    .data-table th { background: #f8f9fa; font-weight: 600; color: #495057; }
    .data-table tr:hover { background: #f8f9fa; }
    .btn-sm { padding: 6px 12px; margin-left: 5px; border: none; border-radius: 4px; cursor: pointer; }
    .btn-edit { background: #ffc107; color: #212529; }
    .btn-delete { background: #dc3545; color: white; }
    .scrape-form, .product-form { padding: 15px; }
    .form-group { margin-bottom: 15px; }
    .form-group label { display: block; margin-bottom: 5px; font-weight: 600; color: #495057; }
    .w-full { width: 100%; }
    .input-control, .select-control { padding: 10px; border: 1px solid #ced4da; border-radius: 6px; font-family: inherit; }
    .selectors-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 20px; margin-top: 15px; }
    .scrape-btn { padding: 12px; font-size: 1rem; border: none; border-radius: 6px; cursor: pointer; margin-top: 10px; }
    .scrape-btn:disabled { background: #ccc; cursor: not-allowed; }
    .preview-section { margin-top: 25px; border-top: 2px solid #e9ecef; padding-top: 20px; }
    .btn-success { background: #28a745; color: white; padding: 12px; border: none; border-radius: 6px; cursor: pointer; font-size: 1rem; margin-top: 15px; }
  `]
})
export class AdminProductsComponent implements OnInit {
  products: any[] = [];
  suppliers: any[] = [];
  showScrapeDialog = false;
  isScraping = false;
  scrapedProducts: any[] = [];
  
  showProductModal = false;
  isProductEdit = false;
  currentProduct: any = { id: null, name: '', basePrice: 0, description: '', imageUrl: '' };

  scrapeConfig = {
    supplierId: null,
    url: '',
    containerSelector: '.product-item',
    nameSelector: '.product-title',
    priceSelector: '.product-price',
    stockSelector: '.product-stock'
  };

  constructor(
    private adminService: AdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadSuppliers();
  }

  loadProducts(): void {
    this.adminService.getProducts().subscribe({
      next: (data) => this.products = data || [],
      error: (err) => console.error('Error loading products', err)
    });
  }

  loadSuppliers(): void {
    this.adminService.getSuppliers().subscribe({
      next: (data) => {
        this.suppliers = data || [];
        if (this.suppliers.length > 0) {
          this.scrapeConfig.supplierId = this.suppliers[0].id;
        }
      },
      error: (err) => console.error('Error loading suppliers', err)
    });
  }

  openImportModal(): void {
    this.router.navigate(['/admin/products/import-excel']);
  }

  openScrapeModal(): void {
    this.scrapedProducts = [];
    this.showScrapeDialog = true;
  }

  openAddModal(): void {
    this.isProductEdit = false;
    this.currentProduct = { id: null, name: '', basePrice: 0, description: '', imageUrl: '' };
    this.showProductModal = true;
  }

  openEditModal(product: any): void {
    this.isProductEdit = true;
    this.currentProduct = { ...product };
    this.showProductModal = true;
  }

  saveProduct(): void {
    if (!this.currentProduct.name || this.currentProduct.basePrice <= 0) {
      alert('لطفاً نام و قیمت صحیح برای کالا وارد کنید.');
      return;
    }

    if (this.isProductEdit) {
      this.adminService.updateProduct(this.currentProduct.id, this.currentProduct).subscribe({
        next: () => {
          alert('کالا با موفقیت ویرایش شد.');
          this.showProductModal = false;
          this.loadProducts();
        },
        error: (err: any) => alert('خطا در ویرایش کالا: ' + err.message)
      });
    } else {
      this.adminService.createProduct(this.currentProduct).subscribe({
        next: () => {
          alert('کالا با موفقیت ایجاد شد.');
          this.showProductModal = false;
          this.loadProducts();
        },
        error: (err: any) => alert('خطا در ایجاد کالا: ' + err.message)
      });
    }
  }

  deleteProduct(id: number): void {
    if (confirm('آیا از حذف این کالا اطمینان دارید؟')) {
      this.adminService.deleteProduct(id).subscribe({
        next: () => {
          alert('کالا با موفقیت حذف شد.');
          this.loadProducts();
        },
        error: (err: any) => alert('خطا در حذف کالا: ' + err.message)
      });
    }
  }

  runScraper(): void {
    if (!this.scrapeConfig.url || !this.scrapeConfig.supplierId) {
      alert('لطفاً آدرس سایت و تأمین‌کننده را مشخص کنید.');
      return;
    }

    this.isScraping = true;
    const body = {
      url: this.scrapeConfig.url,
      extractionConfig: {
        ContainerSelector: this.scrapeConfig.containerSelector,
        NameSelector: this.scrapeConfig.nameSelector,
        PriceSelector: this.scrapeConfig.priceSelector,
        StockSelector: this.scrapeConfig.stockSelector
      }
    };

    this.adminService.scrapeProducts(this.scrapeConfig.supplierId, body).subscribe({
      next: (res: any) => {
        this.isScraping = false;
        if (res.success && res.data) {
          this.scrapedProducts = res.data.map((item: any) => ({
            ...item,
            selected: true
          }));
          if (this.scrapedProducts.length === 0) {
            alert('هیچ محصولی با سلکتورهای مشخص‌شده پیدا نشد.');
          }
        } else {
          alert('خطا در انجام اسکرپ.');
        }
      },
      error: (err) => {
        this.isScraping = false;
        console.error('Error running scraper', err);
        alert('خطا در اجرای اسکرپر.');
      }
    });
  }

  confirmScrapeImport(): void {
    const selectedItems = this.scrapedProducts.filter(p => p.selected);
    if (selectedItems.length === 0) {
      alert('لطفاً حداقل یک کالا را انتخاب کنید.');
      return;
    }

    // Map to ConfirmImportItem format
    const confirmItems = selectedItems.map(item => ({
      excelName: item.name,
      confirmed: true,
      description: 'اسکرپ شده از سایت تأمین‌کننده',
      basePrice: item.price,
      purchasePrice: item.price,
      sellingPrice: item.price * 1.2, // markup
      discount: 0,
      stock: item.stock || 10,
      minQtyPerUser: 1,
      maxQtyPerUser: 10
    }));

    this.adminService.confirmImport(confirmItems).subscribe({
      next: () => {
        alert(`${confirmItems.length} کالا با موفقیت ذخیره و به کمپین جاری ایمپورت شد.`);
        this.showScrapeDialog = false;
        this.loadProducts();
      },
      error: (err) => {
        console.error('Error importing scraped items', err);
        alert('خطا در ثبت نهایی کالاها: ' + (err.error?.message || err.message));
      }
    });
  }
}
