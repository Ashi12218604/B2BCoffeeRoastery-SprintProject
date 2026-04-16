import { createFeatureSelector, createSelector } from '@ngrx/store';
import { OrderState } from '../app.state';

export const selectOrderState = createFeatureSelector<OrderState>('orders');
export const selectAllOrders = createSelector(selectOrderState, (s) => s.orders);
export const selectSelectedOrder = createSelector(selectOrderState, (s) => s.selectedOrder);
export const selectOrdersLoading = createSelector(selectOrderState, (s) => s.loading);
export const selectOrdersError = createSelector(selectOrderState, (s) => s.error);
