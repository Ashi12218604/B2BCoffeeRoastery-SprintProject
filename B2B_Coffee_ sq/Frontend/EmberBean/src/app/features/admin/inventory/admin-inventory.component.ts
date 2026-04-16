import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { InventoryItem } from '../../../shared/models/inventory.model';
import { selectUser } from '../../../store/auth/auth.selectors';
import { selectAllInventory, selectInventoryLoading, selectInventoryError } from '../../../store/inventory/inventory.selectors';
import * as InventoryActions from '../../../store/inventory/inventory.actions';

@Component({
  selector: 'app-admin-inventory',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="inv-page" *ngIf="(user$ | async) as user">
      <div class="page-header animate-fade-in-up">
        <div>
          <h1 class="page-title">Inventory <span class="highlight">Control</span></h1>
          <p class="page-sub">Monitor stock levels and manage restocking</p>
        </div>
        <div style="display: flex; gap: 15px; align-items: center;">
          <div class="error-badge" *ngIf="error$ | async as error">{{ error }}</div>
          <label class="low-stock-toggle">
            <input type="checkbox" [(ngModel)]="lowStockOnly" (change)="reload()"/>
            <span>Low Stock Only</span>
          </label>
        </div>
      </div>
      <div class="loading-state" *ngIf="loading$ | async"><div class="spinner"></div></div>
      
      <div class="inv-grid" *ngIf="(inventory$ | async) as items">
        <div class="inv-card" *ngFor="let item of items" [class.low-stock]="item.quantityAvailable <= item.lowStockThreshold" id="inv-{{ item.productId }}">
          <div class="inv-header">
            <span class="inv-product">{{ item.productName || 'Product' }}</span>
            <div style="display: flex; gap: 8px; align-items: center;">
              <span class="stock-badge" [class.danger]="item.quantityAvailable <= item.lowStockThreshold" [class.ok]="item.quantityAvailable > item.lowStockThreshold">
                {{ item.quantityAvailable }} units
              </span>
              <button class="btn-icon edit-btn" *ngIf="user.role === 'SuperAdmin'" (click)="openOverrideModal(item)" title="Override Inventory Rules">⚙️</button>
            </div>
          </div>
          <div class="inv-bar-wrapper">
            <div class="inv-bar" [style.width.%]="getStockPercent(item)" [class.bar-danger]="item.quantityAvailable <= item.lowStockThreshold"></div>
          </div>
          <div class="inv-meta">
            <span>Reorder Level: {{ item.lowStockThreshold }}</span>
            <span>Last Updated: {{ item.updatedAt | date:'MMM dd, HH:mm' }}</span>
          </div>
          
          <div class="restock-row">
            <input type="number" [(ngModel)]="restockQty[item.productId]" placeholder="Qty" class="restock-input" min="1"/>
            <button class="btn-restock" (click)="restock(item.productId)" [disabled]="!restockQty[item.productId]">
              Restock
            </button>
          </div>
        </div>
        <div class="empty-state" *ngIf="items.length === 0 && !(loading$ | async)">
          <div class="empty-icon">📦</div><h3>No inventory items</h3>
        </div>
      </div>

      <!-- Override Modal for SuperAdmin -->
      <div class="modal-overlay" *ngIf="showOverrideModal">
        <div class="modal-content animate-fade-in-up">
          <h2>Override Inventory Rules</h2>
          <p class="modal-sub"><strong>{{ overrideItem?.productName }}</strong></p>
          
          <div class="form-group" style="margin-top: 20px;">
            <label>Manual Quantity Available</label>
            <input type="number" [(ngModel)]="overrideData.quantityAvailable" class="override-input" />
            <small class="form-hint">Directly adjusts the system record of current stock.</small>
          </div>
          <div class="form-group">
            <label>Reorder Level (Threshold)</label>
            <input type="number" [(ngModel)]="overrideData.lowStockThreshold" class="override-input" />
            <small class="form-hint">Triggers low stock warnings when availability falls to this line or below.</small>
          </div>
          
          <div class="modal-actions" style="margin-top: 24px; display: flex; justify-content: flex-end; gap: 12px;">
            <button class="btn-cancel" (click)="closeOverrideModal()">Cancel</button>
            <button class="btn-submit" (click)="submitOverride()">Save Rule</button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .inv-page { max-width: 1200px; }
    .page-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 24px; }
    .page-title { font-family: 'Playfair Display', serif; font-size: 1.875rem; font-weight: 600; color: #FDFCF5; margin-bottom: 6px; }
    .highlight { color: #B87333; }
    .page-sub { font-family: 'Inter', sans-serif; font-size: 0.875rem; color: #FDF5E6; text-shadow: 0 1px 4px rgba(0,0,0,0.3); }
    .low-stock-toggle { display: flex; align-items: center; gap: 8px; font-family: 'Inter', sans-serif; font-size: 0.85rem; color: rgba(253,252,245,0.6); cursor: pointer;
      input { accent-color: #B87333; }
    }
    .loading-state { display: flex; justify-content: center; padding: 60px 0; }
    .spinner { width: 36px; height: 36px; border: 3px solid rgba(184,115,51,0.2); border-top: 3px solid #B87333; border-radius: 50%; animation: spin 0.8s linear infinite; }
    .inv-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 20px; }
    .inv-card {
      background: rgba(30,30,30,0.4); backdrop-filter: blur(20px); border: 1px solid rgba(184,115,51,0.15);
      border-radius: 12px; padding: 20px; transition: all 0.3s cubic-bezier(0.4,0,0.2,1);
      &:hover { border-color: rgba(184,115,51,0.35); transform: translateY(-2px); }
      &.low-stock { border-color: rgba(244,67,54,0.35); }
    }
    .inv-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 14px; }
    .inv-product { font-family: 'Playfair Display', serif; font-size: 1rem; font-weight: 600; color: #FDFCF5; }
    
    .edit-btn { background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); border-radius: 6px; padding: 2px 6px; cursor: pointer; transition: 0.2s; }
    .edit-btn:hover { background: rgba(184,115,51,0.2); border-color: rgba(184,115,51,0.5); transform: scale(1.05); }

    .stock-badge { padding: 4px 12px; border-radius: 20px; font-size: 0.72rem; font-weight: 600;
      &.ok { background: rgba(76,175,80,0.12); color: #81C784; }
      &.danger { background: rgba(244,67,54,0.12); color: #EF5350; }
    }
    .inv-bar-wrapper { height: 6px; background: rgba(255,255,255,0.06); border-radius: 3px; margin-bottom: 12px; overflow: hidden; }
    .inv-bar { height: 100%; background: linear-gradient(90deg, #B87333, #D4A76A); border-radius: 3px; transition: width 0.5s; min-width: 2%;
      &.bar-danger { background: linear-gradient(90deg, #EF5350, #E57373); }
    }
    .inv-meta { display: flex; justify-content: space-between; font-size: 0.72rem; color: rgba(253,252,245,0.35); margin-bottom: 14px; }
    .restock-row { display: flex; gap: 8px; }
    .restock-input {
      flex: 1; background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1);
      border-radius: 8px; color: #FDFCF5; padding: 8px 12px; font-size: 0.85rem; outline: none;
      &:focus { border-color: rgba(184,115,51,0.5); }
    }
    .btn-restock {
      padding: 8px 18px; border-radius: 8px; background: rgba(76,175,80,0.15); color: #81C784;
      border: 1px solid rgba(76,175,80,0.3); font-size: 0.8rem; font-weight: 600; cursor: pointer;
      transition: all 0.2s;
      &:hover:not(:disabled) { background: rgba(76,175,80,0.25); }
      &:disabled { opacity: 0.4; cursor: not-allowed; }
    }
    .empty-state { grid-column: 1 / -1; text-align: center; padding: 60px; }
    .empty-icon { font-size: 40px; opacity: 0.3; margin-bottom: 12px; }
    h3 { font-family: 'Playfair Display', serif; font-weight: 600; color: #FDFCF5; }

    /* Override Modal Styles */
    .modal-overlay { position: fixed; inset: 0; background: rgba(15, 10, 5, 0.8); backdrop-filter: blur(4px); display: flex; align-items: center; justify-content: center; z-index: 1000; }
    .modal-content { background: rgba(35, 25, 20, 0.95); border: 1px solid rgba(184, 115, 51, 0.4); border-radius: 16px; padding: 32px; width: 100%; max-width: 440px; color: #FDFCF5; box-shadow: 0 20px 40px rgba(0,0,0,0.5); }
    .modal-content h2 { font-family: 'Playfair Display', serif; font-size: 1.5rem; margin-bottom: 4px; }
    .modal-sub { color: #B87333; font-size: 0.85rem; margin-bottom: 20px; }
    .form-group { display: flex; flex-direction: column; gap: 8px; margin-bottom: 16px; }
    .form-group label { font-size: 0.8rem; font-weight: 600; color: rgba(253, 245, 230, 0.8); text-transform: uppercase; letter-spacing: 0.05em; }
    .override-input { background: rgba(20, 15, 10, 0.6); border: 1px solid rgba(184, 115, 51, 0.3); border-radius: 8px; padding: 12px; color: #FDFCF5; font-size: 1rem; outline: none; transition: border-color 0.2s; }
    .override-input:focus { border-color: #B87333; box-shadow: 0 0 0 2px rgba(184, 115, 51, 0.2); }
    .form-hint { font-size: 0.72rem; color: rgba(253, 245, 230, 0.5); margin-top: 4px; }
    .btn-cancel { background: transparent; border: 1px solid rgba(255,255,255,0.2); color: #FDFCF5; padding: 10px 20px; border-radius: 8px; cursor: pointer; transition: 0.2s; font-weight: 500; }
    .btn-cancel:hover { background: rgba(255,255,255,0.1); }
    .btn-submit { background: #B87333; border: none; color: #fff; padding: 10px 20px; border-radius: 8px; cursor: pointer; font-weight: 600; transition: 0.2s; box-shadow: 0 4px 12px rgba(184, 115, 51, 0.3); }
    .btn-submit:hover { background: #D18F52; transform: translateY(-1px); }
  `]
})
export class AdminInventoryComponent implements OnInit {
  inventory$!: Observable<InventoryItem[]>;
  loading$!: Observable<boolean>;
  error$!: Observable<string | null>;
  user$!: Observable<any>;
  lowStockOnly = false;
  restockQty: Record<string, number> = {};

  showOverrideModal = false;
  overrideItem: InventoryItem | null = null;
  overrideData = { quantityAvailable: 0, lowStockThreshold: 10 };

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.user$ = this.store.select(selectUser);
    this.inventory$ = this.store.select(selectAllInventory);
    this.loading$ = this.store.select(selectInventoryLoading);
    this.error$ = this.store.select(selectInventoryError);
    this.reload();
  }

  reload(): void {
    this.store.dispatch(InventoryActions.loadInventory({ lowStockOnly: this.lowStockOnly }));
  }

  getStockPercent(item: InventoryItem): number {
    const max = Math.max(item.quantityAvailable, item.lowStockThreshold * 3, 100);
    return Math.min((item.quantityAvailable / max) * 100, 100);
  }

  restock(productId: string): void {
    const qty = this.restockQty[productId];
    if (!qty || qty <= 0) return;
    this.store.dispatch(InventoryActions.restockInventory({
      productId,
      request: { quantity: qty, reason: 'Refilled directly via roastery inventory' }
    }));
    this.restockQty[productId] = 0;
  }

  openOverrideModal(item: InventoryItem): void {
    this.overrideItem = item;
    this.overrideData = {
      quantityAvailable: item.quantityAvailable,
      lowStockThreshold: item.lowStockThreshold
    };
    this.showOverrideModal = true;
  }

  closeOverrideModal(): void {
    this.showOverrideModal = false;
    this.overrideItem = null;
  }

  submitOverride(): void {
    if (this.overrideItem) {
      this.store.dispatch(InventoryActions.upsertInventory({
        request: {
          productId: this.overrideItem.productId,
          productName: this.overrideItem.productName,
          sku: this.overrideItem.sku,
          quantityAvailable: this.overrideData.quantityAvailable,
          lowStockThreshold: this.overrideData.lowStockThreshold
        }
      }));
    }
    this.closeOverrideModal();
  }
}

