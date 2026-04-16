// ═══════════════════════════════════════════════════════════
// Inventory Models
// ═══════════════════════════════════════════════════════════

export interface InventoryItem {
  id: string;
  productId: string;
  productName: string;
  sku: string;
  quantityAvailable: number;
  reservedQuantity: number;
  lowStockThreshold: number;
  isLowStock: boolean;
  lastRestockedAt?: string;
  updatedAt: string;
}

export interface UpsertInventoryRequest {
  productId: string;
  productName: string;
  sku: string;
  quantityAvailable: number;
  lowStockThreshold: number;
}

export interface RestockRequest {
  quantity: number;
  reason: string;
}

export interface InventoryTransaction {
  id: string;
  productId: string;
  type: string;
  quantity: number;
  reason: string;
  createdAt: string;
}
