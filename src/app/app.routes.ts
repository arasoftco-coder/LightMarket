import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AuthComponent } from './auth/auth.component';
import { CartComponent } from './cart/cart.component';
import { CheckoutComponent } from './checkout/checkout.component';
import { UserPanelComponent } from './panel/user-panel.component';
import { MagicLinkCheckoutComponent } from './magic-link-checkout/magic-link-checkout.component';
import { AdminComponent } from './admin/admin.component';
import { AdminDashboardComponent } from './admin/dashboard/dashboard.component';
import { AdminCampaignsComponent } from './admin/campaigns/campaigns.component';
import { AdminCampaignEditComponent } from './admin/campaigns/campaign-edit.component';
import { AdminSuppliersComponent } from './admin/suppliers/suppliers.component';
import { AdminSupplierEditComponent } from './admin/suppliers/supplier-edit.component';
import { AdminOrdersComponent } from './admin/orders/orders.component';
import { AdminOrderDetailComponent } from './admin/orders/order-detail.component';
import { AdminProductsComponent } from './admin/products/products.component';
import { ExcelImportComponent } from './admin/products/excel-import.component';
import { AdminSettingsComponent } from './admin/settings/settings.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'auth', component: AuthComponent },
  { path: 'cart', component: CartComponent },
  { path: 'checkout', component: CheckoutComponent },
  { 
    path: 'panel', 
    component: UserPanelComponent,
    children: [
      { path: 'profile', loadComponent: () => import('./panel/profile/profile.component').then(m => m.ProfileComponent) },
      { path: 'addresses', loadComponent: () => import('./panel/addresses/addresses.component').then(m => m.AddressesComponent) },
      { path: 'orders', loadComponent: () => import('./panel/orders/orders.component').then(m => m.OrdersComponent) },
      { path: 'tickets', loadComponent: () => import('./panel/tickets/tickets.component').then(m => m.TicketsComponent) }
    ]
  },
  { 
    path: 'admin', 
    component: AdminComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: AdminDashboardComponent },
      { path: 'campaigns', component: AdminCampaignsComponent },
      { path: 'campaigns/new', component: AdminCampaignEditComponent },
      { path: 'campaigns/edit/:id', component: AdminCampaignEditComponent },
      { path: 'suppliers', component: AdminSuppliersComponent },
      { path: 'suppliers/new', component: AdminSupplierEditComponent },
      { path: 'suppliers/edit/:id', component: AdminSupplierEditComponent },
      { path: 'orders', component: AdminOrdersComponent },
      { path: 'orders/:id', component: AdminOrderDetailComponent },
      { path: 'products', component: AdminProductsComponent },
      { path: 'products/import-excel', component: ExcelImportComponent },
      { path: 'settings', component: AdminSettingsComponent }
    ]
  },
  { path: 'pay/:token', component: MagicLinkCheckoutComponent },
  { path: '**', redirectTo: '' }
];
