import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, InputTextModule, PasswordModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  user: any = {};
  editing: boolean = false;
  showPasswordForm: boolean = false;
  passwords: { current: string; new: string; confirm: string } = { current: '', new: '', confirm: '' };

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.userService.getProfile().subscribe({
      next: (data: any) => {
        this.user = data;
      },
      error: (err: any) => console.error('Error loading profile:', err)
    });
  }

  saveProfile(): void {
    this.userService.updateProfile(this.user).subscribe({
      next: () => {
        this.editing = false;
        alert('پروفایل با موفقیت به‌روزرسانی شد.');
      },
      error: (err: any) => {
        console.error('Error updating profile:', err);
        alert('خطا در به‌روزرسانی پروفایل.');
      }
    });
  }

  changePassword(): void {
    if (this.passwords.new !== this.passwords.confirm) {
      alert('رمز عبور جدید و تکرار آن مطابقت ندارند.');
      return;
    }
    
    // TODO: Call API to change password
    alert('تغییر رمز عبور با موفقیت انجام شد.');
    this.showPasswordForm = false;
    this.passwords = { current: '', new: '', confirm: '' };
  }
}
