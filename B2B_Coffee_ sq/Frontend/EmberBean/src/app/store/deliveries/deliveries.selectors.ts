import { createFeatureSelector, createSelector } from '@ngrx/store';
import { DeliveryState } from '../app.state';

export const selectDeliveryState = createFeatureSelector<DeliveryState>('deliveries');
export const selectAllDeliveries = createSelector(selectDeliveryState, (s) => s.deliveries);
export const selectSelectedDelivery = createSelector(selectDeliveryState, (s) => s.selectedDelivery);
export const selectDeliveriesLoading = createSelector(selectDeliveryState, (s) => s.loading);
export const selectDeliveriesError = createSelector(selectDeliveryState, (s) => s.error);
