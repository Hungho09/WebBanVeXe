import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
    constructor(
        private router: Router,
        private authService: AuthService,
        private toastService: ToastService
    ) {}

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (this.authService.isLoggedIn()) {
            // Check if route has restricted roles
            const expectedRoles = route.data['roles'] as Array<string>;
            if (expectedRoles) {
                const user = this.authService.getUser();
                if (!user || !user.role || !expectedRoles.includes(user.role as string)) {
                    this.toastService.showError('Bạn không có quyền truy cập vào chức năng này!');
                    this.router.navigate(['/']);
                    return false;
                }
            }
            return true;
        }

        // Not logged in so redirect to login page with the return url
        this.toastService.showWarning('Vui lòng đăng nhập để tiếp tục.');
        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url }});
        return false;
    }
}
