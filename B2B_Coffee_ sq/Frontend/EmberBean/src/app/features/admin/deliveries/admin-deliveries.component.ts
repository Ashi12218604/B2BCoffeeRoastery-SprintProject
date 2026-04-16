import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Delivery } from '../../../shared/models/delivery.model';
import { selectAllDeliveries, selectDeliveriesLoading } from '../../../store/deliveries/deliveries.selectors';
import * as DeliveryActions from '../../../store/deliveries/deliveries.actions';

@Component({
  selector: 'app-admin-deliveries',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="deliveries-page">
      <div class="page-header animate-fade-in-up">
        <div>
          <h1 class="page-title">Delivery <span class="highlight">Tracking</span></h1>
          <p class="page-sub">Track and manage all shipments</p>
        </div>
        <div class="search-controls">
          <div class="order-search">
            <select [(ngModel)]="searchDeliveryStatus" class="search-input">
              <option value="">All Statuses</option>
              <option value="Pending">Pending</option>
              <option value="Assigned">Assigned</option>
              <option value="InProcess">In Process</option>
              <option value="Dispatched">Dispatched</option>
              <option value="Delivered">Delivered</option>
            </select>
          </div>
          <div class="order-search">
            <select [(ngModel)]="searchOrderId" class="search-input">
              <option value="">All Orders</option>
              <option *ngFor="let id of uniqueOrderIds" [value]="id">Order #{{ id.substring(0,8).toUpperCase() }}</option>
            </select>
          </div>
        </div>
      </div>
      <div class="loading-state" *ngIf="loading$ | async"><div class="spinner"></div></div>
      <div class="deliveries-list">
        <div class="delivery-card" *ngFor="let d of filteredDeliveries" id="delivery-{{ d.id }}">
          <div class="delivery-header">
            <div class="delivery-id">
              <span class="label">DELIVERY</span>
              <span class="id">#{{ d.id.substring(0,8).toUpperCase() }}</span>
            </div>
            <span class="status-pill" [attr.data-status]="d.status">{{ d.status }}</span>
          </div>
          <div class="delivery-body">
            <div class="info-row"><span class="info-label">Order ID</span><span class="info-val mono">#{{ d.orderId.substring(0,8).toUpperCase() }}</span></div>
            <div class="info-row" *ngIf="d.productNames"><span class="info-label">Items</span><span class="info-val">{{ d.productNames }}</span></div>
            <div class="info-row" *ngIf="d.approvedByAdminName"><span class="info-label">Oversight</span><span class="info-val">{{ d.approvedByAdminName }}</span></div>
            <div class="info-row" *ngIf="d.trackingNumber"><span class="info-label">Tracking</span><span class="info-val mono">{{ d.trackingNumber }}</span></div>
            <div class="info-row" *ngIf="d.assignedAgent"><span class="info-label">Agent</span><span class="info-val">{{ d.assignedAgent }}</span></div>
            <div class="info-row" *ngIf="d.estimatedDeliveryDate"><span class="info-label">ETA</span><span class="info-val">{{ d.estimatedDeliveryDate | date:'MMM dd, yyyy' }}</span></div>
          </div>
          <div class="delivery-actions">
            <button class="btn-outline" (click)="openDetails(d)">📝 View Details & Assign Agent</button>
            <select class="status-select" (change)="updateStatus(d.id, $event)" [value]="d.status" *ngIf="d.status !== 'Delivered'">
              <option value="" disabled>Update status...</option>
              <option value="Assigned">Assigned</option>
              <option value="InProcess">In Process</option>
              <option value="Dispatched">Dispatched</option>
              <option value="Delivered">Delivered</option>
            </select>
          </div>
        </div>
        <div class="empty-state" *ngIf="filteredDeliveries.length === 0 && !(loading$ | async)">
          <div class="empty-icon">🚚</div><h3>No deliveries tracked</h3>
        </div>
      </div>

      <!-- Delivery Details Modal -->
      <div class="modal-overlay" *ngIf="selectedDelivery" (click)="closeDetails()">
        <div class="modal-content animate-fade-in-up" (click)="$event.stopPropagation()">
          <div class="modal-header">
            <h2>Delivery Details <span class="mono">#{{ selectedDelivery.id.substring(0,8).toUpperCase() }}</span></h2>
            <button class="btn-close" (click)="closeDetails()">✕</button>
          </div>
          
          <div class="detail-grid">
            <div class="info-group">
              <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 4px;">
                <label style="margin: 0;">Shipping Address</label>
                <button class="btn-text-edit" *ngIf="!editAddressMode" (click)="toggleEditAddress()">Edit</button>
              </div>
              
              <div *ngIf="!editAddressMode">
                <p>{{ selectedDelivery.deliveryAddress }}<br>
                   {{ selectedDelivery.city }}, {{ selectedDelivery.state }} - {{ selectedDelivery.pinCode }}</p>
              </div>

              <div *ngIf="editAddressMode" class="address-edit-form">
                <input type="text" [(ngModel)]="editAddress.deliveryAddress" placeholder="Street Address" class="form-input full-width" />
                <div class="form-row-small">
                  <input type="text" [(ngModel)]="editAddress.city" placeholder="City" class="form-input" />
                  <input type="text" [(ngModel)]="editAddress.state" placeholder="State" class="form-input" />
                  <input type="text" [(ngModel)]="editAddress.pinCode" placeholder="Pincode" class="form-input" />
                </div>
                <div class="form-actions-small">
                  <button class="btn-cancel-small" (click)="toggleEditAddress()">Cancel</button>
                  <button class="btn-submit-small" (click)="saveAddress()">Save</button>
                </div>
              </div>
            </div>
            <div class="info-group">
              <label>Oversight</label>
              <p>{{ selectedDelivery.approvedByAdminName || 'Automatically Approved' }}</p>
            </div>
            <div class="info-group">
              <label>Timeline</label>
              <p><strong>ETA:</strong> {{ (selectedDelivery.estimatedDeliveryDate | date) || 'TBD' }}</p>
              <p *ngIf="selectedDelivery.actualDeliveryDate" style="color: #66BB6A;">
                <strong>Delivered On:</strong> {{ selectedDelivery.actualDeliveryDate | date:'medium' }}
              </p>
            </div>
          </div>

          <hr style="border-color: rgba(184,115,51,0.2); margin: 20px 0;">

          <h3>Assign Delivery Agent</h3>
          <div class="agent-form">
            <input type="text" [(ngModel)]="agentName" placeholder="Agent Name" class="form-input" />
            <input type="tel" [(ngModel)]="agentPhone" placeholder="10-digit Phone No." class="form-input" maxlength="10" />
            <button class="btn-submit" (click)="saveAgentAssignment()">Assign Agent</button>
          </div>
          <p *ngIf="selectedDelivery.assignedAgent" style="margin-top: 10px; font-size: 0.85rem; color: #81C784;">
             Current Agent: {{ selectedDelivery.assignedAgent }} 
             <span *ngIf="selectedDelivery.agentPhone">({{ selectedDelivery.agentPhone }})</span>
          </p>

        </div>
      </div>

    </div>
  `,
  styles: [`
    .deliveries-page { max-width: 1000px; }
    .page-header { margin-bottom: 24px; display: flex; justify-content: space-between; align-items: flex-end; }
    .search-controls { display: flex; gap: 12px; }
    .order-search { width: 200px; }
    .search-input { width: 100%; padding: 10px 16px; background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); border-radius: 8px; color: #fff; outline: none; transition: border 0.3s; &:focus { border-color: #B87333; } }
    .search-input option { background: #2d231c; color: #fff; }
    .page-title { font-family: 'Playfair Display', serif; font-size: 1.875rem; font-weight: 600; color: #FDFCF5; margin-bottom: 6px; }
    .highlight { color: #B87333; }
    .page-sub { font-family: 'Inter', sans-serif; font-size: 0.875rem; color: #FDF5E6; text-shadow: 0 1px 4px rgba(0,0,0,0.3); }
    .loading-state { display: flex; justify-content: center; padding: 60px 0; }
    .spinner { width: 36px; height: 36px; border: 3px solid rgba(184,115,51,0.2); border-top: 3px solid #B87333; border-radius: 50%; animation: spin 0.8s linear infinite; }
    .deliveries-list { display: flex; flex-direction: column; gap: 16px; }
    .delivery-card {
      background: rgba(30,30,30,0.4); backdrop-filter: blur(20px); border: 1px solid rgba(184,115,51,0.15);
      border-radius: 12px; padding: 20px; transition: all 0.3s;
      &:hover { border-color: rgba(184,115,51,0.35); }
    }
    .delivery-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
    .delivery-id .label { font-family: 'Inter', sans-serif; font-size: 0.6rem; font-weight: 600; color: rgba(253,252,245,0.35); letter-spacing: 0.12em; display: block; }
    .delivery-id .id { font-family: 'JetBrains Mono', monospace; font-size: 0.9rem; color: #FDFCF5; font-weight: 600; }
    .status-pill {
      padding: 4px 12px; border-radius: 20px; font-size: 0.7rem; font-weight: 600;
      &[data-status="Pending"] { background: rgba(255,191,0,0.12); color: #FFD54F; }
      &[data-status="Dispatched"] { background: rgba(41,182,246,0.12); color: #4FC3F7; }
      &[data-status="InTransit"] { background: rgba(171,71,188,0.12); color: #CE93D8; }
      &[data-status="OutForDelivery"] { background: rgba(255,152,0,0.12); color: #FFB74D; }
      &[data-status="Delivered"] { background: rgba(76,175,80,0.12); color: #66BB6A; }
    }
    .delivery-body { display: flex; flex-direction: column; gap: 8px; margin-bottom: 16px; }
    .info-row { display: flex; justify-content: space-between; align-items: center; }
    .info-label { font-family: 'Inter', sans-serif; font-size: 0.75rem; color: rgba(253,252,245,0.35); }
    .info-val { font-family: 'Inter', sans-serif; font-size: 0.85rem; color: rgba(253,252,245,0.7); }
    .mono { font-family: 'JetBrains Mono', monospace; }
    .delivery-actions { padding-top: 12px; border-top: 1px solid rgba(255,255,255,0.05); }
    .status-select {
      background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); border-radius: 8px;
      color: rgba(253,252,245,0.7); padding: 8px 12px; font-size: 0.8rem; outline: none; cursor: pointer; width: 100%;
      option { background: #1a1a1a; }
      &:focus { border-color: rgba(184,115,51,0.5); }
    }
    .empty-state { text-align: center; padding: 60px; }
    .empty-icon { font-size: 40px; opacity: 0.3; margin-bottom: 12px; }
    h3 { font-family: 'Playfair Display', serif; font-weight: 600; color: #FDFCF5; margin-bottom: 8px; }
    .btn-outline { background: transparent; border: 1px solid rgba(184,115,51,0.5); color: #B87333; padding: 8px 12px; border-radius: 6px; cursor: pointer; width: 100%; margin-bottom: 8px; font-weight: 600; transition: all 0.2s; &:hover { background: rgba(184,115,51,0.1); } }
    
    .modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.7); backdrop-filter: blur(4px); display: flex; align-items: center; justify-content: center; z-index: 1000; }
    .modal-content { background: #1E120A; border: 1px solid rgba(184,115,51,0.3); padding: 24px; border-radius: 12px; width: 500px; max-width: 90%; color: #fff; box-shadow: 0 10px 40px rgba(0,0,0,0.8); }
    .modal-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
    .modal-header h2 { margin: 0; color: #FDFCF5; font-family: 'Playfair Display', serif; }
    .btn-close { background: none; border: none; color: #8A7362; font-size: 1.2rem; cursor: pointer; }
    .detail-grid { display: grid; gap: 16px; margin-bottom: 20px; }
    .info-group label { display: block; font-size: 0.75rem; color: #8A7362; text-transform: uppercase; letter-spacing: 0.05em; margin-bottom: 4px; }
    .info-group p { margin: 0; font-size: 0.9rem; color: #E5D3B3; line-height: 1.4; }
    .agent-form { display: flex; gap: 8px; margin-top: 10px; flex-wrap: wrap; }
    .form-input { flex: 1; min-width: 150px; background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); border-radius: 6px; color: #fff; padding: 10px; font-size: 0.85rem; outline: none; &:focus { border-color: #B87333; } }
    .btn-submit { background: #B87333; color: #fff; border: none; padding: 10px 16px; border-radius: 6px; cursor: pointer; font-weight: 600; white-space: nowrap; }
    .btn-text-edit { background: none; border: none; color: #B87333; font-size: 0.7rem; cursor: pointer; text-decoration: underline; font-weight: 600; }
    .address-edit-form { display: flex; flex-direction: column; gap: 8px; margin-top: 8px; }
    .full-width { width: 100%; box-sizing: border-box; }
    .form-row-small { display: flex; gap: 8px; }
    .form-actions-small { display: flex; justify-content: flex-end; gap: 8px; margin-top: 4px; }
    .btn-cancel-small { background: transparent; border: 1px solid #8A7362; color: #E5D3B3; padding: 4px 10px; border-radius: 4px; cursor: pointer; font-size: 0.75rem; }
    .btn-submit-small { background: #B87333; color: #fff; border: none; padding: 4px 10px; border-radius: 4px; cursor: pointer; font-size: 0.75rem; font-weight: 600; }
  `]
})
export class AdminDeliveriesComponent implements OnInit {
  deliveries$!: Observable<Delivery[]>;
  deliveriesCache: Delivery[] = [];
  loading$!: Observable<boolean>;
  searchOrderId = '';
  searchDeliveryStatus = '';
  uniqueOrderIds: string[] = [];

  selectedDelivery: Delivery | null = null;
  agentName = '';
  agentPhone = '';

  editAddressMode = false;
  editAddress = {
    deliveryAddress: '',
    city: '',
    state: '',
    pinCode: ''
  };

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.deliveries$ = this.store.select(selectAllDeliveries);
    this.loading$ = this.store.select(selectDeliveriesLoading);
    this.store.dispatch(DeliveryActions.loadDeliveries({}));

    this.deliveries$.subscribe(deliveries => {
      if (deliveries) {
        this.deliveriesCache = deliveries;
        const ids = deliveries.map(d => d.orderId).filter(id => !!id);
        this.uniqueOrderIds = [...new Set(ids)];
      }
    });
  }

  get filteredDeliveries(): Delivery[] {
    let filtered = this.deliveriesCache;
    
    if (this.searchDeliveryStatus) {
      filtered = filtered.filter(d => d.status === this.searchDeliveryStatus);
    }
    
    if (this.searchOrderId) {
      const term = this.searchOrderId.toLowerCase();
      filtered = filtered.filter(d => 
        d.orderId.toLowerCase().includes(term) || 
        d.id.toLowerCase().includes(term)
      );
    }
    
    return filtered;
  }

  openDetails(d: Delivery): void {
    this.selectedDelivery = d;
    this.agentName = d.assignedAgent || '';
    this.agentPhone = d.agentPhone || '';
    this.editAddressMode = false;
  }

  closeDetails(): void {
    this.selectedDelivery = null;
    this.editAddressMode = false;
  }

  toggleEditAddress(): void {
    if (!this.selectedDelivery) return;
    this.editAddressMode = !this.editAddressMode;
    if (this.editAddressMode) {
      this.editAddress = {
        deliveryAddress: this.selectedDelivery.deliveryAddress === 'Address Pending' ? '' : this.selectedDelivery.deliveryAddress,
        city: this.selectedDelivery.city === 'Pending' ? '' : this.selectedDelivery.city,
        state: this.selectedDelivery.state === 'Pending' ? '' : this.selectedDelivery.state,
        pinCode: this.selectedDelivery.pinCode === '000000' ? '' : this.selectedDelivery.pinCode
      };
    }
  }

  saveAddress(): void {
    if (!this.selectedDelivery) return;
    this.store.dispatch(DeliveryActions.updateDeliveryAddress({
      id: this.selectedDelivery.id,
      request: this.editAddress
    }));
    this.editAddressMode = false;
    const updatedDelivery = { 
      ...this.selectedDelivery, 
      ...this.editAddress,
      deliveryAddress: this.editAddress.deliveryAddress || 'Address Pending',
      city: this.editAddress.city || 'Pending',
      state: this.editAddress.state || 'Pending',
      pinCode: this.editAddress.pinCode || '000000'
    };
    this.selectedDelivery = updatedDelivery;
    // Update cache so reopening modal shows updated data immediately
    const idx = this.deliveriesCache.findIndex(d => d.id === updatedDelivery.id);
    if (idx !== -1) {
      this.deliveriesCache[idx] = updatedDelivery;
    }
  }

  saveAgentAssignment(): void {
    if (!this.selectedDelivery || !this.agentName) return;
    
    // Validate 10-digit phone number
    if (this.agentPhone && !/^\d{10}$/.test(this.agentPhone)) {
      alert('Please enter a valid 10-digit phone number for the agent.');
      return;
    }

    this.store.dispatch(DeliveryActions.assignAgent({
      id: this.selectedDelivery.id,
      request: { agentName: this.agentName, agentPhone: this.agentPhone }
    }));
    // Optimistically update so reopening shows the saved data
    const updatedDelivery = {
      ...this.selectedDelivery,
      assignedAgent: this.agentName,
      agentPhone: this.agentPhone
    };
    this.selectedDelivery = updatedDelivery;
    const idx = this.deliveriesCache.findIndex(d => d.id === updatedDelivery.id);
    if (idx !== -1) {
      this.deliveriesCache[idx] = updatedDelivery;
    }
  }

  updateStatus(id: string, event: Event): void {
    const status = (event.target as HTMLSelectElement).value;
    if (!status) return;
    const statusMap: Record<string, number> = {
      'Assigned': 1, 'InProcess': 2, 'Dispatched': 3, 'Delivered': 4
    };
    const newStatus = statusMap[status];
    if (newStatus === undefined) return;
    this.store.dispatch(DeliveryActions.updateDeliveryStatus({ id, request: { newStatus } }));
  }
}
