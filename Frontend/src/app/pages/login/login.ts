import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginData = {
    email: '',
    password: ''
  };
  isLoading = false;

  constructor(private authService: AuthService, private router: Router) {}

  onLogin(event: Event) {
    event.preventDefault();
    this.isLoading = true;
    
    this.authService.login(this.loginData).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success && response.token) {
          this.authService.saveUser(response.token, response.userName, response.role);
          alert('Đăng nhập thành công!');
          this.router.navigate(['/']); // Redirect to home
        } else {
          alert('Đăng nhập thất bại: ' + response.message);
        }
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Login error:', error);
        alert('Lỗi: ' + (error.error?.message || 'Không thể đăng nhập'));
      }
    });
  }
}
