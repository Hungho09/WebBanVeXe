import { Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Homepage } from './pages/homepage/homepage';
import { RouteManagement } from './pages/route-management/route-management';
import { BusManagement } from './pages/bus-management/bus-management';
import { TripManagement } from './pages/trip-management/trip-management';
import { UserManagement } from './pages/user-management/user-management';
import { CmsManagement } from './pages/cms-management/cms-management';
import { AdminLayout } from './shared/layout/admin-layout/admin-layout';
import { AuthGuard } from './guards/auth.guard';
import { PaymentComponent } from './pages/payment/payment.component';
import { Booking } from './pages/booking/booking';

export const routes: Routes = [
  { path: '', redirectTo: 'homepage', pathMatch: 'full' },
  { path: 'homepage', component: Homepage },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'payment', component: PaymentComponent },
  {
    path: 'admin',
    component: AdminLayout,
    canActivate: [AuthGuard],
    data: { roles: ['Admin', 'Employee'] },
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: Dashboard },
      { path: 'user-management', component: UserManagement, data: { roles: ['Admin'] } },
      { path: 'bus', component: BusManagement },
      { path: 'route', component: RouteManagement },
      { path: 'trip', component: TripManagement },
      { path: 'cms-management', component: CmsManagement, data: { roles: ['Admin'] } },
    ]
  },
  { path: 'booking/:id', loadComponent: () => import('./pages/booking/booking').then(m => m.Booking) },
  { path: '**', redirectTo: 'homepage' }
];
