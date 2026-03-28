import { Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Homepage } from './pages/homepage/homepage';
import { RouteManagement } from './pages/route-management/route-management';
import { BusManagement } from './pages/bus-management/bus-management';
import { TripManagement } from './pages/trip-management/trip-management';
import { UserManagement } from './pages/user-management/user-management';
import { AdminPoiComponent } from './pages/admin-poi/admin-poi';
import { CmsManagement } from './pages/cms-management/cms-management';
import { BookingManagement } from './pages/booking-management/booking-management';
import { AdminLayout } from './shared/layout/admin-layout/admin-layout';
import { AuthGuard } from './guards/auth.guard';
import { PaymentComponent } from './pages/payment/payment.component';
import { BookingSearch } from './pages/booking-search/booking-search';
import { Booking } from './pages/booking/booking';
import { MyBookingsComponent } from './pages/my-bookings/my-bookings';
import { AdminCancelManagementComponent } from './pages/admin-cancel-management/admin-cancel-management';

export const routes: Routes = [
  { path: '', redirectTo: 'homepage', pathMatch: 'full' },
  { path: 'homepage', component: Homepage },
  { path: 'search-results', component: BookingSearch },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'payment', component: PaymentComponent },
  { path: 'invoice-pdf/:id', loadComponent: () => import('./pages/invoice-pdf-page/invoice-pdf-page.component').then(m => m.InvoicePdfPageComponent) },
  { path: 'invoices/:id', loadComponent: () => import('./pages/invoice-detail/invoice-detail.component').then(m => m.InvoiceDetailComponent) },
  {
    path: 'admin',
    component: AdminLayout,
    canActivate: [AuthGuard],
    data: { roles: ['Admin', 'Employee'] },
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: Dashboard },
      { path: 'booking-management', component: BookingManagement },
      { path: 'user-management', component: UserManagement, data: { roles: ['Admin'] } },
      { path: 'poi', component: AdminPoiComponent, data: { roles: ['Admin', 'Employee'] } },
      { path: 'bus', component: BusManagement },
      { path: 'route', component: RouteManagement },
      { path: 'trip', component: TripManagement },
      { path: 'booking-cancel', component: AdminCancelManagementComponent, data: { roles: ['Admin'] } },
      { path: 'cms-management', component: CmsManagement, data: { roles: ['Admin'] } },
    ]
  },
  { path: 'booking/:id', loadComponent: () => import('./pages/booking/booking').then(m => m.Booking) },
  { path: 'my-bookings', component: MyBookingsComponent },
  { path: '**', redirectTo: 'homepage' }
];
