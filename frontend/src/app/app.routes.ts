import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ShipmentsComponent } from './features/shipments/shipments.component';

export const routes: Routes = [{ path: '', component: DashboardComponent }, { path: 'shipments', component: ShipmentsComponent }, { path: '**', redirectTo: '' }];
