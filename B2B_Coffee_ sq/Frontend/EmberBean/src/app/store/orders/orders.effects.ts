import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError } from 'rxjs/operators';
import { OrderService } from '../../core/services/order.service';
import * as OrderActions from './orders.actions';

@Injectable()
export class OrderEffects {
  private actions$ = inject(Actions);
  private orderService = inject(OrderService);

  loadOrders$ = createEffect(() =>
    this.actions$.pipe(
      ofType(OrderActions.loadOrders),
      mergeMap(({ params }) =>
        this.orderService.getAll(params).pipe(
          map(orders => OrderActions.loadOrdersSuccess({ orders })),
          catchError(err => of(OrderActions.loadOrdersFailure({ error: err.error?.message || 'Failed to load orders' })))
        )
      )
    )
  );

  loadMyOrders$ = createEffect(() =>
    this.actions$.pipe(
      ofType(OrderActions.loadMyOrders),
      mergeMap(() =>
        this.orderService.getMyOrders().pipe(
          map(orders => OrderActions.loadMyOrdersSuccess({ orders })),
          catchError(err => of(OrderActions.loadMyOrdersFailure({ error: err.error?.message || 'Failed to load orders' })))
        )
      )
    )
  );

  loadOrder$ = createEffect(() =>
    this.actions$.pipe(
      ofType(OrderActions.loadOrder),
      mergeMap(({ id }) =>
        this.orderService.getById(id).pipe(
          map(order => OrderActions.loadOrderSuccess({ order })),
          catchError(err => of(OrderActions.loadOrderFailure({ error: err.error?.message || 'Failed to load order' })))
        )
      )
    )
  );

  placeOrder$ = createEffect(() =>
    this.actions$.pipe(
      ofType(OrderActions.placeOrder),
      mergeMap(({ request }) =>
        this.orderService.placeOrder(request).pipe(
          map(order => OrderActions.placeOrderSuccess({ order })),
          catchError(err => of(OrderActions.placeOrderFailure({ error: err.error?.message || 'Failed to place order' })))
        )
      )
    )
  );

  updateOrderStatus$ = createEffect(() =>
    this.actions$.pipe(
      ofType(OrderActions.updateOrderStatus),
      mergeMap(({ id, request }) =>
        this.orderService.updateStatus(id, request).pipe(
          map(order => OrderActions.updateOrderStatusSuccess({ order })),
          catchError(err => of(OrderActions.updateOrderStatusFailure({ error: err.error?.message || 'Failed to update order' })))
        )
      )
    )
  );

  cancelOrder$ = createEffect(() =>
    this.actions$.pipe(
      ofType(OrderActions.cancelOrder),
      mergeMap(({ id }) =>
        this.orderService.cancelOrder(id).pipe(
          map(order => OrderActions.cancelOrderSuccess({ order })),
          catchError(err => of(OrderActions.cancelOrderFailure({ error: err.error?.message || 'Failed to cancel order' })))
        )
      )
    )
  );
}
