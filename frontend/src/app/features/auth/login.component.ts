import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService, CurrentUser } from '../../core/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  template: `<main class="login-page"><section class="login-card"><div class="brand-mark">P</div><h1>Welcome to PicoChain</h1><p>Sign in to operate the logistics intelligence workspace.</p><label>Name<input [(ngModel)]="name" placeholder="Your name"></label><label>Role<select [(ngModel)]="role"><option value="Operator">Operator</option><option value="Manager">Manager</option></select></label><button (click)="login()">Enter workspace</button><small>Development authentication is enabled for local demonstration.</small></section></main>`,
  styles: [`.login-page{min-height:calc(100vh - 4rem);display:grid;place-items:center}.login-card{width:min(420px,100%);background:#fff;padding:2.5rem;border-radius:1rem;box-shadow:0 18px 50px #16294c18}.brand-mark{display:grid;place-items:center;width:2.5rem;height:2.5rem;border-radius:.7rem;background:#3d7bfd;color:#fff;font-weight:800}.login-card h1{margin-bottom:.4rem}.login-card p{color:#63718a}.login-card label{display:block;margin:1rem 0;font-size:.82rem;font-weight:600}.login-card input,.login-card select{display:block;width:100%;margin-top:.4rem;padding:.7rem;border:1px solid #ccd6e4;border-radius:.45rem;font:inherit}.login-card button{width:100%;padding:.75rem;border:0;border-radius:.45rem;background:#3d7bfd;color:#fff;font-weight:700;cursor:pointer}.login-card small{display:block;color:#7b879a;margin-top:1rem}`],
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  name = '';
  role: CurrentUser['role'] = 'Operator';
  readonly busy = signal(false);

  async login(): Promise<void> {
    this.busy.set(true);
    try {
      await this.auth.login(this.name, this.role);
      await this.router.navigateByUrl('/');
    } finally {
      this.busy.set(false);
    }
  }
}
