import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="app-shell">
      <aside class="sidebar">
        <a class="brand" routerLink="/">
          <span class="brand-mark">P</span>
          <span><strong>PicoChain</strong><small>Logistics intelligence</small></span>
        </a>
        <nav aria-label="Main navigation">
          <p class="nav-label">Operations</p>
          <a routerLink="/" [routerLinkActiveOptions]="{ exact: true }" routerLinkActive="active">Dashboard</a>
          <a routerLink="/shipments" routerLinkActive="active">Shipments</a>
          <a routerLink="/orders" routerLinkActive="active">Orders</a>
          <a routerLink="/approvals" routerLinkActive="active">Approval Center</a>
          <a routerLink="/ai-copilot" routerLinkActive="active">AI Copilot</a>
          <p class="nav-label">Intelligence</p>
          <a routerLink="/agent-control" routerLinkActive="active">Agent Control</a>
          <a routerLink="/evaluations" routerLinkActive="active">Evaluations</a>
          <a routerLink="/observability" routerLinkActive="active">Monitoring</a>
          <a routerLink="/audit" routerLinkActive="active">Audit Trail</a>
        </nav>
        <div class="sidebar-footer"><span class="status-dot"></span> Platform online</div>
      </aside>
      <section class="workspace">
        <header class="topbar"><span class="eyebrow">Operations workspace</span><span class="user-menu">{{ auth.user()?.name }} · {{ auth.user()?.role }} <button (click)="logout()">Sign out</button></span></header>
        <main class="content"><router-outlet /></main>
      </section>
    </div>
  `,
})
export class AppComponent {
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }
}
