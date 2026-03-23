import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  currentDate = new Date();

  constructor(private router: Router) {}

  navigateTo(url: string) {
    this.router.navigateByUrl(url);
  }
}
