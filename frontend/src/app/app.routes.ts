import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ShipmentsComponent } from './features/shipments/shipments.component';
import { ApprovalsComponent } from './features/approvals/approvals.component';
import { AgentControlComponent } from './features/agent-control/agent-control.component';
import { AiCopilotComponent } from './features/ai-copilot/ai-copilot.component';
import { ObservabilityComponent } from './features/observability/observability.component';
import { AuditComponent } from './features/audit/audit.component';

export const routes: Routes = [{ path: '', component: DashboardComponent }, { path: 'shipments', component: ShipmentsComponent }, { path: 'approvals', component: ApprovalsComponent }, { path: 'agent-control', component: AgentControlComponent }, { path: 'ai-copilot', component: AiCopilotComponent }, { path: 'observability', component: ObservabilityComponent }, { path: 'audit', component: AuditComponent }, { path: '**', redirectTo: '' }];
