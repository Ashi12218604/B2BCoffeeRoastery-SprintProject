import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface PendingClient {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  companyName: string;
  status: string;
  createdAt: string;
}

@Component({
  selector: 'app-pending-clients',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pending-clients.component.html',
  styleUrls: ['./pending-clients.component.scss']
})
export class PendingClientsComponent implements OnInit {
  private http = inject(HttpClient);
  private apiBase = 'http://localhost:7101/api/admin';

  clients: PendingClient[] = [];
  loading = true;
  actionLoading: string | null = null;
  rejectionReason = '';
  rejectingClientId: string | null = null;

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.loading = true;
    this.http.get<PendingClient[]>(`${this.apiBase}/pending-clients`)
      .subscribe({
        next: (clients) => { this.clients = clients; this.loading = false; },
        error: () => { this.loading = false; }
      });
  }

  playChime() {
    try {
      const AudioContextClass = window.AudioContext || (window as any).webkitAudioContext;
      if (!AudioContextClass) return;
      const ctx = new AudioContextClass();
      const osc = ctx.createOscillator();
      const gain = ctx.createGain();
      osc.connect(gain);
      gain.connect(ctx.destination);
      osc.type = 'triangle';
      osc.frequency.setValueAtTime(1200, ctx.currentTime);
      gain.gain.setValueAtTime(0.2, ctx.currentTime);
      gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 1.2);
      osc.start();
      osc.stop(ctx.currentTime + 1.2);
    } catch(e) {}
  }

  approveClient(clientId: string): void {
    this.actionLoading = clientId;
    this.http.put(`${this.apiBase}/approve-client/${clientId}`, { approve: true })
      .subscribe({
        next: () => {
          this.playChime();
          this.clients = this.clients.filter(c => c.id !== clientId);
          this.actionLoading = null;
        },
        error: () => { this.actionLoading = null; }
      });
  }

  showRejectModal(clientId: string): void {
    this.rejectingClientId = clientId;
    this.rejectionReason = '';
  }

  cancelReject(): void {
    this.rejectingClientId = null;
    this.rejectionReason = '';
  }

  confirmReject(): void {
    if (!this.rejectingClientId) return;
    const id = this.rejectingClientId;
    this.actionLoading = id;
    this.http.put(`${this.apiBase}/approve-client/${id}`, {
      approve: false,
      rejectionReason: this.rejectionReason || 'Application does not meet requirements.'
    }).subscribe({
      next: () => {
        this.clients = this.clients.filter(c => c.id !== id);
        this.rejectingClientId = null;
        this.actionLoading = null;
      },
      error: () => { this.actionLoading = null; }
    });
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().substring(0, 2);
  }

  getTimeSince(date: string): string {
    const ms = Date.now() - new Date(date).getTime();
    const hours = Math.floor(ms / 3600000);
    if (hours < 1) return 'Just now';
    if (hours < 24) return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    return `${days}d ago`;
  }
}
