import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { Product } from '../../../shared/models/product.model';
import { selectAllProducts, selectProductsLoading } from '../../../store/products/products.selectors';
import { selectUser } from '../../../store/auth/auth.selectors';
import { User } from '../../../shared/models/user.model';
import * as ProductActions from '../../../store/products/products.actions';
import { ProductService } from '../../../core/services/product.service';
import { ProductReview } from '../../../shared/models/product.model';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-products.component.html',
  styleUrls: ['./admin-products.component.scss']
})
export class AdminProductsComponent implements OnInit {
  products$!: Observable<Product[]>;
  loading$!: Observable<boolean>;
  user$!: Observable<User | null>;
  searchTerm = '';
  showCreateModal = false;
  isEditing = false;
  editingId: string | null = null;
  newProduct: any = {
    name: '', description: '', sku: '', price: 0, origin: '', category: 1, roastLevel: 2, weightInGrams: 250, imageUrl: '', isFeatured: false, minimumOrderQuantity: 1
  };

  showReviewsModal = false;
  selectedProductForReviews: Product | null = null;
  productReviews: ProductReview[] = [];
  loadingReviews = false;

  constructor(private store: Store, private productService: ProductService) {}

  ngOnInit(): void {
    this.user$ = this.store.select(selectUser);
    this.products$ = this.store.select(selectAllProducts);
    this.loading$ = this.store.select(selectProductsLoading);
    this.store.dispatch(ProductActions.loadProducts({}));
  }

  filterProducts(products: Product[]): Product[] {
    if (!this.searchTerm) return products;
    const term = this.searchTerm.toLowerCase();
    return products.filter(p =>
      p.name.toLowerCase().includes(term) || p.sku.toLowerCase().includes(term)
    );
  }

  toggleActive(product: Product): void {
    this.store.dispatch(ProductActions.updateProduct({
      id: product.id,
      product: { ...product, isActive: !product.isActive } as any
    }));
  }

  deleteProduct(id: string): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.store.dispatch(ProductActions.deleteProduct({ id }));
    }
  }

  openCreateModal(): void {
    this.isEditing = false;
    this.editingId = null;
    this.newProduct = { name: '', description: '', sku: '', price: 0, origin: '', category: 1, roastLevel: 2, weightInGrams: 250, imageUrl: '', isFeatured: false, minimumOrderQuantity: 1 };
    this.showCreateModal = true;
  }

  editProduct(p: Product): void {
    this.isEditing = true;
    this.editingId = p.id;
    this.newProduct = { ...p };
    this.showCreateModal = true;
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  submitCreateProduct(): void {
    if (!this.newProduct.name || !this.newProduct.sku) {
      alert('Name and SKU are required.');
      return;
    }
    
    if (this.isEditing && this.editingId) {
      this.store.dispatch(ProductActions.updateProduct({ id: this.editingId, product: this.newProduct }));
    } else {
      this.newProduct.createdBy = '00000000-0000-0000-0000-000000000001'; 
      this.store.dispatch(ProductActions.createProduct({ product: this.newProduct }));
    }
    this.showCreateModal = false;
  }

  // ── Reviews methods ──
  openReviews(p: Product): void {
    this.selectedProductForReviews = p;
    this.showReviewsModal = true;
    this.loadingReviews = true;
    this.productService.getReviews(p.id).subscribe({
      next: (reviews) => {
        this.productReviews = reviews;
        this.loadingReviews = false;
      },
      error: (err) => {
        console.error('Failed to load reviews', err);
        this.loadingReviews = false;
      }
    });
  }

  closeReviews(): void {
    this.showReviewsModal = false;
    this.selectedProductForReviews = null;
    this.productReviews = [];
  }
}
