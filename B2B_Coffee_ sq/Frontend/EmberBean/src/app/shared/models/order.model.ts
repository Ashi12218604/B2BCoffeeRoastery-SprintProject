// ═══════════════════════════════════════════════════════════
// Order Models
// ═══════════════════════════════════════════════════════════

export interface OrderItemRequest {
  productId: string;
  productName: string;
  sku: string;
  quantity: number;
  unitPrice: number;
}

export interface PlaceOrderRequest {
  items: OrderItemRequest[];
  notes?: string;
  deliveryAddress?: string;
  city?: string;
  state?: string;
  pinCode?: string;
}

export interface Order {
  id: string;
  clientId: string;
  clientEmail: string;
  clientName: string;
  companyName: string;
  status: string;
  totalAmount: number;
  rejectionReason?: string;
  notes?: string;
  paymentStatus: string;
  razorpayOrderId?: string;
  razorpayPaymentId?: string;
  paidAt?: string;
  items: OrderItem[];
  statusHistory: OrderStatusHistory[];
  placedAt: string;
  updatedAt: string;
}

export interface OrderItem {
  id: string;
  productId: string;
  productName: string;
  sku: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

export interface OrderStatusHistory {
  status: string;
  note?: string;
  changedAt: string;
}

export interface UpdateOrderStatusRequest {
  newStatus: number;    // OrderStatus enum
  note?: string;
  trackingNumber?: string;
}

export interface PaymentResponse {
  success: boolean;
  message: string;
  razorpayOrderId?: string;
  demoPaymentIdToUseForTest?: string;
  demoSignatureToUseForTest?: string;
}

export interface PaymentVerificationRequest {
  razorpayPaymentId: string;
  razorpaySignature: string;
}

// Enums
export enum OrderStatus {
  Pending = 0,
  Confirmed = 1,
  InProcess = 2,
  Dispatched = 3,
  Delivered = 4,
  Rejected = 5,
  Cancelled = 6
}

export enum PaymentStatus {
  Unpaid = 0,
  Paid = 1,
  Refunded = 2,
  Failed = 3
}
