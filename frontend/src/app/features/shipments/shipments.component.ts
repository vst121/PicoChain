import { Component, signal } from '@angular/core';
import { environment } from '../../../environments/environment';

type Shipment = { id: string; shipmentNumber: string; status: string; eventCount: number };

@Component({
  selector: 'app-shipments',
  standalone: true,
  template: `<main><h1>Shipments</h1><p class="muted">Canonical shipment status from the PicoChain backend.</p><table><thead><tr><th>Shipment</th><th>Status</th><th>Events</th><th>Action</th></tr></thead><tbody>@for (shipment of shipments(); track shipment.id) {<tr><td>{{ shipment.shipmentNumber }}</td><td>{{ shipment.status }}</td><td>{{ shipment.eventCount }}</td><td><button (click)="markDelayed(shipment.id)" [disabled]="shipment.status === 'Delayed'">{{ shipment.status === 'Delayed' ? 'Already delayed' : 'Mark delayed' }}</button></td></tr>}</tbody></table></main>`,
  styles: ['table{width:100%;background:#fff;border-collapse:collapse}th,td{text-align:left;padding:1rem;border-bottom:1px solid #e5e7eb}button{padding:.4rem .7rem}'],
})
export class ShipmentsComponent {
  readonly shipments = signal<Shipment[]>([]);

  constructor() {
    void this.load();
    const events = new EventSource(`${environment.apiBaseUrl.replace('/api/v1', '')}/api/v1/events`);
    events.addEventListener('shipment-delayed', () => void this.load());
    events.addEventListener('action-executed', () => void this.load());
  }

  private async load(): Promise<void> {
    const response = await fetch(`${environment.apiBaseUrl}/shipments`);
    if (response.ok) this.shipments.set(await response.json() as Shipment[]);
  }

  async markDelayed(id: string): Promise<void> {
    const response = await fetch(`${environment.apiBaseUrl}/shipments/${id}/delay`, { method: 'POST', headers: { 'Content-Type': 'application/json', 'X-Correlation-Id': crypto.randomUUID() }, body: JSON.stringify({ reason: 'Delay reported from operations dashboard' }) });
    if (response.ok) await this.load();
  }
}
