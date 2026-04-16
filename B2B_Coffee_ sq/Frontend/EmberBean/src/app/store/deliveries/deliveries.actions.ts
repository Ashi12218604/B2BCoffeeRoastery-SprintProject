import { createAction, props } from '@ngrx/store';
import { Delivery, CreateDeliveryRequest, UpdateDeliveryStatusRequest, AssignAgentRequest, UpdateDeliveryAddressRequest } from '../../shared/models/delivery.model';

export const loadDeliveries = createAction('[Deliveries] Load All', props<{ params?: any }>());
export const loadDeliveriesSuccess = createAction('[Deliveries] Load All Success', props<{ deliveries: Delivery[] }>());
export const loadDeliveriesFailure = createAction('[Deliveries] Load All Failure', props<{ error: string }>());

export const loadDelivery = createAction('[Deliveries] Load One', props<{ id: string }>());
export const loadDeliverySuccess = createAction('[Deliveries] Load One Success', props<{ delivery: Delivery }>());
export const loadDeliveryFailure = createAction('[Deliveries] Load One Failure', props<{ error: string }>());

export const createDelivery = createAction('[Deliveries] Create', props<{ request: CreateDeliveryRequest }>());
export const createDeliverySuccess = createAction('[Deliveries] Create Success', props<{ delivery: Delivery }>());
export const createDeliveryFailure = createAction('[Deliveries] Create Failure', props<{ error: string }>());

export const updateDeliveryStatus = createAction('[Deliveries] Update Status', props<{ id: string; request: UpdateDeliveryStatusRequest }>());
export const updateDeliveryStatusSuccess = createAction('[Deliveries] Update Status Success', props<{ delivery: Delivery }>());
export const updateDeliveryStatusFailure = createAction('[Deliveries] Update Status Failure', props<{ error: string }>());

export const assignAgent = createAction('[Deliveries] Assign Agent', props<{ id: string; request: AssignAgentRequest }>());
export const assignAgentSuccess = createAction('[Deliveries] Assign Agent Success', props<{ delivery: Delivery }>());
export const assignAgentFailure = createAction('[Deliveries] Assign Agent Failure', props<{ error: string }>());

export const updateDeliveryAddress = createAction('[Deliveries] Update Address', props<{ id: string; request: UpdateDeliveryAddressRequest }>());
export const updateDeliveryAddressSuccess = createAction('[Deliveries] Update Address Success', props<{ delivery: Delivery }>());
export const updateDeliveryAddressFailure = createAction('[Deliveries] Update Address Failure', props<{ error: string }>());
