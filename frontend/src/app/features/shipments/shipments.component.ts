import { Component, signal } from '@angular/core';
import { environment } from '../../../environments/environment';

type Shipment = { id: string; reference?: string; status?: string; carrierName?: string };

@Component({
  selector: 'app-shipments',
  standalone: true,
  template: `<main><h1>Shipments</h1><p class="muted">Canonical shipment status from the PicoChain backend.</p><table><thead><tr><th>Reference</th><th>Status</th><th>Carrier</th></tr></thead><tbody>@for (shipment of shipments(); track shipment.id) {<tr><td>{{ shipment.reference ?? shipment.id }}</td><td>{{ shipment.status ?? 'Unknown' }}</td><td>{{ shipment.carrierName ?? '—' }}</td></tr>}</tbody></table></main>`,
  styles: ['table{width:100%;background:#fff;border-collapse:collapse}th,td{text-align:left;padding:1rem;border-bottom:1px solid #e5e7eb}'],
})
export class ShipmentsComponent {
  readonly shipments = signal<Shipment[]>([]);

  constructor() { void this.load(); }

  private async load(): Promise<void> {
    const response = await fetch(`${environment.apiBaseUrl}/shipments`);
    if (response.ok) this.shipments.set(await response.json() as Shipment[]);
  }
}
