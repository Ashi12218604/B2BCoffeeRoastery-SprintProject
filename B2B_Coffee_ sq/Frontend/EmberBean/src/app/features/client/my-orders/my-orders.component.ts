import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { Order } from '../../../shared/models/order.model';
import { selectAllOrders, selectOrdersLoading } from '../../../store/orders/orders.selectors';
import * as OrderActions from '../../../store/orders/orders.actions';

@Component({
  selector: 'app-my-orders',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-orders.component.html',
  styleUrls: ['./my-orders.component.scss']
})
export class MyOrdersComponent implements OnInit {
  orders$!: Observable<Order[]>;
  loading$!: Observable<boolean>;
  selectedOrder: Order | null = null;

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.orders$ = this.store.select(selectAllOrders);
    this.loading$ = this.store.select(selectOrdersLoading);
    this.store.dispatch(OrderActions.loadMyOrders());
  }

  selectOrder(order: Order): void {
    this.selectedOrder = this.selectedOrder?.id === order.id ? null : order;
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'status-pending',
      'Confirmed': 'status-confirmed',
      'In-Process': 'status-processing',
      'Dispatched': 'status-dispatched',
      'Delivered': 'status-delivered',
      'Cancelled': 'status-cancelled',
      'Rejected': 'status-rejected'
    };
    return map[status] || 'status-pending';
  }

  getStatusIcon(status: string): string {
    const map: Record<string, string> = {
      'Pending': '⏳', 'Confirmed': '✅', 'In-Process': '🔄',
      'Dispatched': '🚚', 'Delivered': '📦', 'Cancelled': '❌', 'Rejected': '🚫'
    };
    return map[status] || '📋';
  }

  cancelOrder(orderId: string): void {
    if (confirm('Are you sure you want to cancel this order?')) {
      this.store.dispatch(OrderActions.cancelOrder({ id: orderId }));
    }
  }

  trackEvent(index: number, item: Order): string {
    return item.id;
  }
}
