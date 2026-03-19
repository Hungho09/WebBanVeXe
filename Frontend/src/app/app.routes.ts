import { Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Homepage } from './pages/homepage/homepage';
import { ResourceManagement } from './pages/resource-management/resource-management';
import { TripManagement } from './pages/trip-management/trip-management';
import { UserManagement } from './pages/user-management/user-management';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { 
    path: 'resource', 
    component: ResourceManagement, 
    canActivate: [AuthGuard], 
    data: { roles: ['Admin', 'Employee'] } 
  },
  { 
    path: 'trip', 
    component: TripManagement, 
    canActivate: [AuthGuard], 
    data: { roles: ['Admin', 'Employee'] } 
  },
  { 
    path: 'user-management', 
    component: UserManagement, 
    canActivate: [AuthGuard], 
    data: { roles: ['Admin'] } 
  },
  { path: '', redirectTo: 'homepage', pathMatch: 'full' },
  { path: 'homepage', component: Homepage },
  { path: 'dashboard', component: Dashboard },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: '**', redirectTo: 'dashboard' }
];

