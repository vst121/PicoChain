import { Component, signal } from '@angular/core';
import { environment } from '../../../environments/environment';

type Order = { id: string; orderNumber: string; customerId: string; status: string; itemCount: number };

@Component({
  selector: 'app-orders',
  standalone: true,
  template: `<main><h1>Orders</h1><p class="muted">Orders from the canonical logistics model.</p><table><thead><tr><th>Order</th><th>Customer</th><th>Status</th><th>Items</th></tr></thead><tbody>@for (order of orders(); track order.id) {<tr><td>{{ order.orderNumber }}</td><td>{{ order.customerId }}</td><td>{{ order.status }}</td><td>{{ order.itemCount }}</td></tr>}</tbody></table></main>`,
  styles: ['table{width:100%;background:#fff;border-collapse:collapse}th,td{text-align:left;padding:.8rem;border-bottom:1px solid #e5e7eb}'],
})
export class OrdersComponent {
  readonly orders = signal<Order[]>([]);

  constructor() { void this.load(); }

  private async load(): Promise<void> {
    const response = await fetch(`${environment.apiBaseUrl}/orders`);
    if (response.ok) this.orders.set(await response.json() as Order[]);
  }
}
