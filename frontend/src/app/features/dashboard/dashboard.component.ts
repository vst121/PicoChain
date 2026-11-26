import { Component, signal } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `<main><h1>PicoChain Operations</h1><p class="muted">Logistics intelligence and controlled AI orchestration</p><nav><a href="/shipments">Shipments</a> · <a href="/ai-copilot">AI Copilot</a> · <a href="/approvals">Approvals</a> · <a href="/agent-control">Agent control</a> · <a href="/observability">Monitoring</a></nav><section class="cards"><article class="card"><strong>{{ delayedShipments() }}</strong><p class="muted">Delayed shipments</p></article><article class="card"><strong>{{ pendingApprovals() }}</strong><p class="muted">Pending approvals</p></article><article class="card"><strong>{{ activeAgents() }}</strong><p class="muted">Active agent tasks</p></article></section></main>`,
})
export class DashboardComponent {
  readonly delayedShipments = signal(0);
  readonly pendingApprovals = signal(0);
  readonly activeAgents = signal(0);

  constructor() {
    void this.load();
    const events = new EventSource(`${environment.apiBaseUrl.replace('/api/v1', '')}/api/v1/events`);
    events.addEventListener('shipment-delayed', () => void this.load());
    events.addEventListener('approval-updated', () => void this.load());
  }

  private async load(): Promise<void> {
    const [shipments, approvals, agents] = await Promise.all([
      fetch(`${environment.apiBaseUrl}/shipments?status=Delayed`).then(response => response.json()),
      fetch(`${environment.apiBaseUrl}/approvals/pending`).then(response => response.json()),
      fetch(`${environment.apiBaseUrl}/agent-tasks`).then(response => response.json()),
    ]);
    this.delayedShipments.set(Array.isArray(shipments) ? shipments.length : 0);
    this.pendingApprovals.set(Array.isArray(approvals) ? approvals.length : 0);
    this.activeAgents.set(Array.isArray(agents) ? agents.filter(task => task.status === 'Running').length : 0);
  }
}
