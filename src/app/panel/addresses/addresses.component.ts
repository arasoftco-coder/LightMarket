import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../services/user.service';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';

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
  imports: [CommonModule, FormsModule, ButtonModule, InputTextModule, DialogModule],
  templateUrl: './addresses.component.html',
  styleUrls: ['./addresses.component.css']
})
export class AddressesComponent implements OnInit {
  addresses: Address[] = [];
  showDialog: boolean = false;
  editingAddress: Partial<Address> | null = null;
  newAddress: Partial<Address> = {};

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

  openAddDialog(): void {
    this.editingAddress = null;
    this.newAddress = {};
    this.showDialog = true;
  }

  openEditDialog(address: Address): void {
    this.editingAddress = { ...address };
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
