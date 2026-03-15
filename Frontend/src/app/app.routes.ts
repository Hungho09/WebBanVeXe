
import { Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Homepage } from './pages/homepage/homepage';
// Thử bỏ chữ '.component' nếu file trong thư mục của bạn chỉ là resource-management.ts
import { ResourceManagement } from './pages/resource-management/resource-management';
import { TripManagement } from './pages/trip-management/trip-management';


export const routes: Routes = [
  { path: 'resource', component: ResourceManagement },
  { path: 'trip', component: TripManagement },
  { path: '', redirectTo: 'homepage', pathMatch: 'full' },
  { path: 'homepage', component: Homepage },
  { path: 'dashboard', component: Dashboard },
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    { path: '**', redirectTo: 'dashboard' }
];

