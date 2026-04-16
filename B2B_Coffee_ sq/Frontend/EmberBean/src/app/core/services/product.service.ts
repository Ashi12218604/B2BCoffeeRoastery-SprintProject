import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  Product, CreateProductRequest, UpdateProductRequest,
  ProductReview, AddReviewRequest
} from '../../shared/models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly baseUrl = 'http://localhost:7101/api/products';

  constructor(private http: HttpClient) {}

  getAll(params?: {
    search?: string;
    category?: number;
    roastLevel?: number;
    isFeatured?: boolean;
    minPrice?: number;
    maxPrice?: number;
    page?: number;
    pageSize?: number;
  }): Observable<Product[]> {
    let httpParams = new HttpParams();
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          httpParams = httpParams.set(key, value.toString());
        }
      });
    }
    return this.http.get<any>(this.baseUrl, { params: httpParams }).pipe(
      map(res => res.items ? res.items : res)
    );
  }

  getById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  create(product: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(this.baseUrl, product);
  }

  update(id: string, product: UpdateProductRequest): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}/${id}`, product);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  getReviews(productId: string): Observable<ProductReview[]> {
    return this.http.get<ProductReview[]>(`${this.baseUrl}/${productId}/reviews`);
  }

  addReview(productId: string, review: AddReviewRequest): Observable<ProductReview> {
    return this.http.post<ProductReview>(`${this.baseUrl}/${productId}/reviews`, review);
  }
}
