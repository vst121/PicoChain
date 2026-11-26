import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiClientService {
  async get<T>(path: string, signal?: AbortSignal): Promise<T> {
    return this.request<T>(path, { method: 'GET', signal });
  }

  async post<T>(path: string, body?: unknown, signal?: AbortSignal): Promise<T> {
    return this.request<T>(path, {
      method: 'POST',
      signal,
      headers: { 'Content-Type': 'application/json' },
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  }

  private async request<T>(path: string, init: RequestInit): Promise<T> {
    const headers = new Headers(init.headers);
    const token = localStorage.getItem('picochain.access-token');
    if (token) headers.set('Authorization', `Bearer ${token}`);
    const response = await fetch(`${environment.apiBaseUrl}${path}`, { ...init, headers });
    if (!response.ok) throw new Error(`API request failed (${response.status})`);
    return await response.json() as T;
  }
}
