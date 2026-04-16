import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError } from 'rxjs/operators';
import { DeliveryService } from '../../core/services/delivery.service';
import * as DeliveryActions from './deliveries.actions';

@Injectable()
export class DeliveryEffects {
  private actions$ = inject(Actions);
  private deliveryService = inject(DeliveryService);

  loadDeliveries$ = createEffect(() =>
    this.actions$.pipe(
      ofType(DeliveryActions.loadDeliveries),
      mergeMap(({ params }) =>
        this.deliveryService.getAll(params).pipe(
          map(deliveries => DeliveryActions.loadDeliveriesSuccess({ deliveries })),
          catchError(err => of(DeliveryActions.loadDeliveriesFailure({ error: err.error?.message || 'Failed to load deliveries' })))
        )
      )
    )
  );

  loadDelivery$ = createEffect(() =>
    this.actions$.pipe(
      ofType(DeliveryActions.loadDelivery),
      mergeMap(({ id }) =>
        this.deliveryService.getById(id).pipe(
          map(delivery => DeliveryActions.loadDeliverySuccess({ delivery })),
          catchError(err => of(DeliveryActions.loadDeliveryFailure({ error: err.error?.message || 'Failed to load delivery' })))
        )
      )
    )
  );

  createDelivery$ = createEffect(() =>
    this.actions$.pipe(
      ofType(DeliveryActions.createDelivery),
      mergeMap(({ request }) =>
        this.deliveryService.create(request).pipe(
          map(delivery => DeliveryActions.createDeliverySuccess({ delivery })),
          catchError(err => of(DeliveryActions.createDeliveryFailure({ error: err.error?.message || 'Failed to create delivery' })))
        )
      )
    )
  );

  updateStatus$ = createEffect(() =>
    this.actions$.pipe(
      ofType(DeliveryActions.updateDeliveryStatus),
      mergeMap(({ id, request }) =>
        this.deliveryService.updateStatus(id, request).pipe(
          map(delivery => DeliveryActions.updateDeliveryStatusSuccess({ delivery })),
          catchError(err => of(DeliveryActions.updateDeliveryStatusFailure({ error: err.error?.message || 'Failed to update delivery' })))
        )
      )
    )
  );

  assignAgent$ = createEffect(() =>
    this.actions$.pipe(
      ofType(DeliveryActions.assignAgent),
      mergeMap(({ id, request }) =>
        this.deliveryService.assignAgent(id, request).pipe(
          map(delivery => DeliveryActions.assignAgentSuccess({ delivery })),
          catchError(err => of(DeliveryActions.assignAgentFailure({ error: err.error?.message || 'Failed to assign agent' })))
        )
      )
    )
  );

  updateDeliveryAddress$ = createEffect(() =>
    this.actions$.pipe(
      ofType(DeliveryActions.updateDeliveryAddress),
      mergeMap(({ id, request }) =>
        this.deliveryService.updateAddress(id, request).pipe(
          map(delivery => DeliveryActions.updateDeliveryAddressSuccess({ delivery })),
          catchError(err => of(DeliveryActions.updateDeliveryAddressFailure({ error: err.error?.message || 'Failed to update address' })))
        )
      )
    )
  );
}
