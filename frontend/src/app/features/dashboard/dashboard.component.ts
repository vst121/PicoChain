import { Component, signal } from '@angular/core';
import { ApiClientService } from '../../core/api-client.service';
import { RealtimeService } from '../../core/realtime.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `<main><h1>PicoChain Operations</h1><p class="muted">Logistics intelligence and controlled AI orchestration</p><nav><a href="/orders">Orders</a> · <a href="/shipments">Shipments</a> · <a href="/ai-copilot">AI Copilot</a> · <a href="/approvals">Approvals</a> · <a href="/agent-control">Agent control</a> · <a href="/observability">Monitoring</a> · <a href="/audit">Audit</a> · <a href="/evaluations">Evaluations</a></nav><section class="cards"><article class="card"><strong>{{ delayedShipments() }}</strong><p class="muted">Delayed shipments</p></article><article class="card"><strong>{{ pendingApprovals() }}</strong><p class="muted">Pending approvals</p></article><article class="card"><strong>{{ activeAgents() }}</strong><p class="muted">Active agent tasks</p></article></section></main>`,
})
export class DashboardComponent {
  readonly delayedShipments = signal(0);
  readonly pendingApprovals = signal(0);
  readonly activeAgents = signal(0);

  constructor(private readonly api: ApiClientService, realtime: RealtimeService) {
    void this.load();
    realtime.connect({
      'shipment-delayed': () => void this.load(),
      'approval-updated': () => void this.load(),
      'action-executed': () => void this.load(),
    });
  }

  private async load(): Promise<void> {
    const [shipments, approvals, agents] = await Promise.all([
      this.api.get<any[]>('/shipments?status=Delayed'),
      this.api.get<any[]>('/approvals/pending'),
      this.api.get<any[]>('/agent-tasks'),
    ]);
    this.delayedShipments.set(Array.isArray(shipments) ? shipments.length : 0);
    this.pendingApprovals.set(Array.isArray(approvals) ? approvals.length : 0);
    this.activeAgents.set(Array.isArray(agents) ? agents.filter(task => task.status === 'Running').length : 0);
  }
}
