import { Component, signal } from '@angular/core';
import { environment } from '../../../environments/environment';

type AuditItem = { id: string; action: string; entityType: string; actor?: string; correlationId?: string; occurredAt: string; detailsJson?: string };

@Component({
  selector: 'app-audit',
  standalone: true,
  template: `<main><h1>Audit History</h1><p class="muted">Recorded business actions, agent lifecycle changes, and approvals.</p><table><thead><tr><th>Time</th><th>Action</th><th>Entity</th><th>Actor</th><th>Correlation</th></tr></thead><tbody>@for (item of records(); track item.id) {<tr><td>{{ item.occurredAt }}</td><td>{{ item.action }}</td><td>{{ item.entityType }}</td><td>{{ item.actor ?? '—' }}</td><td>{{ item.correlationId ?? '—' }}</td></tr>}</tbody></table></main>`,
  styles: ['table{width:100%;background:#fff;border-collapse:collapse}th,td{text-align:left;padding:.8rem;border-bottom:1px solid #e5e7eb}'],
})
export class AuditComponent {
  readonly records = signal<AuditItem[]>([]);

  constructor() { void this.load(); }

  private async load(): Promise<void> {
    const response = await fetch(`${environment.apiBaseUrl}/audit?limit=100`);
    if (response.ok) this.records.set(await response.json() as AuditItem[]);
  }
}
