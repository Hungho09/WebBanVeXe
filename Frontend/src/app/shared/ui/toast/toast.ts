import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService, ToastMessage } from '../../../services/toast.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './toast.html',
  styleUrl: './toast.css'
})
export class ToastComponent implements OnInit, OnDestroy {
  toasts: (ToastMessage & { id: number })[] = [];
  private subscription: Subscription = new Subscription();
  private nextId = 0;

  constructor(private toastService: ToastService) {}

  ngOnInit() {
    this.subscription = this.toastService.toast$.subscribe(toast => {
      const id = this.nextId++;
      this.toasts.push({ ...toast, id });
      
      // Auto-remove after 4 seconds
      setTimeout(() => {
        this.removeToast(id);
      }, 4000);
    });
  }

  removeToast(id: number) {
    this.toasts = this.toasts.filter(t => t.id !== id);
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
