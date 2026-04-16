import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  InventoryItem, UpsertInventoryRequest,
  RestockRequest, InventoryTransaction
} from '../../shared/models/inventory.model';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private readonly baseUrl = 'http://localhost:7101/api/inventory';

  constructor(private http: HttpClient) {}

  getAll(lowStockOnly?: boolean): Observable<InventoryItem[]> {
    let params = new HttpParams();
    if (lowStockOnly !== undefined) {
      params = params.set('lowStockOnly', lowStockOnly.toString());
    }
    return this.http.get<InventoryItem[]>(this.baseUrl, { params });
  }

  getByProduct(productId: string): Observable<InventoryItem> {
    return this.http.get<InventoryItem>(`${this.baseUrl}/product/${productId}`);
  }

  getTransactions(productId: string): Observable<InventoryTransaction[]> {
    return this.http.get<InventoryTransaction[]>(
      `${this.baseUrl}/product/${productId}/transactions`
    );
  }

  upsert(request: UpsertInventoryRequest): Observable<InventoryItem> {
    return this.http.post<InventoryItem>(this.baseUrl, request);
  }

  restock(productId: string, request: RestockRequest): Observable<InventoryItem> {
    return this.http.post<InventoryItem>(
      `${this.baseUrl}/product/${productId}/restock`, request
    );
  }
}
