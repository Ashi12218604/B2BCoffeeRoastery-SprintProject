import { createAction, props } from '@ngrx/store';
import { Order, PlaceOrderRequest, UpdateOrderStatusRequest } from '../../shared/models/order.model';

export const loadOrders = createAction('[Orders] Load Orders', props<{ params?: any }>());
export const loadOrdersSuccess = createAction('[Orders] Load Orders Success', props<{ orders: Order[] }>());
export const loadOrdersFailure = createAction('[Orders] Load Orders Failure', props<{ error: string }>());

export const loadMyOrders = createAction('[Orders] Load My Orders');
export const loadMyOrdersSuccess = createAction('[Orders] Load My Orders Success', props<{ orders: Order[] }>());
export const loadMyOrdersFailure = createAction('[Orders] Load My Orders Failure', props<{ error: string }>());

export const loadOrder = createAction('[Orders] Load Order', props<{ id: string }>());
export const loadOrderSuccess = createAction('[Orders] Load Order Success', props<{ order: Order }>());
export const loadOrderFailure = createAction('[Orders] Load Order Failure', props<{ error: string }>());

export const placeOrder = createAction('[Orders] Place Order', props<{ request: PlaceOrderRequest }>());
export const placeOrderSuccess = createAction('[Orders] Place Order Success', props<{ order: Order }>());
export const placeOrderFailure = createAction('[Orders] Place Order Failure', props<{ error: string }>());

export const updateOrderStatus = createAction('[Orders] Update Status', props<{ id: string; request: UpdateOrderStatusRequest }>());
export const updateOrderStatusSuccess = createAction('[Orders] Update Status Success', props<{ order: Order }>());
export const updateOrderStatusFailure = createAction('[Orders] Update Status Failure', props<{ error: string }>());

export const cancelOrder = createAction('[Orders] Cancel Order', props<{ id: string }>());
export const cancelOrderSuccess = createAction('[Orders] Cancel Order Success', props<{ order: Order }>());
export const cancelOrderFailure = createAction('[Orders] Cancel Order Failure', props<{ error: string }>());

export const clearOrderError = createAction('[Orders] Clear Error');
