import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError } from 'rxjs/operators';
import { InventoryService } from '../../core/services/inventory.service';
import * as InventoryActions from './inventory.actions';

@Injectable()
export class InventoryEffects {
  private actions$ = inject(Actions);
  private inventoryService = inject(InventoryService);

  loadInventory$ = createEffect(() =>
    this.actions$.pipe(
      ofType(InventoryActions.loadInventory),
      mergeMap(({ lowStockOnly }) =>
        this.inventoryService.getAll(lowStockOnly).pipe(
          map(items => InventoryActions.loadInventorySuccess({ items })),
          catchError(err => of(InventoryActions.loadInventoryFailure({ error: err.error?.message || 'Failed to load inventory' })))
        )
      )
    )
  );

  upsertInventory$ = createEffect(() =>
    this.actions$.pipe(
      ofType(InventoryActions.upsertInventory),
      mergeMap(({ request }) =>
        this.inventoryService.upsert(request).pipe(
          map(item => InventoryActions.upsertInventorySuccess({ item })),
          catchError(err => of(InventoryActions.upsertInventoryFailure({ error: err.error?.message || 'Failed to update inventory' })))
        )
      )
    )
  );

  restockInventory$ = createEffect(() =>
    this.actions$.pipe(
      ofType(InventoryActions.restockInventory),
      mergeMap(({ productId, request }) =>
        this.inventoryService.restock(productId, request).pipe(
          map(item => InventoryActions.restockInventorySuccess({ item })),
          catchError(err => of(InventoryActions.restockInventoryFailure({ error: err.error?.message || 'Failed to restock' })))
        )
      )
    )
  );
}
