import { Component, computed, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../environments/environment';

type AuditItem = { id: string; action: string; entityType: string; actor?: string; correlationId?: string; occurredAt: string; detailsJson?: string };

@Component({
  selector: 'app-audit',
  standalone: true,
  imports: [FormsModule, DatePipe],
  template: `<main><h1>Trace Explorer</h1><p class="muted">Follow business and agent execution steps by correlation ID.</p><input class="filter" [(ngModel)]="correlationFilter" placeholder="Filter by correlation ID"><section class="trace-list">@for (item of filtered(); track item.id) {<article class="trace-step"><div class="marker"></div><div><div class="step-heading"><strong>{{ item.action }}</strong><span>{{ item.occurredAt | date:'medium' }}</span></div><p>{{ item.entityType }} · {{ item.actor ?? 'system' }}</p><code>{{ item.correlationId ?? 'No correlation ID' }}</code>@if (item.detailsJson) {<details><summary>Details</summary><pre>{{ item.detailsJson }}</pre></details>}</div></article>} @empty {<p>No trace records match the filter.</p>}</section></main>`,
  styles: ['.filter{width:100%;padding:.7rem;border:1px solid #ccd6e4;border-radius:.45rem;margin:1rem 0}.trace-list{background:#fff;border-radius:.8rem;padding:1.2rem 1.5rem}.trace-step{display:flex;gap:1rem;border-bottom:1px solid #edf0f5;padding:1rem 0}.trace-step:last-child{border-bottom:0}.marker{width:.7rem;height:.7rem;background:#4b83f0;border-radius:50%;margin-top:.3rem;flex-shrink:0}.step-heading{display:flex;gap:1rem;align-items:center}.step-heading span{color:#7b879a;font-size:.75rem}.trace-step p{margin:.35rem 0;color:#63718a}.trace-step code{font-size:.72rem;color:#536985}pre{white-space:pre-wrap}'],
})
export class AuditComponent {
  readonly records = signal<AuditItem[]>([]);
  correlationFilter = '';
  readonly filtered = computed(() => this.records().filter(item => (item.correlationId ?? '').toLowerCase().includes(this.correlationFilter.toLowerCase())));

  constructor() { void this.load(); }

  private async load(): Promise<void> {
    const response = await fetch(`${environment.apiBaseUrl}/audit?limit=100`);
    if (response.ok) this.records.set(await response.json() as AuditItem[]);
  }
}
