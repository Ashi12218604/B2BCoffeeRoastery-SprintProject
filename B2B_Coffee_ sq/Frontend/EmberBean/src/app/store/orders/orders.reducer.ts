import { createReducer, on } from '@ngrx/store';
import { OrderState } from '../app.state';
import * as OrderActions from './orders.actions';

export const initialOrderState: OrderState = {
  orders: [],
  selectedOrder: null,
  loading: false,
  error: null
};

export const orderReducer = createReducer(
  initialOrderState,

  on(OrderActions.loadOrders, OrderActions.loadMyOrders, (state) => ({ ...state, loading: true, error: null })),
  on(OrderActions.loadOrdersSuccess, OrderActions.loadMyOrdersSuccess, (state, { orders }) => ({
    ...state, orders, loading: false
  })),
  on(OrderActions.loadOrdersFailure, OrderActions.loadMyOrdersFailure, (state, { error }) => ({
    ...state, loading: false, error
  })),

  on(OrderActions.loadOrder, (state) => ({ ...state, loading: true })),
  on(OrderActions.loadOrderSuccess, (state, { order }) => ({
    ...state, selectedOrder: order, loading: false
  })),

  on(OrderActions.placeOrderSuccess, (state, { order }) => ({
    ...state, orders: [order, ...state.orders], loading: false
  })),

  on(OrderActions.updateOrderStatusSuccess, OrderActions.cancelOrderSuccess, (state, { order }) => ({
    ...state,
    orders: state.orders.map(o => o.id === order.id ? order : o),
    selectedOrder: order,
    loading: false
  })),

  on(OrderActions.clearOrderError, (state) => ({ ...state, error: null }))
);
