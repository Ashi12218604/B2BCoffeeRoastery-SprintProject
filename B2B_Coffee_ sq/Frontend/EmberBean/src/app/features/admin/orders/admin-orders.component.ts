import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { Order } from '../../../shared/models/order.model';
import { selectAllOrders, selectOrdersLoading } from '../../../store/orders/orders.selectors';
import { selectUser } from '../../../store/auth/auth.selectors';
import * as OrderActions from '../../../store/orders/orders.actions';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="orders-page">
      <div class="page-header animate-fade-in-up">
        <h1 class="page-title">All <span class="highlight">Orders</span></h1>
        <p class="page-sub">Manage and process all customer orders</p>
      </div>
      <div class="loading-state" *ngIf="loading$ | async"><div class="spinner"></div></div>
      <div class="orders-table-wrap" *ngIf="(orders$ | async) as orders">
        <table class="data-table" *ngIf="orders.length > 0">
          <thead>
            <tr><th>Order ID</th><th>Client</th><th>Date</th><th>Items</th><th>Total</th><th>Status</th><th>Action</th></tr>
          </thead>
          <tbody>
            <tr *ngFor="let o of orders" id="admin-order-{{ o.id }}">
              <td class="mono">#{{ o.id.substring(0,8).toUpperCase() }}</td>
              <td>{{ o.clientName || '—' }}</td>
              <td>{{ o.placedAt | date:'MMM dd, yyyy' }}</td>
              <td>{{ o.items.length }} items</td>
              <td class="price">₹{{ o.totalAmount | number:'1.2-2' }}</td>
              <td>
                <span class="status-pill" [attr.data-status]="o.status">{{ o.status }}</span>
                <button class="cancel-link" *ngIf="(user$ | async)?.role === 'SuperAdmin' && o.status !== 'Delivered' && o.status !== 'Cancelled' && o.status !== 'Dispatched'" (click)="adminCancelOrder(o.id)">
                  Cancel
                </button>
              </td>
              <td class="action-cell">
                <select class="status-select" [value]="o.status" (change)="updateStatus(o.id, $event)" *ngIf="o.status !== 'Delivered' && o.status !== 'Cancelled' && o.status !== 'Rejected'">
                  <option value="" disabled>Move to...</option>
                  <option value="Confirmed">Confirmed</option>
                  <option value="InProcess">In-Process</option>
                  <option value="Dispatched">Dispatched</option>
                  <option value="Delivered">Delivered</option>
                  <option value="Rejected">Rejected</option>
                </select>
                <div *ngIf="o.status === 'Delivered' || o.status === 'Cancelled' || o.status === 'Rejected'">
                  <span class="final-status">{{ o.status }}</span>
                  <div *ngIf="o.rejectionReason" class="rejection-reason" title="{{ o.rejectionReason }}">
                    Reason: {{ o.rejectionReason.length > 20 ? (o.rejectionReason | slice:0:20) + '...' : o.rejectionReason }}
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
        <div class="empty-state" *ngIf="orders.length === 0 && !(loading$ | async)">
          <div class="empty-icon">📋</div><h3>No orders yet</h3>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .orders-page { max-width: 1200px; }
    .page-header { margin-bottom: 24px; }
    .page-title { font-family: 'Playfair Display', serif; font-size: 1.875rem; font-weight: 600; color: #FDFCF5; margin-bottom: 6px; }
    .highlight { color: #B87333; }
    .page-sub { font-family: 'Inter', sans-serif; font-size: 0.875rem; color: #FDF5E6; text-shadow: 0 1px 4px rgba(0,0,0,0.3); }
    .loading-state { display: flex; justify-content: center; padding: 60px 0; }
    .spinner { width: 36px; height: 36px; border: 3px solid rgba(184,115,51,0.2); border-top: 3px solid #B87333; border-radius: 50%; animation: spin 0.8s linear infinite; }
    .orders-table-wrap {
      background: rgba(30,30,30,0.4); backdrop-filter: blur(20px); border: 1px solid rgba(184,115,51,0.15);
      border-radius: 12px; overflow: hidden;
    }
    .data-table {
      width: 100%; border-collapse: collapse;
      th { padding: 14px 16px; text-align: left; font-family: 'Inter', sans-serif; font-size: 0.65rem; font-weight: 600;
           color: rgba(253,252,245,0.35); letter-spacing: 0.1em; text-transform: uppercase;
           border-bottom: 1px solid rgba(255,255,255,0.06); background: rgba(255,255,255,0.02); }
      td { padding: 14px 16px; font-family: 'Inter', sans-serif; font-size: 0.875rem; color: rgba(253,252,245,0.7);
           border-bottom: 1px solid rgba(255,255,255,0.03); vertical-align: middle; }
      tbody tr { transition: background 0.2s; &:hover { background: rgba(255,255,255,0.02); } }
    }
    .mono { font-family: 'JetBrains Mono', monospace; font-size: 0.8rem; color: #FDFCF5; font-weight: 600; }
    .price { font-family: 'Playfair Display', serif; font-weight: 700; color: #B87333; }
    .status-pill {
      padding: 4px 12px; border-radius: 20px; font-size: 0.7rem; font-weight: 600;
      &[data-status="Pending"] { background: rgba(255,191,0,0.12); color: #FFD54F; }
      &[data-status="Confirmed"] { background: rgba(76,175,80,0.12); color: #81C784; }
      &[data-status="In-Process"] { background: rgba(41,182,246,0.12); color: #4FC3F7; }
      &[data-status="Dispatched"] { background: rgba(171,71,188,0.12); color: #CE93D8; }
      &[data-status="Delivered"] { background: rgba(76,175,80,0.12); color: #66BB6A; }
      &[data-status="Cancelled"] { background: rgba(244,67,54,0.12); color: #EF5350; }
    }
    .cancel-link { background: none; border: none; color: #EF5350; font-size: 0.65rem; text-decoration: underline; cursor: pointer; display: block; margin-top: 4px; padding: 0; opacity: 0.6; &:hover { opacity: 1; } }
    .status-select {
      background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); border-radius: 8px;
      color: rgba(253,252,245,0.7); padding: 6px 10px; font-size: 0.8rem; outline: none; cursor: pointer;
      option { background: #1a1a1a; }
      &:focus { border-color: rgba(184,115,51,0.5); }
    }
    .final-status { font-size: 0.75rem; color: rgba(253,252,245,0.3); display: block; }
    .rejection-reason { font-size: 0.65rem; color: #EF5350; margin-top: 4px; font-style: italic; }
    .empty-state { text-align: center; padding: 60px; }
    .empty-icon { font-size: 40px; opacity: 0.3; margin-bottom: 12px; }
    h3 { font-family: 'Playfair Display', serif; font-weight: 600; color: #FDFCF5; }
  `]
})
export class AdminOrdersComponent implements OnInit {
  orders$!: Observable<Order[]>;
  loading$!: Observable<boolean>;
  user$!: Observable<any>; // Using any here assuming User isn't explicitly imported or we don't care

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.user$ = this.store.select(selectUser);
    this.orders$ = this.store.select(selectAllOrders);
    this.loading$ = this.store.select(selectOrdersLoading);
    this.store.dispatch(OrderActions.loadOrders({}));
  }

  updateStatus(orderId: string, event: Event): void {
    const status = (event.target as HTMLSelectElement).value;
    if (!status) return;
    const statusMap: Record<string, number> = {
      'Confirmed': 1, 'InProcess': 2, 'Dispatched': 3, 'Delivered': 4, 'Rejected': 5, 'Cancelled': 6
    };
    const newStatus = statusMap[status];
    if (newStatus === undefined) return;

    let note = undefined;
    if (status === 'Rejected' || status === 'Cancelled') {
      const reason = prompt(`Please enter a reason for marking this order as ${status}:`);
      if (reason === null) {
        (event.target as HTMLSelectElement).value = '';
        return;
      }
      note = reason || undefined;
    }

    this.store.dispatch(OrderActions.updateOrderStatus({
      id: orderId,
      request: { newStatus, note }
    }));
  }

  adminCancelOrder(id: string): void {
    if (confirm('Are you sure you want to cancel this order as SuperAdmin?')) {
      this.store.dispatch(OrderActions.cancelOrder({ id }));
    }
  }
}
