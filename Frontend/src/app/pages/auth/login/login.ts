import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login implements OnInit {
  loginData = {
    userName: '',
    password: ''
  };
  isLoading = false;
  returnUrl: string = '/';

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private toastService: ToastService
  ) { }

  ngOnInit() {
    // Get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  navigateTo(url: string) {
    this.router.navigateByUrl(url);
  }

  onLogin(event: Event) {
    event.preventDefault();
    this.isLoading = true;


    this.authService.login(this.loginData).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success && response.token) {
          const userData = {
            id: response.userId || response.UserId || response.id,
            userName: response.userName || response.UserName,
            fullName: response.fullName || response.FullName,
            email: response.email || response.Email,
            phoneNumber: response.phoneNumber || response.PhoneNumber,
            role: response.role || response.Role || 'Customer'
          };
          
          this.authService.saveUser(response.token, userData);
          this.toastService.showSuccess(`Chào mừng trở lại, ${userData.userName || 'Thành viên'}!`);
          this.router.navigateByUrl(this.returnUrl);
        } else {
          this.toastService.showError(response.message || 'Đăng nhập thất bại');
        }
      },
      error: (error) => {
        this.isLoading = false;
        const errMsg = error.error?.message || 'Email hoặc mật khẩu không đúng';
        this.toastService.showError(errMsg);
      }
    });
  }
}
