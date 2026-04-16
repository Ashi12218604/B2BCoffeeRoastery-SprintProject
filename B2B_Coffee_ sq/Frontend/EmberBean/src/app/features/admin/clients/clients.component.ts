import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface Client {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  companyName: string;
  approvedAt: string;
}

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="clients-container">
      <div class="header-section">
        <div>
          <h1 class="page-title">Approved Clients</h1>
          <p class="page-subtitle">Manage your active partner accounts.</p>
        </div>
      </div>

      <div class="loading-state" *ngIf="loading">
        <div class="brew-spinner"></div>
        <p>Loading clients...</p>
      </div>

      <div class="empty-state" *ngIf="!loading && clients.length === 0">
        <div class="empty-icon">👥</div>
        <h3>No Approved Clients</h3>
        <p>You don't have any active clients yet.</p>
      </div>

      <div class="clients-grid" *ngIf="!loading && clients.length > 0">
        <div class="client-card" *ngFor="let client of clients">
          <div class="card-header">
            <div class="client-avatar">{{ getInitials(client.fullName) }}</div>
            <div class="client-basic">
              <h3 class="client-name">{{ client.fullName }}</h3>
              <p class="company-name">{{ client.companyName || 'Independent' }}</p>
            </div>
            <div class="status-badge approved">Active</div>
          </div>
          
          <div class="card-body">
            <div class="info-row"><span class="icon">✉️</span>{{ client.email }}</div>
            <div class="info-row"><span class="icon">📞</span>{{ client.phoneNumber }}</div>
            <div class="info-row"><span class="icon">📅</span>Since {{ client.approvedAt | date:'mediumDate' }}</div>
          </div>

          <div class="card-actions">
            <button class="btn-reject" (click)="terminateClient(client.id)" [disabled]="actionLoading === client.id">
              <span *ngIf="actionLoading !== client.id">Terminate Account</span>
              <span *ngIf="actionLoading === client.id">Terminating...</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .clients-container { max-width: 1200px; }
    .header-section { margin-bottom: 32px; }
    .page-title { font-family: 'Playfair Display', serif; font-size: 2rem; font-weight: 600; color: #FDFCF5; margin-bottom: 8px; }
    .page-subtitle { font-family: 'Inter', sans-serif; font-size: 0.875rem; color: #FDF5E6; }

    .loading-state, .empty-state {
      display: flex; flex-direction: column; align-items: center; justify-content: center;
      padding: 64px 0; background: rgba(30,30,30,0.4); border: 1px dashed rgba(184,115,51,0.3);
      border-radius: 12px; backdrop-filter: blur(20px); text-align: center;
    }
    .empty-icon { font-size: 48px; margin-bottom: 16px; opacity: 0.5; }
    .empty-state h3 { font-family: 'Playfair Display', serif; font-size: 1.5rem; color: #FDFCF5; margin-bottom: 8px; }
    .empty-state p { font-family: 'Inter', sans-serif; font-size: 0.875rem; color: rgba(253,252,245,0.5); }

    .clients-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(340px, 1fr)); gap: 24px; }
    .client-card {
      background: rgba(30,30,30,0.4); border: 1px solid rgba(184,115,51,0.2); border-radius: 12px;
      padding: 24px; backdrop-filter: blur(20px); display: flex; flex-direction: column; gap: 20px;
      transition: all 0.3s ease;
    }
    .client-card:hover { border-color: rgba(184,115,51,0.5); transform: translateY(-2px); box-shadow: 0 8px 30px rgba(0,0,0,0.2); }

    .card-header { display: flex; align-items: flex-start; gap: 16px; position: relative; }
    .client-avatar { width: 48px; height: 48px; border-radius: 50%; background: linear-gradient(135deg, #b87333, #8c5936); display: flex; align-items: center; justify-content: center; font-family: 'Inter', sans-serif; font-weight: 600; font-size: 1.1rem; color: #FDFCF5; }
    .client-basic { flex: 1; }
    .client-name { font-family: 'Inter', sans-serif; font-size: 1.1rem; font-weight: 600; color: #FDFCF5; margin: 0 0 4px 0; }
    .company-name { font-family: 'Inter', sans-serif; font-size: 0.8125rem; color: rgba(184,115,51,0.8); margin: 0; font-weight: 500; }
    
    .status-badge { position: absolute; top: 0; right: 0; padding: 4px 8px; border-radius: 100px; font-size: 0.6875rem; font-weight: 600; font-family: 'Inter', sans-serif; }
    .status-badge.approved { background: rgba(76,175,80,0.15); color: #4CAF50; border: 1px solid rgba(76,175,80,0.3); }

    .card-body { display: flex; flex-direction: column; gap: 12px; padding-top: 16px; border-top: 1px solid rgba(184,115,51,0.1); }
    .info-row { display: flex; align-items: center; gap: 10px; font-family: 'Inter', sans-serif; font-size: 0.875rem; color: rgba(253,252,245,0.7); }
    .icon { opacity: 0.7; font-size: 1rem; }

    .card-actions { display: flex; gap: 12px; padding-top: 16px; border-top: 1px solid rgba(184,115,51,0.1); margin-top: auto; }
    .btn-reject { flex: 1; padding: 10px 16px; border-radius: 8px; font-family: 'Inter', sans-serif; font-size: 0.8125rem; font-weight: 600; cursor: pointer; transition: all 0.2s ease; display: inline-flex; justify-content: center; align-items: center; background: rgba(244,67,54,0.1); color: #F44336; border: 1px solid rgba(244,67,54,0.3); }
    .btn-reject:hover:not(:disabled) { background: rgba(244,67,54,0.2); border-color: #F44336; }
    .btn-reject:disabled { opacity: 0.5; cursor: not-allowed; }
  `]
})
export class ClientsComponent implements OnInit {
  private http = inject(HttpClient);
  private apiBase = 'http://localhost:7101/api/admin';

  clients: Client[] = [];
  loading = true;
  actionLoading: string | null = null;

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.loading = true;
    this.http.get<Client[]>(`${this.apiBase}/clients`)
      .subscribe({
        next: (clients) => { this.clients = clients; this.loading = false; },
        error: () => { this.loading = false; }
      });
  }

  terminateClient(id: string): void {
    if (!confirm('Are you sure you want to terminate this client account?')) return;
    this.actionLoading = id;
    this.http.put(`${this.apiBase}/terminate-client/${id}`, {})
      .subscribe({
        next: () => {
          this.clients = this.clients.filter(c => c.id !== id);
          this.actionLoading = null;
        },
        error: () => { this.actionLoading = null; alert('Failed to terminate client.'); }
      });
  }

  getInitials(name: string): string {
    return name ? name.split(' ').map(n => n[0]).join('').toUpperCase().substring(0, 2) : 'CL';
  }
}
