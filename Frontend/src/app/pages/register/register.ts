import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink, CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register implements OnInit {
  registerData = {
    userName: '',
    email: '',
    password: '',
    fullName: '',
    phoneNumber: '',
    role: 'Customer'
  };
  isLoading = false;
  returnUrl: string = '/';

  constructor(
    private authService: AuthService, 
    private router: Router,
    private route: ActivatedRoute,
    private toastService: ToastService
  ) {}

  ngOnInit() {
    // Get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  navigateTo(url: string) {
    this.router.navigateByUrl(url);
  }

  onRegister(event: Event) {
    event.preventDefault();
    this.isLoading = true;
    
    this.authService.register(this.registerData).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success && response.token) {
          // AUTO-LOGIN: Save the user info immediately
          this.authService.saveUser(response.token, response.userName, response.role);
          
          // Professional Toast
          this.toastService.showSuccess('Đăng ký và Đăng nhập thành công! Chào mừng bạn gia nhập Vexere.');
          
          // Redirect to the previous page the user was trying to access
          this.router.navigateByUrl(this.returnUrl);
        } else {
          this.toastService.showError(response.message || 'Đăng ký thất bại');
        }
      },
      error: (error) => {
        this.isLoading = false;
        const errMsg = error.error?.message || 'Lỗi hệ thống khi đăng ký';
        this.toastService.showError(errMsg);
      }
    });
  }
}
