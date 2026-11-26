import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RealtimeService {
  connect(events: Record<string, () => void>): EventSource {
    const source = new EventSource(`${environment.apiBaseUrl.replace('/api/v1', '')}/api/v1/events`);
    for (const [eventName, handler] of Object.entries(events)) source.addEventListener(eventName, handler);
    return source;
  }
}
