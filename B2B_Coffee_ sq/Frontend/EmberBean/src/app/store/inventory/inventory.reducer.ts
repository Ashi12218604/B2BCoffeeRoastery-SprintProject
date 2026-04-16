import { createReducer, on } from '@ngrx/store';
import { InventoryState } from '../app.state';
import * as InventoryActions from './inventory.actions';

export const initialInventoryState: InventoryState = {
  items: [],
  loading: false,
  error: null
};

export const inventoryReducer = createReducer(
  initialInventoryState,

  on(InventoryActions.loadInventory, (state) => ({ ...state, loading: true, error: null })),
  on(InventoryActions.loadInventorySuccess, (state, { items }) => ({
    ...state, items, loading: false
  })),
  on(InventoryActions.loadInventoryFailure, (state, { error }) => ({
    ...state, loading: false, error
  })),

  on(InventoryActions.upsertInventorySuccess, (state, { item }) => ({
    ...state,
    items: state.items.some(i => i.productId === item.productId)
      ? state.items.map(i => i.productId === item.productId ? item : i)
      : [item, ...state.items],
    loading: false
  })),

  on(InventoryActions.restockInventorySuccess, (state, { item }) => ({
    ...state,
    items: state.items.map(i => i.productId === item.productId ? item : i),
    loading: false
  }))
);
