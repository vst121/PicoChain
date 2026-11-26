import { Component, signal } from '@angular/core';
import { environment } from '../../../environments/environment';

type Health = { status: string; service: string };

@Component({
  selector: 'app-observability',
  standalone: true,
  template: `<main><h1>System Monitoring</h1><p class="muted">Live service health from the running platform.</p><section class="cards"><article class="card"><h2>Backend API</h2><strong>{{ api()?.status ?? 'Unavailable' }}</strong><p>{{ api()?.service ?? 'picochain-api' }}</p></article><article class="card"><h2>AI Service</h2><strong>{{ ai()?.status ?? 'Unavailable' }}</strong><p>{{ ai()?.service ?? 'picochain-ai-service' }}</p></article></section></main>`,
})
export class ObservabilityComponent {
  readonly api = signal<Health | null>(null);
  readonly ai = signal<Health | null>(null);

  constructor() { void this.load(); }

  private async load(): Promise<void> {
    const [apiResponse, aiResponse] = await Promise.all([
      fetch(`${environment.apiBaseUrl.replace('/api/v1', '')}/health`),
      fetch(`${environment.apiBaseUrl.replace('/api/v1', '')}/health/ai`),
    ]);
    if (apiResponse.ok) this.api.set(await apiResponse.json() as Health);
    if (aiResponse.ok) this.ai.set(await aiResponse.json() as Health);
  }
}
