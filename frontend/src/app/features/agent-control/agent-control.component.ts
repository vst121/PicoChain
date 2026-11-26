import { Component, signal } from '@angular/core';
import { environment } from '../../../environments/environment';

type AgentTask = { id: string; agentName?: string; status?: string; startedAt?: string; completedAt?: string };

@Component({
  selector: 'app-agent-control',
  standalone: true,
  template: `<main><h1>Agent Control Center</h1><p class="muted">Execution state and lifecycle records from the backend.</p><table><thead><tr><th>Agent</th><th>Status</th><th>Started</th><th>Completed</th></tr></thead><tbody>@for (task of tasks(); track task.id) {<tr><td>{{ task.agentName ?? 'Unknown agent' }}</td><td>{{ task.status ?? 'Unknown' }}</td><td>{{ task.startedAt ?? '—' }}</td><td>{{ task.completedAt ?? '—' }}</td></tr>}</tbody></table></main>`,
  styles: ['table{width:100%;background:#fff;border-collapse:collapse}th,td{text-align:left;padding:1rem;border-bottom:1px solid #e5e7eb}'],
})
export class AgentControlComponent {
  readonly tasks = signal<AgentTask[]>([]);

  constructor() {
    void this.load();
    const events = new EventSource(`${environment.apiBaseUrl.replace('/api/v1', '')}/api/v1/events`);
    for (const name of ['agent-task-started', 'agent-task-failed', 'agent-task-completed'])
      events.addEventListener(name, () => void this.load());
  }

  private async load(): Promise<void> {
    const response = await fetch(`${environment.apiBaseUrl}/agent-tasks`);
    if (response.ok) this.tasks.set(await response.json() as AgentTask[]);
  }
}
