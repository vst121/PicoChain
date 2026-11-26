import { Injectable, signal } from '@angular/core';

export type CurrentUser = { name: string; role: 'Operator' | 'Manager' };

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly storageKey = 'picochain.current-user';
  readonly user = signal<CurrentUser | null>(this.readUser());

  login(name: string, role: CurrentUser['role']): void {
    const user = { name: name.trim() || 'Demo Operator', role };
    localStorage.setItem(this.storageKey, JSON.stringify(user));
    this.user.set(user);
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    this.user.set(null);
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
