import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent {
  mode: 'otp' | 'password' = 'otp';
  step: 'phone' | 'otp' | 'register' = 'phone';
  phoneNumber: string = '';
  otpCode: string = '';
  fullName: string = '';
  password: string = '';
  loading: boolean = false;
  error: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  switchMode(mode: 'otp' | 'password'): void {
    this.mode = mode;
    this.step = 'phone';
    this.error = '';
    this.otpCode = '';
    this.password = '';
  }

  private isValidPhone(): boolean {
    return !!this.phoneNumber && this.phoneNumber.length === 11;
  }

  sendOtp(): void {
    if (!this.isValidPhone()) {
      this.error = 'شماره موبایل معتبر وارد کنید';
      return;
    }

    this.loading = true;
    this.error = '';

    this.authService.sendOtp(this.phoneNumber).subscribe({
      next: () => {
        this.step = 'otp';
        this.otpCode = '';
        this.loading = false;
      },
      error: () => {
        this.error = 'خطا در ارسال کد تأیید';
        this.loading = false;
      }
    });
  }

  verifyOtp(): void {
    if (!this.otpCode || this.otpCode.length !== 5) {
      this.error = 'کد تأیید 5 رقمی را وارد کنید';
      return;
    }

    this.loading = true;
    this.error = '';

    this.authService.verifyOtp(this.phoneNumber, this.otpCode).subscribe({
      next: (res: any) => {
        this.loading = false;
        if (res?.isNewUser) {
          // New account was created; ask for the user's name before continuing.
          this.step = 'register';
        } else {
          this.router.navigate(['']);
        }
      },
      error: () => {
        this.error = 'کد تأیید نامعتبر است';
        this.loading = false;
      }
    });
  }

  register(): void {
    if (!this.fullName) {
      this.error = 'نام و نام خانوادگی را وارد کنید';
      return;
    }

    this.loading = true;
    this.error = '';

    this.authService.register({ phoneNumber: this.phoneNumber, fullName: this.fullName }).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['']);
      },
      error: () => {
        this.error = 'خطا در ثبت‌نام';
        this.loading = false;
      }
    });
  }

  loginWithPassword(): void {
    if (!this.isValidPhone()) {
      this.error = 'شماره موبایل معتبر وارد کنید';
      return;
    }
    if (!this.password) {
      this.error = 'رمز عبور را وارد کنید';
      return;
    }

    this.loading = true;
    this.error = '';

    this.authService.loginWithPassword(this.phoneNumber, this.password).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['']);
      },
      error: () => {
        this.error = 'شماره موبایل یا رمز عبور نادرست است';
        this.loading = false;
      }
    });
  }
}
