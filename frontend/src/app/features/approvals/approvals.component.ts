import { Component, signal } from '@angular/core';
import { environment } from '../../../environments/environment';

type Approval = { approvalRequestId: string; actionType?: string; status?: string; evidenceSummary?: string; parametersJson?: string };

@Component({
  selector: 'app-approvals',
  standalone: true,
  template: `<main><h1>Approval Center</h1><p class="muted">Business-critical actions require backend-enforced approval.</p>@if (approvals().length === 0) {<p>No pending approvals.</p>} @for (approval of approvals(); track approval.approvalRequestId) {<article class="approval"><h2>{{ approval.actionType ?? 'Proposed action' }}</h2><p>{{ approval.evidenceSummary ?? 'Review the investigation evidence before deciding.' }}</p><details><summary>Parameters</summary><pre>{{ approval.parametersJson }}</pre></details><button (click)="decide(approval.approvalRequestId, 'approve')">Approve</button><button (click)="decide(approval.approvalRequestId, 'reject')">Reject</button></article>}</main>`,
  styles: ['.approval{background:#fff;padding:1rem;margin:1rem 0;border-radius:10px}button{margin-right:.5rem;padding:.5rem 1rem}'],
})
export class ApprovalsComponent {
  readonly approvals = signal<Approval[]>([]);

  constructor() { void this.load(); }

  private async load(): Promise<void> {
    const response = await fetch(`${environment.apiBaseUrl}/approvals/pending`);
    if (response.ok) this.approvals.set(await response.json() as Approval[]);
  }

  async decide(id: string, decision: 'approve' | 'reject'): Promise<void> {
    const response = await fetch(`${environment.apiBaseUrl}/approvals/${id}/${decision}`, { method: 'POST' });
    if (response.ok) this.approvals.update(items => items.filter(item => item.approvalRequestId !== id));
  }
}
