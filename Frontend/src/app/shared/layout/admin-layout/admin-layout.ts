import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterOutlet, NavigationEnd, RouterLinkActive } from '@angular/router';
import { filter } from 'rxjs/operators';
import { AuthService } from '../../../services/auth.service';
import { AdminSidebarComponent } from '../admin-sidebar/admin-sidebar';
import { AdminTopnavComponent } from '../admin-topnav/admin-topnav';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLinkActive, AdminSidebarComponent, AdminTopnavComponent],
  templateUrl: './admin-layout.html',
  styleUrls: ['./admin-layout.css']
})
export class AdminLayout {
  currentUrl: string = '';
  isAdmin: boolean = false;

  constructor(private router: Router, private authService: AuthService) {
    this.isAdmin = this.authService.getUser().role === 'Admin';
    this.currentUrl = this.router.url;
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.currentUrl = event.urlAfterRedirects;
    });
  }

  navigateTo(url: string) {
    this.router.navigateByUrl(url);
  }

  isActive(url: string): boolean {
    return this.currentUrl === url || this.currentUrl.startsWith(url + '/');
  }
}
