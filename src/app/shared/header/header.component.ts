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
  isAdmin = false;
  cartCount = 0;
  private authSub: Subscription;
  private userSub: Subscription;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    this.authSub = this.authService.isLoggedIn$.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
      if (!loggedIn) {
        this.isAdmin = false;
      }
    });
    this.userSub = this.authService.currentUser$.subscribe(user => {
      this.isAdmin = user?.role === 'Admin';
    });
  }

  ngOnDestroy(): void {
    this.authSub.unsubscribe();
    this.userSub.unsubscribe();
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
