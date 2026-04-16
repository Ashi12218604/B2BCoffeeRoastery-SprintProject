import { createFeatureSelector, createSelector } from '@ngrx/store';
import { InventoryState } from '../app.state';

export const selectInventoryState = createFeatureSelector<InventoryState>('inventory');
export const selectAllInventory = createSelector(selectInventoryState, (s) => s.items);
export const selectInventoryLoading = createSelector(selectInventoryState, (s) => s.loading);
export const selectInventoryError = createSelector(selectInventoryState, (s) => s.error);
export const selectLowStockItems = createSelector(selectAllInventory, (items) => items.filter(i => i.isLowStock));
