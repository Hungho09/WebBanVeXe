import { Routes } from '@angular/router';
import { Dashboard } from './pages/management/dashboard/dashboard';
import { Login } from './pages/auth/login/login';
import { Register } from './pages/auth/register/register';
import { Homepage } from './pages/home/homepage';
import { RouteManagement } from './pages/management/routes/route-management';
import { BusManagement } from './pages/management/buses/bus-management';
import { TripManagement } from './pages/management/trips/trip-management';
import { UserManagement } from './pages/management/users/user-management';
import { AdminPoiComponent } from './pages/management/poi/admin-poi';
import { CmsManagement } from './pages/management/cms/cms-management';
import { BookingManagement } from './pages/management/bookings/booking-management';
import { AdminLayout } from './shared/layout/admin-layout/admin-layout';
import { AuthGuard } from './guards/auth.guard';
import { PaymentComponent } from './pages/booking/payment/payment.component';
import { BookingSearch } from './pages/booking/search/booking-search';
import { Booking } from './pages/booking/detail/booking';
import { MyBookingsComponent } from './pages/booking/user-bookings/my-bookings';
import { AdminCancelManagementComponent } from './pages/management/cancellations/admin-cancel-management';

export const routes: Routes = [
  { path: '', redirectTo: 'homepage', pathMatch: 'full' },
  { path: 'homepage', component: Homepage },
  { path: 'search-results', component: BookingSearch },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'payment', component: PaymentComponent },
  { path: 'invoice-pdf/:id', loadComponent: () => import('./pages/invoices/pdf/invoice-pdf-page.component').then(m => m.InvoicePdfPageComponent) },
  { path: 'invoices/:id', loadComponent: () => import('./pages/invoices/detail/invoice-detail.component').then(m => m.InvoiceDetailComponent) },
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
  { path: 'booking/:id', loadComponent: () => import('./pages/booking/detail/booking').then(m => m.Booking) },
  { path: 'my-bookings', component: MyBookingsComponent },
  { path: '**', redirectTo: 'homepage' }
];
