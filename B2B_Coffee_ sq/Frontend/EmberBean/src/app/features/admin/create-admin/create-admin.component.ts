import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="create-admin-container">
      <div class="header-section">
        <h1 class="page-title">Manage Admins</h1>
        <p class="page-subtitle">Create new administrator accounts for the platform and view existing ones.</p>
      </div>

      <div class="split-layout">
        <!-- Form Section -->
        <div class="form-card">
          <h2 class="form-title">Create Admin Account</h2>
          <p class="form-desc">The new admin will have access to manage products, orders, and clients.</p>

          <form [formGroup]="adminForm" (ngSubmit)="onSubmit()">
            
            <div class="form-row">
              <div class="form-group">
                <label>Full Name</label>
                <input type="text" formControlName="fullName" placeholder="John Doe">
              </div>
              <div class="form-group">
                <label>Email Address</label>
                <input type="email" formControlName="email" placeholder="admin@emberandbean.com">
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label>Phone Number</label>
                <input type="text" formControlName="phoneNumber" placeholder="10-digit number" maxlength="10">
              </div>
              <div class="form-group">
                <label>Temporary Password</label>
                <input type="password" formControlName="password" placeholder="Min 8 chars, 1 uppercase, 1 symbol">
              </div>
            </div>

            <div class="form-actions">
              <button type="submit" class="btn-primary" [disabled]="adminForm.invalid || loading">
                <span *ngIf="!loading">Create Administrator</span>
                <span *ngIf="loading">Creating...</span>
              </button>
              <div class="error-msg" *ngIf="errorMsg">{{ errorMsg }}</div>
              <div class="success-msg" *ngIf="successMsg">{{ successMsg }}</div>
            </div>

          </form>
        </div>

        <!-- Admins List Section -->
        <div class="admins-list-card">
          <h2 class="form-title">Existing Admins</h2>
          <p class="form-desc">List of all active administrator accounts.</p>

          <div *ngIf="loadingAdmins" class="loading-state">Loading admins...</div>
          <div *ngIf="!loadingAdmins && admins.length === 0" class="empty-state">No admins found.</div>
          
          <div class="admins-list" *ngIf="!loadingAdmins && admins.length > 0">
            <div class="admin-item" *ngFor="let admin of admins">
              <div class="admin-info">
                <div class="admin-name">{{ admin.fullName }}</div>
                <div class="admin-details">
                  <span class="admin-email">{{ admin.email }}</span> • 
                  <span class="admin-date">Created on {{ admin.createdAt | date }}</span>
                </div>
              </div>
              <button class="btn-delete" (click)="deleteAdmin(admin.id)">Delete</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .create-admin-container { max-width: 1200px; }
    .header-section { margin-bottom: 32px; }
    .page-title { font-family: 'Playfair Display', serif; font-size: 2rem; font-weight: 600; color: #FDFCF5; margin-bottom: 8px; }
    .page-subtitle { font-family: 'Inter', sans-serif; font-size: 0.875rem; color: #FDF5E6; }

    .split-layout { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; align-items: start; }
    @media(max-width: 900px) { .split-layout { grid-template-columns: 1fr; } }

    .form-card, .admins-list-card {
      background: rgba(30,30,30,0.4); border: 1px solid rgba(184,115,51,0.2); border-radius: 12px;
      padding: 32px; backdrop-filter: blur(20px);
    }
    .form-title { font-family: 'Playfair Display', serif; font-size: 1.5rem; color: #FDFCF5; margin: 0 0 8px 0; }
    .form-desc { font-family: 'Inter', sans-serif; font-size: 0.875rem; color: rgba(253,252,245,0.5); margin-bottom: 32px; }

    .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; margin-bottom: 24px; }
    @media(max-width: 600px) { .form-row { grid-template-columns: 1fr; } }
    
    .form-group label { display: block; font-family: 'Inter', sans-serif; font-size: 0.75rem; font-weight: 600; letter-spacing: 0.05em; color: rgba(184,115,51,0.9); text-transform: uppercase; margin-bottom: 8px; }
    
    input {
      width: 100%; background: rgba(0,0,0,0.2); border: 1px solid rgba(184,115,51,0.2); border-radius: 8px;
      padding: 12px 16px; color: #FDFCF5; font-family: 'Inter', sans-serif; font-size: 0.95rem; transition: all 0.2s;
    }
    input:focus { outline: none; border-color: rgba(184,115,51,0.6); background: rgba(0,0,0,0.3); box-shadow: 0 0 0 3px rgba(184,115,51,0.1); }
    input::placeholder { color: rgba(253,252,245,0.2); }

    .form-actions { margin-top: 32px; display: flex; flex-direction: column; align-items: flex-start; gap: 16px; }
    .btn-primary { 
      background: linear-gradient(135deg, #b87333, #8c5936); color: #FDFCF5; border: none; border-radius: 8px;
      padding: 14px 28px; font-family: 'Inter', sans-serif; font-size: 0.95rem; font-weight: 600; cursor: pointer;
      transition: all 0.2s; box-shadow: 0 4px 15px rgba(184,115,51,0.2);
    }
    .btn-primary:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 8px 25px rgba(184,115,51,0.3); background: linear-gradient(135deg, #c98544, #9c6643); }
    .btn-primary:disabled { opacity: 0.5; cursor: not-allowed; }

    .error-msg { color: #f44336; font-size: 0.875rem; }
    .success-msg { color: #4CAF50; font-size: 0.875rem; }

    .admins-list { display: flex; flex-direction: column; gap: 16px; }
    .admin-item {
      padding: 16px;
      background: rgba(255,255,255,0.03);
      border: 1px solid rgba(255,255,255,0.05);
      border-radius: 8px;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    .btn-delete {
      background: rgba(244,67,54,0.1); color: #EF5350; border: 1px solid rgba(244,67,54,0.3);
      border-radius: 6px; padding: 6px 12px; cursor: pointer; transition: 0.2s;
    }
    .btn-delete:hover { background: rgba(244,67,54,0.2); }
    .admin-name { font-family: 'Inter', sans-serif; font-weight: 600; font-size: 1rem; color: #FDFCF5; margin-bottom: 4px; }
    .admin-details { font-family: 'Inter', sans-serif; font-size: 0.85rem; color: rgba(253,252,245,0.6); }
    .loading-state, .empty-state { font-family: 'Inter', sans-serif; font-size: 0.9rem; color: rgba(253,252,245,0.5); }
  `]
})
export class CreateAdminComponent implements OnInit {
  private fb = inject(FormBuilder);
  private http = inject(HttpClient);
  private router = inject(Router);
  
  adminForm: FormGroup = this.fb.group({
    fullName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  loading = false;
  errorMsg = '';
  successMsg = '';

  admins: any[] = [];
  loadingAdmins = false;

  ngOnInit() {
    this.fetchAdmins();
  }

  deleteAdmin(id: string) {
    if (!confirm('Are you sure you want to delete this admin account?')) return;
    this.http.delete('http://localhost:7101/api/admin/admins/' + id)
      .subscribe({
        next: () => {
          this.successMsg = 'Admin deleted successfully!';
          this.fetchAdmins();
        },
        error: (err) => {
          this.errorMsg = err.error?.message || 'Failed to delete admin.';
        }
      });
  }

  fetchAdmins() {
    this.loadingAdmins = true;
    this.http.get<any[]>('http://localhost:7101/api/admin/admins')
      .subscribe({
        next: (res) => {
          this.admins = res;
          this.loadingAdmins = false;
        },
        error: () => {
          this.loadingAdmins = false;
        }
      });
  }

  onSubmit() {
    if (this.adminForm.invalid) return;
    this.loading = true;
    this.errorMsg = '';
    this.successMsg = '';

    this.http.post('http://localhost:7101/api/admin/create-admin', this.adminForm.value)
      .subscribe({
        next: () => {
          this.loading = false;
          this.successMsg = 'Admin created successfully!';
          this.adminForm.reset();
          this.fetchAdmins(); // Refresh list after creating
        },
        error: (err) => {
          this.loading = false;
          this.errorMsg = err.error?.message || 'Failed to create admin. Only SuperAdmin can perform this action.';
        }
      });
  }
}

