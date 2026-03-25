import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = '/api/auth'; // Using proxy configured earlier

  constructor(private http: HttpClient) {}

  register(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data);
  }

  login(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, data);
  }

  // Save token and user info to localStorage
  saveUser(token: string, userData: any) {
    localStorage.setItem('auth_token', token);
    localStorage.setItem('user_id', userData.id);
    localStorage.setItem('user_name', userData.userName);
    localStorage.setItem('user_full_name', userData.fullName || '');
    localStorage.setItem('user_email', userData.email || '');
    localStorage.setItem('user_phone', userData.phoneNumber || '');
    localStorage.setItem('user_role', userData.role);
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
    localStorage.clear(); // Safe for this app since we only use it for auth/cms
  }
}
