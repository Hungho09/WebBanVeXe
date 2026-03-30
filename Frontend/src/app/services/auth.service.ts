import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { from, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = '/api/auth'; // Using proxy configured earlier

  constructor(private http: HttpClient) { }

  register(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data);
  }

  login(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, data);
  }

  // Save token and user info to localStorage
  saveUser(token: string, user: any) {
    localStorage.setItem('auth_token', token);
    localStorage.setItem('user_id', user.id || user.Id || '');
    localStorage.setItem('user_name', user.userName || user.UserName || '');
    localStorage.setItem('user_full_name', user.fullName || user.FullName || '');
    localStorage.setItem('user_email', user.email || user.Email || '');
    localStorage.setItem('user_phone', user.phoneNumber || user.PhoneNumber || '');
    localStorage.setItem('user_role', user.role || user.Role || 'Customer');
  }

  // Get current userInfo
  getUser() {
    return {
      id: localStorage.getItem('user_id'),
      userName: localStorage.getItem('user_name'),
      fullName: localStorage.getItem('user_full_name'),
      email: localStorage.getItem('user_email'),
      phoneNumber: localStorage.getItem('user_phone'),
      role: localStorage.getItem('user_role')
    };
  }

  // Check login
  isLoggedIn() {
    return !!localStorage.getItem('auth_token');
  }

  // Logout
  logout() {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('user_id');
    localStorage.removeItem('user_name');
    localStorage.removeItem('user_full_name');
    localStorage.removeItem('user_email');
    localStorage.removeItem('user_phone');
    localStorage.removeItem('user_role');
    localStorage.removeItem('user_full_name');
    localStorage.removeItem('user_email');
    localStorage.removeItem('user_phone');
  }
}
