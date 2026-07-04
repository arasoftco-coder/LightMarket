import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { CampaignService } from '../services/campaign.service';
import { CartService } from '../services/cart.service';

interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  discountPrice?: number;
  stock: number;
  images: string[];
  specifications?: { key: string; value: string }[];
}

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, InputNumberModule],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit {
  product: Product | null = null;
  quantity: number = 1;
  selectedImageIndex: number = 0;
  isLoading: boolean = true;

  constructor(
    private route: ActivatedRoute,
    private campaignService: CampaignService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    const productId = this.route.snapshot.paramMap.get('id');
    if (productId) {
      this.loadProduct(+productId);
    }
  }

  loadProduct(id: number): void {
    this.isLoading = true;
    // TODO: Replace with actual API call to get product details
    // For now, fetch from active campaign products
    this.campaignService.getActiveCampaign().subscribe({
      next: (campaign: any) => {
        if (campaign) {
          this.campaignService.getCampaignProducts(campaign.id).subscribe({
            next: (products: any) => {
              this.product = products?.find((p: any) => p.id === id) || null;
              this.isLoading = false;
            },
            error: () => {
              this.isLoading = false;
            }
          });
        } else {
          this.product = null;
          this.isLoading = false;
        }
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  addToCart(): void {
    if (this.product) {
      this.cartService.addToCart(this.product.id, this.quantity).subscribe({
        next: () => {
          alert('محصول به سبد خرید اضافه شد');
        },
        error: (err: any) => {
          alert('خطا در افزودن به سبد خرید');
        }
      });
    }
  }

  selectImage(index: number): void {
    this.selectedImageIndex = index;
  }

  get finalPrice(): number {
    if (!this.product) return 0;
    return this.product.discountPrice || this.product.price;
  }

  get hasDiscount(): boolean {
    return !!this.product?.discountPrice;
  }

  get mainImage(): string {
    if (!this.product || !this.product.images.length) return '';
    return this.product.images[this.selectedImageIndex] || this.product.images[0];
  }
}
