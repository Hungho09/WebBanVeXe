import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  currentDate = new Date();
  isAdmin = false;

  constructor(
    private authService: AuthService,
  ) {}

  ngOnInit() {
    this.isAdmin = this.authService.getUser().role === 'Admin';
  }
}

