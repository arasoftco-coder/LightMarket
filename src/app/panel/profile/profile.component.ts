import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
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

  constructor(
    private userService: UserService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadProfile();
    const currentUser = this.authService.getCurrentUser();
    if (currentUser) {
      this.user = { ...this.user, ...currentUser };
    }
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
    if (!this.passwords.new) {
      alert('رمز عبور جدید را وارد کنید.');
      return;
    }
    if (this.passwords.new !== this.passwords.confirm) {
      alert('رمز عبور جدید و تکرار آن مطابقت ندارند.');
      return;
    }

    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      alert('برای تنظیم رمز عبور ابتدا وارد شوید.');
      return;
    }

    this.authService.setPassword(currentUser.id, this.passwords.new).subscribe({
      next: () => {
        alert('رمز عبور با موفقیت تنظیم شد. از این پس می‌توانید با رمز عبور وارد شوید.');
        this.showPasswordForm = false;
        this.passwords = { current: '', new: '', confirm: '' };
      },
      error: (err: any) => {
        console.error('Error setting password:', err);
        alert('خطا در تنظیم رمز عبور.');
      }
    });
  }
}
