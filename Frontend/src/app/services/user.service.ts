import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';

export interface User {
  id: string;
  userName: string;
  email: string;
  fullName: string;
  phoneNumber: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = '/api/users';

  constructor(private http: HttpClient) {}

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>('/api/Users').pipe(
      catchError(err => {
        console.error('Lỗi lấy danh sách người dùng:', err);
        return throwError(() => err);
      })
    );
  }

  createUser(userData: any): Observable<any> {
    return this.http.post('/api/Users', userData);
  }

  updateUser(id: string, userData: any): Observable<any> {
    return this.http.put(`/api/Users/${id}`, userData);
  }

  deleteUser(id: string): Observable<any> {
    return this.http.delete(`/api/Users/${id}`);
  }
}
