import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ShipmentsComponent } from './features/shipments/shipments.component';
import { ApprovalsComponent } from './features/approvals/approvals.component';
import { AgentControlComponent } from './features/agent-control/agent-control.component';
import { AiCopilotComponent } from './features/ai-copilot/ai-copilot.component';
import { ObservabilityComponent } from './features/observability/observability.component';
import { AuditComponent } from './features/audit/audit.component';
import { OrdersComponent } from './features/orders/orders.component';
import { EvaluationsComponent } from './features/evaluations/evaluations.component';
import { LoginComponent } from './features/auth/login.component';
import { authGuard } from './core/auth.guard';
import { managerGuard } from './core/manager.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: '', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'shipments', component: ShipmentsComponent, canActivate: [authGuard] },
  { path: 'orders', component: OrdersComponent, canActivate: [authGuard] },
  { path: 'approvals', component: ApprovalsComponent, canActivate: [authGuard, managerGuard] },
  { path: 'agent-control', component: AgentControlComponent, canActivate: [authGuard] },
  { path: 'ai-copilot', component: AiCopilotComponent, canActivate: [authGuard] },
  { path: 'observability', component: ObservabilityComponent, canActivate: [authGuard] },
  { path: 'audit', component: AuditComponent, canActivate: [authGuard] },
  { path: 'evaluations', component: EvaluationsComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: '' },
];
