import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-ai-copilot',
  standalone: true,
  imports: [FormsModule],
  template: `<main><h1>AI Copilot</h1><p class="muted">Investigations are executed by the AI service through the governed backend gateway.</p><label>Shipment ID <input [(ngModel)]="shipmentId" placeholder="00000000-0000-0000-0000-000000000000"></label><button (click)="investigate()" [disabled]="busy()">{{ busy() ? 'Investigating…' : 'Investigate delay' }}</button>@if (error()) {<p class="error">{{ error() }}</p>} @if (result()) {<article><h2>{{ result().probable_cause }}</h2><p>{{ result().decision_summary }}</p><h3>Evidence</h3><ul>@for (item of result().evidence; track item) {<li>{{ item }}</li>}</ul><p><strong>Recommendation:</strong> {{ result().recommended_action }}</p></article>}</main>`,
  styles: ['label{display:block;margin:1rem 0}input{display:block;width:28rem;max-width:100%;padding:.6rem}button{padding:.6rem 1rem}.error{color:#b42318}article{background:#fff;padding:1rem;margin-top:1rem;border-radius:10px}'],
})
export class AiCopilotComponent {
  shipmentId = '';
  readonly busy = signal(false);
  readonly error = signal('');
  readonly result = signal<any>(null);

  async investigate(): Promise<void> {
    this.busy.set(true); this.error.set(''); this.result.set(null);
    try {
      const response = await fetch(`${environment.apiBaseUrl}/copilot/investigate`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ shipmentId: this.shipmentId, correlationId: crypto.randomUUID() }) });
      const body = await response.json();
      if (!response.ok) throw new Error(body.detail ?? 'Investigation failed');
      this.result.set(body);
    } catch (error) { this.error.set(error instanceof Error ? error.message : 'Investigation failed'); }
    finally { this.busy.set(false); }
  }
}
