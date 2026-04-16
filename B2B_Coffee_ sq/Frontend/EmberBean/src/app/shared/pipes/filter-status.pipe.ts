import { Pipe, PipeTransform } from '@angular/core';
import { Order } from '../models/order.model';

@Pipe({ name: 'filterStatus', standalone: true })
export class FilterStatusPipe implements PipeTransform {
  transform(orders: Order[], status: string): number {
    if (!orders) return 0;
    return orders.filter(o => o.status === status).length;
  }
}
