import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Order, PlaceOrderRequest, UpdateOrderStatusRequest,
  PaymentResponse, PaymentVerificationRequest
} from '../../shared/models/order.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly baseUrl = 'http://localhost:7101/api/orders';

  constructor(private http: HttpClient) {}

  placeOrder(request: PlaceOrderRequest): Observable<Order> {
    return this.http.post<Order>(this.baseUrl, request);
  }

  getMyOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.baseUrl}/my`);
  }

  getById(id: string): Observable<Order> {
    return this.http.get<Order>(`${this.baseUrl}/${id}`);
  }

  getAll(params?: {
    status?: number;
    page?: number;
    pageSize?: number;
  }): Observable<Order[]> {
    let httpParams = new HttpParams();
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          httpParams = httpParams.set(key, value.toString());
        }
      });
    }
    return this.http.get<Order[]>(this.baseUrl, { params: httpParams });
  }

  updateStatus(id: string, request: UpdateOrderStatusRequest): Observable<Order> {
    return this.http.put<Order>(`${this.baseUrl}/${id}/status`, request);
  }

  createPaymentOrder(orderId: string): Observable<PaymentResponse> {
    return this.http.post<PaymentResponse>(`${this.baseUrl}/${orderId}/pay`, {});
  }

  verifyPayment(orderId: string, request: PaymentVerificationRequest): Observable<PaymentResponse> {
    return this.http.post<PaymentResponse>(
      `${this.baseUrl}/${orderId}/verify-payment`, request
    );
  }

  cancelOrder(orderId: string): Observable<Order> {
    return this.http.post<Order>(`${this.baseUrl}/${orderId}/cancel`, {});
  }
}
