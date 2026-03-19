import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink, CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  registerData = {
    userName: '',
    email: '',
    password: '',
    fullName: '',
    phoneNumber: '',
    role: 'Customer' // Default role
  };
  isLoading = false;

  constructor(private authService: AuthService, private router: Router) {}

  onRegister(event: Event) {
    event.preventDefault();
    this.isLoading = true;
    
    console.log('Registering user:', this.registerData);
    
    this.authService.register(this.registerData).subscribe({
      next: (response) => {
        this.isLoading = false;
        alert('Đăng ký thành công! Vui lòng đăng nhập.');
        this.router.navigate(['/login']);
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Registration failed:', error);
        alert('Đăng ký thất bại: ' + (error.error?.message || 'Lỗi hệ thống'));
      }
    });
  }
}
