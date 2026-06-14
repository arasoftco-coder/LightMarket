import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AuthComponent } from './auth/auth.component';
import { CartComponent } from './cart/cart.component';
import { CheckoutComponent } from './checkout/checkout.component';
import { UserPanelComponent } from './panel/user-panel.component';
import { MagicLinkCheckoutComponent } from './magic-link-checkout/magic-link-checkout.component';

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
  { path: 'pay/:token', component: MagicLinkCheckoutComponent },
  { path: '**', redirectTo: '' }
];
