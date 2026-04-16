// ═══════════════════════════════════════════════════════════
// Product Models
// ═══════════════════════════════════════════════════════════

export interface Product {
  id: string;
  name: string;
  description: string;
  sku: string;
  price: number;
  discountedPrice?: number;
  origin: string;
  category: string;
  roastLevel: string;
  weightInGrams: number;
  imageUrl: string;
  isActive: boolean;
  isFeatured: boolean;
  minimumOrderQuantity: number;
  averageRating: number;
  reviewCount: number;
  createdAt: string;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  sku: string;
  price: number;
  discountedPrice?: number;
  origin: string;
  category: number;       // ProductCategory enum
  roastLevel: number;     // RoastLevel enum
  weightInGrams: number;
  imageUrl: string;
  isFeatured: boolean;
  minimumOrderQuantity: number;
}

export interface UpdateProductRequest {
  name: string;
  description: string;
  price: number;
  discountedPrice?: number;
  origin: string;
  category: number;
  roastLevel: number;
  weightInGrams: number;
  imageUrl: string;
  isFeatured: boolean;
  isActive: boolean;
  minimumOrderQuantity: number;
}

export interface ProductReview {
  id: string;
  clientId: string;
  clientName: string;
  rating: number;
  comment: string;
  createdAt: string;
}

export interface AddReviewRequest {
  rating: number;
  comment: string;
}

export interface ProductsPagedResponse {
  items: Product[];
  totalCount: number;
  page: number;
  pageSize: number;
}
