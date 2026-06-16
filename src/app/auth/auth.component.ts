import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent {
  step: 'phone' | 'otp' | 'register' = 'phone';
  phoneNumber: string = '';
  otpCode: string = '';
  fullName: string = '';
  loading: boolean = false;
  error: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  sendOtp(): void {
    if (!this.phoneNumber || this.phoneNumber.length !== 11) {
      this.error = 'شماره موبایل معتبر وارد کنید';
      return;
    }
    
    this.loading = true;
    this.error = '';
    
    this.authService.sendOtp(this.phoneNumber).subscribe({
      next: () => {
        this.step = 'otp';
        this.loading = false;
      },
      error: (err: any) => {
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
      next: () => {
        this.router.navigate(['']);
      },
      error: (err: any) => {
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
    
    this.authService.register(this.phoneNumber, this.fullName).subscribe({
      next: () => {
        this.step = 'otp';
        this.loading = false;
      },
      error: (err: any) => {
        this.error = 'خطا در ثبت‌نام';
        this.loading = false;
      }
    });
  }
}
