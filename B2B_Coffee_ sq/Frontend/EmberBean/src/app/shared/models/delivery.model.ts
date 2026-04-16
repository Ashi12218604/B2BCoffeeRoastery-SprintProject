// ═══════════════════════════════════════════════════════════
// Delivery Models
// ═══════════════════════════════════════════════════════════

export interface Delivery {
  id: string;
  orderId: string;
  clientId: string;
  clientEmail: string;
  clientName: string;
  deliveryAddress: string;
  city: string;
  state: string;
  pinCode: string;
  approvedByAdminName?: string;
  productNames?: string;
  status: string;
  trackingNumber?: string;
  assignedAgent?: string;
  agentPhone?: string;
  estimatedDeliveryDate?: string;
  actualDeliveryDate?: string;
  notes?: string;
  location?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateDeliveryRequest {
  orderId: string;
  clientId: string;
  clientEmail: string;
  clientName: string;
  deliveryAddress: string;
  city: string;
  state: string;
  pinCode: string;
  estimatedDeliveryDate?: string;
  notes?: string;
}

export interface UpdateDeliveryStatusRequest {
  newStatus: number;
  note?: string;
  location?: string;
  trackingNumber?: string;
  assignedAgent?: string;
  agentPhone?: string;
}

export interface AssignAgentRequest {
  agentName: string;
  agentPhone: string;
}

export interface UpdateDeliveryAddressRequest {
  deliveryAddress: string;
  city: string;
  state: string;
  pinCode: string;
}
