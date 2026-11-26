import { Component, signal } from '@angular/core';
import { environment } from '../../../environments/environment';

type Evaluation = { id: string; scenarioId: string; evaluatorType: string; overallScore: number; passed: boolean; createdAt: string; agentVersion?: string; promptVersion?: string; modelIdentifier?: string; estimatedCost?: number };

@Component({
  selector: 'app-evaluations',
  standalone: true,
  template: `<main><h1>AI Evaluations</h1><p class="muted">Persisted quality measurements for agent executions.</p><table><thead><tr><th>Scenario</th><th>Evaluator</th><th>Score</th><th>Result</th><th>Version</th><th>Cost</th><th>Review</th></tr></thead><tbody>@for (item of evaluations(); track item.id) {<tr><td>{{ item.scenarioId }}</td><td>{{ item.evaluatorType }}</td><td>{{ item.overallScore }}</td><td>{{ item.passed ? 'Passed' : 'Failed' }}</td><td>{{ item.agentVersion ?? 'n/a' }} / {{ item.promptVersion ?? 'n/a' }}<br>{{ item.modelIdentifier ?? 'n/a' }}</td><td>{{ item.estimatedCost ?? 0 }}</td><td><button (click)="review(item.id)">Review</button></td></tr>}</tbody></table></main>`,
  styles: ['table{width:100%;background:#fff;border-collapse:collapse}th,td{text-align:left;padding:.8rem;border-bottom:1px solid #e5e7eb}'],
})
export class EvaluationsComponent {
  readonly evaluations = signal<Evaluation[]>([]);

  constructor() { void this.load(); }

  private async load(): Promise<void> {
    const response = await fetch(`${environment.apiBaseUrl}/evaluations`);
    if (response.ok) this.evaluations.set(await response.json() as Evaluation[]);
  }

  async review(evaluationId: string): Promise<void> {
    const notes = window.prompt('Review notes:', '') ?? '';
    const decision = window.prompt('Decision (Approved, Rejected, NeedsRevision):', 'Approved') ?? 'Approved';
    await fetch(`${environment.apiBaseUrl}/evaluations/${evaluationId}/reviews`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ reviewer: 'development-user', decision, notes }),
    });
  }
}
