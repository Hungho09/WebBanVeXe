import {
  ChangeDetectorRef,
  Component,
  OnDestroy,
  OnInit,
  inject
} from '@angular/core';
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
  private readonly toastService = inject(ToastService);
  private readonly cdr = inject(ChangeDetectorRef);

  toasts: (ToastMessage & { id: number })[] = [];
  private subscription = new Subscription();
  private nextId = 0;
  private removeTimers = new Map<number, ReturnType<typeof setTimeout>>();

  ngOnInit() {
    this.subscription = this.toastService.toast$.subscribe((toast) => {
      const id = this.nextId++;
      this.toasts = [...this.toasts, { ...toast, id }];
      this.cdr.detectChanges();

      const timer = setTimeout(() => {
        this.removeTimers.delete(id);
        this.removeToast(id);
      }, 4000);
      this.removeTimers.set(id, timer);
    });
  }

  removeToast(id: number) {
    const t = this.removeTimers.get(id);
    if (t !== undefined) {
      clearTimeout(t);
      this.removeTimers.delete(id);
    }
    this.toasts = this.toasts.filter((x) => x.id !== id);
    this.cdr.detectChanges();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    for (const t of this.removeTimers.values()) {
      clearTimeout(t);
    }
    this.removeTimers.clear();
  }
}
