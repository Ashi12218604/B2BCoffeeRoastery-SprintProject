import { User } from '../shared/models/user.model';
import { Product } from '../shared/models/product.model';
import { Order } from '../shared/models/order.model';
import { InventoryItem } from '../shared/models/inventory.model';
import { Delivery } from '../shared/models/delivery.model';

export interface AuthState {
  user: User | null;
  token: string | null;
  loading: boolean;
  error: string | null;
  otpEmail: string | null;       // Track email for OTP flow
  registrationSuccess: boolean;
  otpVerified: boolean;
}

export interface ProductState {
  products: Product[];
  selectedProduct: Product | null;
  loading: boolean;
  error: string | null;
}

export interface OrderState {
  orders: Order[];
  selectedOrder: Order | null;
  loading: boolean;
  error: string | null;
}

export interface InventoryState {
  items: InventoryItem[];
  loading: boolean;
  error: string | null;
}

export interface DeliveryState {
  deliveries: Delivery[];
  selectedDelivery: Delivery | null;
  loading: boolean;
  error: string | null;
}

export interface AppState {
  auth: AuthState;
  products: ProductState;
  orders: OrderState;
  inventory: InventoryState;
  deliveries: DeliveryState;
}
