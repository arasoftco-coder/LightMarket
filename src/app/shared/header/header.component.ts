import { Component, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnDestroy {
  isLoggedIn = false;
  cartCount = 0;
  private authSub: Subscription;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    this.authSub = this.authService.isLoggedIn$.subscribe(loggedIn => this.isLoggedIn = loggedIn);
  }

  ngOnDestroy(): void {
    this.authSub.unsubscribe();
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }

  logout(): void {
    this.authService.logout();
    this.isLoggedIn = false;
    this.router.navigate(['']);
  }
}
