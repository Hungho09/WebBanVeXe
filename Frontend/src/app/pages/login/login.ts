import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, CommonModule, FormsModule],
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

    // Bypass logic cho admin (không cần backend)
    if (this.loginData.userName === 'admin' && this.loginData.password === '123456') {
      this.isLoading = false;
      // Tạo một JWT giả (Header.Payload.Signature) với exp rất xa
      const dummyToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiQWRtaW4iLCJuYW1lIjoiQWRtaW4iLCJleHAiOjk5OTk5OTk5OTl9.dummy';
      this.authService.saveUser(dummyToken, 'Admin Local', 'admin-id-123', 'Admin');
      this.toastService.showSuccess('Đăng nhập thành công (Bypass Backend)!');
      this.router.navigateByUrl('/admin/dashboard');
      return;
    }

    this.authService.login(this.loginData).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success && response.token) {
          const uName = response.userName || response.UserName || 'Thành viên';
          const uRole = response.role || response.Role || 'Customer';
          const uId = response.userId || response.UserId || response.id || response.Id || '';
          
          this.authService.saveUser(response.token, uName, uId, uRole);
          this.toastService.showSuccess(`Chào mừng trở lại, ${uName}!`);
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
