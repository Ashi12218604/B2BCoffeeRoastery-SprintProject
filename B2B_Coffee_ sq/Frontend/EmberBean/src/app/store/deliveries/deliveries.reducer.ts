import { createReducer, on } from '@ngrx/store';
import { DeliveryState } from '../app.state';
import * as DeliveryActions from './deliveries.actions';

export const initialDeliveryState: DeliveryState = {
  deliveries: [],
  selectedDelivery: null,
  loading: false,
  error: null
};

export const deliveryReducer = createReducer(
  initialDeliveryState,

  on(DeliveryActions.loadDeliveries, (state) => ({ ...state, loading: true, error: null })),
  on(DeliveryActions.loadDeliveriesSuccess, (state, { deliveries }) => ({
    ...state, deliveries, loading: false
  })),
  on(DeliveryActions.loadDeliveriesFailure, (state, { error }) => ({
    ...state, loading: false, error
  })),

  on(DeliveryActions.loadDeliverySuccess, (state, { delivery }) => ({
    ...state, selectedDelivery: delivery, loading: false
  })),

  on(DeliveryActions.createDeliverySuccess, (state, { delivery }) => ({
    ...state, deliveries: [delivery, ...state.deliveries], loading: false
  })),

  on(DeliveryActions.updateDeliveryStatusSuccess, DeliveryActions.assignAgentSuccess, DeliveryActions.updateDeliveryAddressSuccess, (state, { delivery }) => ({
    ...state,
    deliveries: state.deliveries.map(d => d.id === delivery.id ? delivery : d),
    selectedDelivery: delivery,
    loading: false
  }))
);
