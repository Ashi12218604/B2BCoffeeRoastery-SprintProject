import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Delivery, CreateDeliveryRequest,
  UpdateDeliveryStatusRequest, AssignAgentRequest
} from '../../shared/models/delivery.model';

@Injectable({ providedIn: 'root' })
export class DeliveryService {
  private readonly baseUrl = 'http://localhost:7101/api/deliveries';

  constructor(private http: HttpClient) {}

  getAll(params?: {
    status?: number;
    page?: number;
    pageSize?: number;
  }): Observable<Delivery[]> {
    let httpParams = new HttpParams();
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          httpParams = httpParams.set(key, value.toString());
        }
      });
    }
    return this.http.get<Delivery[]>(this.baseUrl, { params: httpParams });
  }

  getById(id: string): Observable<Delivery> {
    return this.http.get<Delivery>(`${this.baseUrl}/${id}`);
  }

  getByOrder(orderId: string): Observable<Delivery> {
    return this.http.get<Delivery>(`${this.baseUrl}/order/${orderId}`);
  }

  create(request: CreateDeliveryRequest): Observable<Delivery> {
    return this.http.post<Delivery>(this.baseUrl, request);
  }

  updateStatus(id: string, request: UpdateDeliveryStatusRequest): Observable<Delivery> {
    return this.http.put<Delivery>(`${this.baseUrl}/${id}/status`, request);
  }

  assignAgent(id: string, request: AssignAgentRequest): Observable<Delivery> {
    return this.http.put<Delivery>(`${this.baseUrl}/${id}/assign-agent`, request);
  }

  updateAddress(id: string, request: Partial<Delivery>): Observable<Delivery> {
    return this.http.put<Delivery>(`${this.baseUrl}/${id}/address`, request);
  }
}
