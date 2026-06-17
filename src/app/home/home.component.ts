import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CampaignService } from '../services/campaign.service';
import { CartService } from '../services/cart.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  products: any[] = [];
  campaign: any = null;
  loading = true;

  constructor(
    private campaignService: CampaignService,
    private cartService: CartService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCampaign();
  }

  loadCampaign(): void {
    this.campaignService.getActiveCampaign().subscribe({
      next: (data) => {
        this.campaign = data;
        this.loadProducts(data.id);
      },
      error: (err) => {
        console.error('Error loading campaign:', err);
        this.loading = false;
      }
    });
  }

  loadProducts(campaignId: number): void {
    this.campaignService.getCampaignProducts(campaignId).subscribe({
      next: (data) => {
        this.products = data.slice(0, 9); // Max 9 items
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading products:', err);
        this.loading = false;
      }
    });
  }

  addToCart(product: any): void {
    if (!this.campaign) return;
    
    this.cartService.addToCart(product.id, 1).subscribe({
      next: () => {
        alert('محصول به سبد خرید اضافه شد');
      },
      error: (err) => {
        console.error('Error adding to cart:', err);
        alert('خطا در افزودن به سبد خرید');
      }
    });
  }
}
