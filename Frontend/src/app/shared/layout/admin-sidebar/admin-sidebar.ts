import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './admin-sidebar.html'
})
export class AdminSidebarComponent {
  @Input() isAdmin: boolean = false;

  constructor(private router: Router, private authService: AuthService) {}

  navigateTo(url: string) {
    this.router.navigateByUrl(url);
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
