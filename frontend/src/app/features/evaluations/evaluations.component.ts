import { Component, computed, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../environments/environment';

type Evaluation = { id: string; scenarioId: string; evaluatorType: string; overallScore: number; passed: boolean; createdAt: string; agentVersion?: string; promptVersion?: string; modelIdentifier?: string; estimatedCost?: number };

@Component({
  selector: 'app-evaluations',
  standalone: true,
  imports: [FormsModule],
  template: `<main><h1>AI Evaluations</h1><p class="muted">Quality, safety, efficiency, and version comparisons for agent executions.</p><section class="cards"><article class="card"><strong>{{ total() }}</strong><span>Evaluated runs</span></article><article class="card"><strong>{{ passRate() }}%</strong><span>Pass rate</span></article><article class="card"><strong>{{ averageScore() }}</strong><span>Average score</span></article><article class="card"><strong>{{ totalCost() }}</strong><span>Estimated cost</span></article></section><input class="filter" [(ngModel)]="filter" placeholder="Filter scenarios, agents, or models"><table><thead><tr><th>Scenario</th><th>Evaluator</th><th>Score</th><th>Result</th><th>Version</th><th>Cost</th><th>Review</th></tr></thead><tbody>@for (item of filtered(); track item.id) {<tr><td>{{ item.scenarioId }}</td><td>{{ item.evaluatorType }}</td><td><span class="score" [style.width.%]="item.overallScore * 100"></span>{{ item.overallScore }}</td><td>{{ item.passed ? 'Passed' : 'Failed' }}</td><td>{{ item.agentVersion ?? 'n/a' }} / {{ item.promptVersion ?? 'n/a' }}<br>{{ item.modelIdentifier ?? 'n/a' }}</td><td>{{ item.estimatedCost ?? 0 }}</td><td><button (click)="review(item.id)">Review</button></td></tr>} @empty {<tr><td colspan="7">No evaluations match the filter.</td></tr>}</tbody></table></main>`,
  styles: ['.cards{display:grid;grid-template-columns:repeat(4,1fr);gap:1rem;margin:1.5rem 0}.card{background:#fff;padding:1.2rem;border-radius:.7rem}.card strong,.card span{display:block}.card strong{font-size:1.6rem}.card span{color:#63718a;font-size:.78rem;margin-top:.3rem}.filter{width:100%;padding:.7rem;border:1px solid #ccd6e4;border-radius:.45rem;margin-bottom:1rem}table{width:100%;background:#fff;border-collapse:collapse}th,td{text-align:left;padding:.8rem;border-bottom:1px solid #e5e7eb}.score{display:inline-block;height:.45rem;background:#4b83f0;border-radius:99px;margin-right:.5rem;vertical-align:middle}@media(max-width:800px){.cards{grid-template-columns:repeat(2,1fr)}}'],
})
export class EvaluationsComponent {
  readonly evaluations = signal<Evaluation[]>([]);
  filter = '';
  readonly filtered = computed(() => {
    const value = this.filter.toLowerCase();
    return this.evaluations().filter(item => `${item.scenarioId} ${item.agentVersion ?? ''} ${item.modelIdentifier ?? ''}`.toLowerCase().includes(value));
  });
  readonly total = computed(() => this.evaluations().length);
  readonly passRate = computed(() => this.total() ? Math.round(this.evaluations().filter(item => item.passed).length / this.total() * 100) : 0);
  readonly averageScore = computed(() => this.total() ? (this.evaluations().reduce((sum, item) => sum + item.overallScore, 0) / this.total()).toFixed(2) : '0.00');
  readonly totalCost = computed(() => this.evaluations().reduce((sum, item) => sum + (item.estimatedCost ?? 0), 0).toFixed(4));

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
