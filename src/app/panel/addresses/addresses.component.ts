import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';

interface Address {
  id: number;
  province: string;
  city: string;
  street: string;
  postalCode: string;
  fullAddress: string;
}

@Component({
  selector: 'app-addresses',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, InputTextModule, DialogModule, DropdownModule],
  templateUrl: './addresses.component.html',
  styleUrls: ['./addresses.component.css']
})
export class AddressesComponent implements OnInit {
  addresses: Address[] = [];
  showDialog: boolean = false;
  editingAddress: Partial<Address> | null = null;
  newAddress: Partial<Address> = {};

  provinces: string[] = ['تهران', 'اصفهان', 'خراسان رضوی', 'فارس', 'آذربایجان شرقی', 'مازندران', 'گیلان', 'البرز'];
  cities: string[] = [];
  citiesMap: { [key: string]: string[] } = {
    'تهران': ['تهران', 'ری', 'شمیرانات', 'اسلامشهر', 'شهریار', 'ورامین', 'دماوند', 'پاکدشت'],
    'اصفهان': ['اصفهان', 'کاشان', 'خمینی‌شهر', 'نجف‌آباد', 'شاهین‌شهر', 'شهرضا', 'گلپایگان'],
    'خراسان رضوی': ['مشهد', 'نیشابور', 'سبزوار', 'تربت حیدریه', 'قوچان', 'کاشمر', 'گناباد'],
    'فارس': ['شیراز', 'مرودشت', 'جهرم', 'فسا', 'کازرون', 'لارستان', 'داراب'],
    'آذربایجان شرقی': ['تبریز', 'مراغه', 'مرند', 'میانه', 'اهر', 'بناب'],
    'مازندران': ['ساری', 'بابل', 'آمل', 'قائم‌شهر', 'بهشهر', 'چالوس', 'تنکابن', 'رامسر'],
    'گیلان': ['رشت', 'بندرانزلی', 'لاهیجان', 'لنگرود', 'تالش', 'آستارا', 'رودسر'],
    'البرز': ['کرج', 'فردیس', 'نظرآباد', 'هشتگرد', 'طالقان']
  };

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.loadAddresses();
  }

  loadAddresses(): void {
    this.userService.getAddresses().subscribe({
      next: (data: any) => {
        this.addresses = data;
      },
      error: (err: any) => console.error('Error loading addresses:', err)
    });
  }

  onProvinceChange(): void {
    const prov = this.newAddress.province;
    this.cities = prov ? this.citiesMap[prov] || [] : [];
    this.newAddress.city = '';
  }

  openAddDialog(): void {
    this.editingAddress = null;
    this.newAddress = { province: '', city: '', street: '', postalCode: '' };
    this.cities = [];
    this.showDialog = true;
  }

  openEditDialog(address: Address): void {
    this.editingAddress = { ...address };
    this.newAddress = { ...address };
    const prefix = `${address.province}, ${address.city}, `;
    if (address.fullAddress && address.fullAddress.startsWith(prefix)) {
      this.newAddress.street = address.fullAddress.substring(prefix.length);
    } else {
      this.newAddress.street = address.fullAddress;
    }
    this.cities = address.province ? this.citiesMap[address.province] || [] : [];
    this.showDialog = true;
  }

  saveAddress(): void {
    const addressData = {
      ...this.newAddress,
      fullAddress: `${this.newAddress.province}, ${this.newAddress.city}, ${this.newAddress.street}`
    };

    if (this.editingAddress && this.editingAddress.id) {
      // Update existing
      this.userService.updateAddress(this.editingAddress.id, addressData).subscribe({
        next: () => {
          this.loadAddresses();
          this.showDialog = false;
        },
        error: (err: any) => console.error('Error updating address:', err)
      });
    } else {
      // Add new
      this.userService.addAddress(addressData).subscribe({
        next: () => {
          this.loadAddresses();
          this.showDialog = false;
        },
        error: (err: any) => console.error('Error adding address:', err)
      });
    }
  }

  deleteAddress(id: number): void {
    if (!confirm('آیا از حذف این آدرس مطمئن هستید؟')) return;

    this.userService.deleteAddress(id).subscribe({
      next: () => {
        this.addresses = this.addresses.filter(a => a.id !== id);
      },
      error: (err: any) => console.error('Error deleting address:', err)
    });
  }
}
