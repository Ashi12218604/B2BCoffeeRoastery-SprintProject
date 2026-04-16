import { createAction, props } from '@ngrx/store';
import { InventoryItem, UpsertInventoryRequest, RestockRequest } from '../../shared/models/inventory.model';

export const loadInventory = createAction('[Inventory] Load All', props<{ lowStockOnly?: boolean }>());
export const loadInventorySuccess = createAction('[Inventory] Load All Success', props<{ items: InventoryItem[] }>());
export const loadInventoryFailure = createAction('[Inventory] Load All Failure', props<{ error: string }>());

export const upsertInventory = createAction('[Inventory] Upsert', props<{ request: UpsertInventoryRequest }>());
export const upsertInventorySuccess = createAction('[Inventory] Upsert Success', props<{ item: InventoryItem }>());
export const upsertInventoryFailure = createAction('[Inventory] Upsert Failure', props<{ error: string }>());

export const restockInventory = createAction('[Inventory] Restock', props<{ productId: string; request: RestockRequest }>());
export const restockInventorySuccess = createAction('[Inventory] Restock Success', props<{ item: InventoryItem }>());
export const restockInventoryFailure = createAction('[Inventory] Restock Failure', props<{ error: string }>());
