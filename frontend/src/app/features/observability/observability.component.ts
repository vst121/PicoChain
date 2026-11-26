import { Component, signal } from '@angular/core';
import { environment } from '../../../environments/environment';

type Health = { status: string; service: string };
type Summary = { shipmentCount: number; delayedShipmentCount: number; runningAgentTaskCount: number; pendingApprovalCount: number; pendingOutboxCount: number };

@Component({
  selector: 'app-observability',
  standalone: true,
  template: `<main><h1>System Monitoring</h1><p class="muted">Live service health and workload from the running platform.</p><section class="cards"><article class="card"><h2>Platform readiness</h2><strong>{{ readiness() }}</strong><p>PostgreSQL and RabbitMQ checks</p></article><article class="card"><h2>Backend API</h2><strong>{{ api()?.status ?? 'Unavailable' }}</strong><p>{{ api()?.service ?? 'picochain-api' }}</p></article><article class="card"><h2>AI Service</h2><strong>{{ ai()?.status ?? 'Unavailable' }}</strong><p>{{ ai()?.service ?? 'picochain-ai-service' }}</p></article><article class="card"><h2>Delayed shipments</h2><strong>{{ summary()?.delayedShipmentCount ?? '—' }}</strong><p>of {{ summary()?.shipmentCount ?? '—' }} shipments</p></article><article class="card"><h2>Running agent tasks</h2><strong>{{ summary()?.runningAgentTaskCount ?? '—' }}</strong><p>Currently executing</p></article><article class="card"><h2>Pending approvals</h2><strong>{{ summary()?.pendingApprovalCount ?? '—' }}</strong><p>Awaiting decision</p></article><article class="card"><h2>Outbox backlog</h2><strong>{{ summary()?.pendingOutboxCount ?? '—' }}</strong><p>Unpublished events</p></article></section></main>`,
})
export class ObservabilityComponent {
  readonly api = signal<Health | null>(null);
  readonly ai = signal<Health | null>(null);
  readonly readiness = signal('Unavailable');
  readonly summary = signal<Summary | null>(null);

  constructor() { void this.load(); }

  private async load(): Promise<void> {
    const [apiResponse, aiResponse, readinessResponse, summaryResponse] = await Promise.all([
      fetch(`${environment.apiBaseUrl.replace('/api/v1', '')}/health`),
      fetch(`${environment.apiBaseUrl.replace('/api/v1', '')}/health/ai`),
      fetch(`${environment.apiBaseUrl.replace('/api/v1', '')}/health/ready`),
      fetch(`${environment.apiBaseUrl}/monitoring/summary`),
    ]);
    if (apiResponse.ok) this.api.set(await apiResponse.json() as Health);
    if (aiResponse.ok) this.ai.set(await aiResponse.json() as Health);
    this.readiness.set(readinessResponse.ok ? 'Healthy' : 'Unavailable');
    if (summaryResponse.ok) this.summary.set(await summaryResponse.json() as Summary);
  }
}
