import { Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';

export type CurrentUser = { name: string; role: 'Operator' | 'Manager' };

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly storageKey = 'picochain.current-user';
  private readonly tokenKey = 'picochain.access-token';
  readonly user = signal<CurrentUser | null>(this.readUser());

  async login(name: string, role: CurrentUser['role']): Promise<void> {
    const user = { name: name.trim() || 'Demo Operator', role };
    const response = await fetch(`${environment.apiBaseUrl.replace('/api/v1', '')}/auth/dev-token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userName: user.name, role: user.role }),
    });
    if (!response.ok) throw new Error('Could not sign in to the backend.');
    const token = await response.json() as { accessToken: string };
    localStorage.setItem(this.tokenKey, token.accessToken);
    localStorage.setItem(this.storageKey, JSON.stringify(user));
    this.user.set(user);
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    localStorage.removeItem(this.tokenKey);
    this.user.set(null);
  }

  token(): string | null { return localStorage.getItem(this.tokenKey); }

  authorizationHeaders(): HeadersInit {
    const token = this.token();
    return token ? { Authorization: `Bearer ${token}` } : {};
  }

  private readUser(): CurrentUser | null {
    try {
      const value = localStorage.getItem(this.storageKey);
      return value ? JSON.parse(value) as CurrentUser : null;
    } catch {
      return null;
    }
  }
}
