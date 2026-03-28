import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { from, Observable } from 'rxjs';

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
  saveUser(token: string, userName: string, userId: string, role: string) {
    localStorage.setItem('auth_token', token);
    localStorage.setItem('user_name', userName);
    localStorage.setItem('user_id', userId);
    localStorage.setItem('user_role', role);
  }

  // Get current userInfo
  getUser() {
    return {
      userName: localStorage.getItem('user_name'),
      id: localStorage.getItem('user_id'),
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
    localStorage.removeItem('user_name');
    localStorage.removeItem('user_id');
    localStorage.removeItem('user_role');
  }
}
