import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserService, User } from '../../services/user.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule], // Thêm RouterLink
  templateUrl: './user-management.html',
  styleUrl: './user-management.css'
})
export class UserManagement implements OnInit {
  users: User[] = [];
  isLoading = false;
  showModal = false;
  isEditMode = false;
  
  selectedUser: any = {
    userName: '',
    email: '',
    password: '',
    fullName: '',
    phoneNumber: '',
    role: 'Customer',
    isActive: true,
    newPassword: ''
  };

  constructor(
    private userService: UserService,
    private toastService: ToastService
  ) {}


  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.isLoading = true;
    this.userService.getUsers().subscribe({
      next: (data: User[]) => {
        this.users = data;
        this.isLoading = false;
      },
      error: (err: any) => {
        this.toastService.showError('Không thể tải danh sách người dùng');
        this.isLoading = false;
      }
    });
  }

  openAddModal() {
    this.isEditMode = false;
    this.selectedUser = {
      userName: '',
      email: '',
      password: '',
      fullName: '',
      phoneNumber: '',
      role: 'Customer',
      isActive: true
    };
    this.showModal = true;
  }

  openEditModal(user: User) {
    this.isEditMode = true;
    this.selectedUser = { 
      ...user,
      newPassword: ''
    };
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  saveUser() {
    if (this.isEditMode) {
      this.userService.updateUser(this.selectedUser.id, this.selectedUser).subscribe({
        next: (res: any) => {
          this.toastService.showSuccess('Cập nhật người dùng thành công');
          this.loadUsers();
          this.closeModal();
        },
        error: (err: any) => {
          this.toastService.showError(err.error?.message || 'Lỗi khi cập nhật');
        }
      });
    } else {
      this.userService.createUser(this.selectedUser).subscribe({
        next: (res: any) => {
          this.toastService.showSuccess('Tạo người dùng thành công');
          this.loadUsers();
          this.closeModal();
        },
        error: (err: any) => {
          this.toastService.showError(err.error?.message || 'Lỗi khi tạo mới');
        }
      });
    }
  }

  deleteUser(id: string) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
      this.userService.deleteUser(id).subscribe({
        next: (res: any) => {
          this.toastService.showSuccess('Xóa người dùng thành công');
          this.loadUsers();
        },
        error: (err: any) => {
          this.toastService.showError('Lỗi khi xóa người dùng');
        }
      });
    }
  }
}
