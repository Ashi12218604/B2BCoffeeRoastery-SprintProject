import { Routes } from '@angular/router';
import { authGuard, adminGuard, clientGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // ── Auth Routes (Public) ────────────────────────────────
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent),
    title: 'Sign In | Ember & Bean'
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent),
    title: 'Request Access | Ember & Bean'
  },
  {
    path: 'verify-otp',
    loadComponent: () => import('./features/auth/verify-otp/verify-otp.component').then(m => m.VerifyOtpComponent),
    title: 'Verify Email | Ember & Bean'
  },

  // ── Client Routes ───────────────────────────────────────
  {
    path: 'client',
    loadComponent: () => import('./features/client/client-layout/client-layout.component').then(m => m.ClientLayoutComponent),
    canActivate: [authGuard, clientGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/client/dashboard/client-dashboard.component').then(m => m.ClientDashboardComponent),
        title: 'Dashboard | Ember & Bean'
      },
      {
        path: 'catalog',
        loadComponent: () => import('./features/client/catalog/catalog.component').then(m => m.CatalogComponent),
        title: 'Catalog | Ember & Bean'
      },
      {
        path: 'orders',
        loadComponent: () => import('./features/client/my-orders/my-orders.component').then(m => m.MyOrdersComponent),
        title: 'My Orders | Ember & Bean'
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },

  // ── Admin Routes ────────────────────────────────────────
  {
    path: 'admin',
    loadComponent: () => import('./features/admin/admin-layout/admin-layout.component').then(m => m.AdminLayoutComponent),
    canActivate: [authGuard, adminGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/admin/dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent),
        title: 'Admin Dashboard | Ember & Bean'
      },
      {
        path: 'pending-clients',
        loadComponent: () => import('./features/admin/pending-clients/pending-clients.component').then(m => m.PendingClientsComponent),
        title: 'Pending Clients | Ember & Bean'
      },
      {
        path: 'clients',
        loadComponent: () => import('./features/admin/clients/clients.component').then(m => m.ClientsComponent),
        title: 'Approved Clients | Ember & Bean'
      },
      {
        path: 'create-admin',
        loadComponent: () => import('./features/admin/create-admin/create-admin.component').then(m => m.CreateAdminComponent),
        title: 'Manage Admins | Ember & Bean'
      },
      {
        path: 'products',
        loadComponent: () => import('./features/admin/products/admin-products.component').then(m => m.AdminProductsComponent),
        title: 'Products | Ember & Bean'
      },
      {
        path: 'inventory',
        loadComponent: () => import('./features/admin/inventory/admin-inventory.component').then(m => m.AdminInventoryComponent),
        title: 'Inventory | Ember & Bean'
      },
      {
        path: 'orders',
        loadComponent: () => import('./features/admin/orders/admin-orders.component').then(m => m.AdminOrdersComponent),
        title: 'All Orders | Ember & Bean'
      },
      {
        path: 'deliveries',
        loadComponent: () => import('./features/admin/deliveries/admin-deliveries.component').then(m => m.AdminDeliveriesComponent),
        title: 'Deliveries | Ember & Bean'
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },

  // ── Default ─────────────────────────────────────────────
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];
