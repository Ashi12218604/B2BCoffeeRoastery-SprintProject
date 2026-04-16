import { Component, OnInit, OnDestroy, ElementRef, ViewChild, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, interval, Subscription } from 'rxjs';
import { Chart, registerables } from 'chart.js';
import { selectUser } from '../../../store/auth/auth.selectors';
import { User } from '../../../shared/models/user.model';

Chart.register(...registerables);

interface CoffeeSlide {
  name: string;
  origin: string;
  description: string;
  imageUrl: string;
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule],
  encapsulation: ViewEncapsulation.None,
  template: `
    <div class="admin-dash" *ngIf="(user$ | async) as user">
      
      <!-- ═══ SUPERADMIN DASHBOARD (Original Layout) ═══ -->
      <ng-container *ngIf="user.role === 'SuperAdmin'">
        <h1 class="dash-title">SuperAdmin Dashboard</h1>
        <p class="dash-sub">Welcome to the Ember & Bean administration console.</p>
        
        <div class="stats-grid sa-stats">
          <div class="stat-card">
            <div class="stat-icon amber">👥</div>
            <div class="stat-info">
              <span class="stat-value">{{ pendingClientsCount }}</span>
              <span class="stat-label">Pending Clients</span>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon green">📦</div>
            <div class="stat-info">
              <span class="stat-value">{{ activeOrdersCount }}</span>
              <span class="stat-label">Active Orders</span>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon red">⚠️</div>
            <div class="stat-info">
              <span class="stat-value">{{ lowStockCount }}</span>
              <span class="stat-label">Low Stock Items</span>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon blue">🚚</div>
            <div class="stat-info">
              <span class="stat-value">{{ inTransitCount }}</span>
              <span class="stat-label">In Transit</span>
            </div>
          </div>
        </div>

        <div class="charts-wrap">
          <div class="chart-box">
            <h3 class="chart-title">Most Ordered Coffee Variants</h3>
            <canvas #pieChart></canvas>
          </div>
          <div class="chart-box">
            <h3 class="chart-title">Daily Revenue</h3>
            <canvas #barChart></canvas>
          </div>
        </div>
      </ng-container>

      <!-- ═══ ADMIN DASHBOARD (New Slideshow Layout) ═══ -->
      <ng-container *ngIf="user.role !== 'SuperAdmin'">
        <div class="admin-display-dash">
          
          <!-- Hero Slideshow -->
          <section class="hero-slideshow" (click)="goToCatalog()">
            <div class="slides-container">
              <div *ngFor="let slide of coffeeSlides; let i = index" class="slide" [class.active]="i === currentSlide">
                <img [src]="slide.imageUrl" [alt]="slide.name" class="slide-img" />
                <div class="slide-overlay"></div>
                <div class="slide-content">
                  <span class="slide-origin">{{ slide.origin }}</span>
                  <h2 class="slide-name">{{ slide.name }}</h2>
                  <p class="slide-desc">{{ slide.description }}</p>
                  <button class="slide-cta">View Products →</button>
                </div>
              </div>
            </div>
            <div class="slide-dots">
              <span *ngFor="let slide of coffeeSlides; let i = index" class="dot" [class.active]="i === currentSlide" (click)="goSlide(i); $event.stopPropagation()"></span>
            </div>
          </section>

          <!-- Stats Grid -->
          <div class="stats-grid">
            <div class="stat-card">
              <div class="stat-icon amber">👥</div>
              <div class="stat-info"><span class="stat-value">{{ pendingClientsCount }}</span><span class="stat-label">Pending Approvals</span></div>
            </div>
            <div class="stat-card">
              <div class="stat-icon green">📦</div>
              <div class="stat-info"><span class="stat-value">{{ activeOrdersCount }}</span><span class="stat-label">Active Orders</span></div>
            </div>
            <div class="stat-card">
              <div class="stat-icon red">⚠️</div>
              <div class="stat-info"><span class="stat-value">{{ lowStockCount }}</span><span class="stat-label">Low Resupply</span></div>
            </div>
            <div class="stat-card">
              <div class="stat-icon blue">🚚</div>
              <div class="stat-info"><span class="stat-value">{{ inTransitCount }}</span><span class="stat-label">In Transit</span></div>
            </div>
          </div>

          <!-- Best Sellers -->
          <div class="section bestsellers-section">
            <h2 class="section-title">🔥 Most Popular Across Ember & Bean</h2>
            <div class="bestsellers-grid">
              <div *ngFor="let coffee of bestSellers" class="bestseller-card" (click)="goToCatalog()">
                <div class="bestseller-img-wrap">
                  <img [src]="coffee.imageUrl" [alt]="coffee.name" class="bestseller-img" />
                  <div class="bestseller-overlay"><span class="bestseller-tag">Top Choice</span></div>
                </div>
                <div class="bestseller-info">
                  <h3 class="bestseller-name">{{ coffee.name }}</h3>
                  <p class="bestseller-origin">{{ coffee.origin }}</p>
                </div>
              </div>
            </div>
          </div>

        </div>
      </ng-container>

    </div>
  `,
  styles: [`
    .admin-dash { width: 100%; display: flex; flex-direction: column; gap: 40px; }
    
    /* COMMON TEXT - HIGH CONTRAST */
    .dash-title { font-family: 'Playfair Display', serif; font-size: 2.8rem; font-weight: 800; color: #FDFCF5; margin-bottom: 12px; text-shadow: 0 4px 15px rgba(0,0,0,0.8); letter-spacing: -0.01em; }
    .dash-sub { font-family: 'Inter', sans-serif; font-size: 1.1rem; font-weight: 400; color: rgba(253, 245, 230, 0.9); margin-bottom: 48px; text-shadow: 0 2px 8px rgba(0,0,0,0.6); }
    
    /* STATS (Shared) - ENHANCED VISIBILITY */
    .stats-grid { perspective: 1000px; display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 24px; }
    .sa-stats { margin-bottom: 48px; }
    .stat-card {
      background: rgba(35, 25, 20, 0.85); backdrop-filter: blur(24px); border: 1px solid rgba(184, 115, 51, 0.4);
      border-radius: 20px; padding: 32px; display: flex; align-items: center; gap: 20px;
      transition: all 0.5s cubic-bezier(0.175, 0.885, 0.32, 1.275);
      box-shadow: 0 10px 30px rgba(0,0,0,0.5);
    }
    .stat-card:hover { border-color: rgba(255,191,0,0.6); transform: translateY(-5px); box-shadow: 0 20px 40px rgba(0,0,0,0.7); }
    .stat-icon { width: 56px; height: 56px; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 28px; transition: transform 0.5s cubic-bezier(0.175, 0.885, 0.32, 1.275); flex-shrink: 0; }
    .stat-icon.amber { background: rgba(255,191,0,0.25); color: #FFD54F; } 
    .stat-icon.green { background: rgba(76,175,80,0.25); color: #81C784; }
    .stat-icon.red { background: rgba(244,67,54,0.25); color: #EF5350; } 
    .stat-icon.blue { background: rgba(41,182,246,0.25); color: #4FC3F7; }
    .stat-card:hover .stat-icon { transform: scale(1.1) rotate(5deg); }
    .stat-info { display: flex; flex-direction: column; }
    .stat-value { font-family: 'Playfair Display', serif; font-size: 2rem; font-weight: 800; color: #FDFCF5; line-height: 1; }
    .stat-label { font-family: 'Inter', sans-serif; font-size: 0.85rem; color: rgba(253,245,230,0.8); margin-top: 6px; text-transform: uppercase; letter-spacing: 0.1em; font-weight: 600; }
    
    /* CHARTS (SuperAdmin) */
    .charts-wrap { 
      display: grid; 
      grid-template-columns: repeat(auto-fit, minmax(450px, 1fr)); 
      gap: 32px; 
      margin-bottom: 40px; 
    }
    .chart-box { 
      background: rgba(30, 20, 15, 0.4); 
      backdrop-filter: blur(24px); 
      border: 1px solid rgba(184, 115, 51, 0.2); 
      border-radius: 16px; 
      padding: 28px;
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
    }
    .chart-title { 
      font-family: 'Playfair Display', serif; 
      font-size: 1.3rem; 
      color: #FDFCF5; 
      margin-bottom: 24px; 
      font-weight: 600; 
      text-shadow: 0 2px 4px rgba(0,0,0,0.5);
      border-bottom: 1px solid rgba(184, 115, 51, 0.1);
      padding-bottom: 12px;
    }
    
    /* ADMIN DASHBOARD LAYOUT */
    .admin-display-dash { display: flex; flex-direction: column; gap: 40px; }
    
    /* Hero Slideshow */
    .hero-slideshow {
      position: relative; width: 100%; height: 400px; border-radius: 16px; overflow: hidden; cursor: pointer;
      background: #1a110a; box-shadow: 0 10px 30px rgba(0, 0, 0, 0.4);
    }
    .slides-container { width: 100%; height: 100%; position: relative; }
    .slide { position: absolute; inset: 0; opacity: 0; transition: opacity 1.5s ease-in-out; z-index: 1; }
    .slide.active { opacity: 1; z-index: 2; }
    .slide-img { width: 100%; height: 100%; object-fit: cover; object-position: center; }
    .slide.active .slide-img { animation: zoomIn 15s linear forwards; }
    .slide-overlay { position: absolute; inset: 0; background: linear-gradient(to top, rgba(15, 10, 5, 0.95) 0%, rgba(15, 10, 5, 0.4) 50%, transparent 100%); }
    .slide-content { position: absolute; bottom: 0; left: 0; width: 100%; padding: 40px; display: flex; flex-direction: column; align-items: flex-start; transform: translateY(20px); opacity: 0; transition: all 1s cubic-bezier(0.175, 0.885, 0.32, 1); transition-delay: 0.3s; }
    .slide.active .slide-content { transform: translateY(0); opacity: 1; }
    .slide-origin { font-family: 'Inter', sans-serif; font-size: 0.8rem; font-weight: 600; color: #B87333; letter-spacing: 0.15em; text-transform: uppercase; margin-bottom: 8px; }
    .slide-name { font-family: 'Playfair Display', serif; font-size: 2.5rem; font-weight: 700; color: #FDFCF5; margin-bottom: 12px; text-shadow: 0 4px 12px rgba(0,0,0,0.6); }
    .slide-desc { font-family: 'Inter', sans-serif; font-size: 1rem; color: rgba(253, 252, 245, 0.8); max-width: 600px; margin-bottom: 24px; line-height: 1.5; }
    .slide-cta { background: rgba(184, 115, 51, 0.9); backdrop-filter: blur(5px); color: #fff; border: none; padding: 12px 24px; border-radius: 6px; font-family: 'Inter', sans-serif; font-weight: 600; cursor: pointer; transition: all 0.3s; }
    .slide-cta:hover { background: #D18F52; transform: translateY(-2px); }
    
    .slide-dots { position: absolute; bottom: 20px; right: 30px; z-index: 10; display: flex; gap: 8px; }
    .dot { width: 10px; height: 10px; border-radius: 50%; background: rgba(255, 255, 255, 0.3); cursor: pointer; transition: all 0.3s; }
    .dot.active { background: #B87333; transform: scale(1.3); }

    @keyframes zoomIn { from { transform: scale(1); } to { transform: scale(1.1); } }

    /* Best Sellers */
    .section-title { font-family: 'Playfair Display', serif; font-size: 1.5rem; font-weight: 600; color: #3D2B1F; margin-bottom: 24px; padding-bottom: 12px; border-bottom: 1px solid rgba(184, 115, 51, 0.2); }
    .bestsellers-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(260px, 1fr)); gap: 24px; }
    .bestseller-card { background: rgba(30,30,30,0.3); border: 1px solid rgba(184,115,51,0.15); border-radius: 12px; overflow: hidden; cursor: pointer; transition: all 0.3s ease; }
    .bestseller-card:hover { transform: translateY(-5px); border-color: rgba(184,115,51,0.5); box-shadow: 0 10px 25px rgba(0,0,0,0.5); }
    .bestseller-card:hover .bestseller-img { transform: scale(1.05); }
    .bestseller-img-wrap { width: 100%; height: 180px; position: relative; overflow: hidden; }
    .bestseller-img { width: 100%; height: 100%; object-fit: cover; transition: transform 0.5s ease; }
    .bestseller-overlay { position: absolute; top: 12px; right: 12px; }
    .bestseller-tag { background: rgba(184, 115, 51, 0.9); color: #fff; padding: 4px 10px; border-radius: 20px; font-size: 0.7rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em; backdrop-filter: blur(4px); }
    .bestseller-info { padding: 20px; }
    .bestseller-name { font-family: 'Playfair Display', serif; font-size: 1.2rem; color: #FDFCF5; margin: 0 0 4px; }
    .bestseller-origin { font-family: 'Inter', sans-serif; font-size: 0.75rem; color: #B87333; text-transform: uppercase; letter-spacing: 0.1em; margin: 0; }

    @media(max-width: 768px) { .charts-wrap { grid-template-columns: 1fr; } }
  `]
})
export class AdminDashboardComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('pieChart') pieChartRef!: ElementRef;
  @ViewChild('barChart') barChartRef!: ElementRef;

  user$!: Observable<User | null>;
  
  pendingClientsCount: number | string = '—';
  activeOrdersCount: number | string = '—';
  lowStockCount: number | string = '—';
  inTransitCount: number | string = '—';

  pieChartInstance: any;
  barChartInstance: any;

  // New Admin Dashboard variables
  private slideSub!: Subscription;
  private quoteSub!: Subscription;
  currentSlide = 0;
  currentQuote = 0;

  coffeeSlides: CoffeeSlide[] = [
    { name: 'Ethiopian Yirgacheffe', origin: 'Ethiopia', description: 'Bright, floral with jasmine, lemon & blueberry notes', imageUrl: '/assets/images/products/ethiopian-yirgacheffe.png' },
    { name: 'Colombian Supremo', origin: 'Colombia', description: 'Well-balanced caramel, chocolate & walnut flavours', imageUrl: '/assets/images/products/colombian-supremo.png' },
    { name: 'Italian Espresso Classico', origin: 'Italy/Brazil', description: 'Bold dark roast with thick crema, spice & cocoa', imageUrl: '/assets/images/products/italian-espresso.png' },
    { name: 'Kenya AA Peaberry', origin: 'Kenya', description: 'Intense blackcurrant, wine & citrus brightness', imageUrl: '/assets/images/products/kenya-aa.png' },
    { name: 'Guatemala Antigua', origin: 'Guatemala', description: 'Complex spicy, smokey & chocolaty highland flavours', imageUrl: '/assets/images/products/guatemala-antigua.png' },
    { name: 'Sumatra Mandheling', origin: 'Indonesia', description: 'Earthy & herbal with dark chocolate & cedar finish', imageUrl: '/assets/images/products/sumatra-mandheling.png' },
    { name: 'Swiss Water Decaf', origin: 'Brazil/Peru', description: 'Full flavour, zero caffeine — smooth & mellow', imageUrl: '/assets/images/products/swiss-decaf.png' },
    { name: 'Cold Brew Concentrate', origin: 'Brazil', description: 'Smooth, low-acid with natural chocolate sweetness', imageUrl: '/assets/images/products/cold-brew.png' },
    { name: 'Costa Rica Tarrazu', origin: 'Costa Rica', description: 'Honey notes of apricot, brown sugar & silky body', imageUrl: '/assets/images/products/costa-rica-tarrazu.png' },
    { name: 'Midnight Mocha Blend', origin: 'Ethiopia/Brazil', description: 'Rich cocoa, toasted almond & vanilla perfection', imageUrl: '/assets/images/products/midnight-mocha.png' },
  ];

  coffeeQuotes: string[] = [
    '"A morning without coffee is like sleep."',
    '"Great management starts with great coffee."',
    '"Roasting is science; brewing is art."',
    '"Fresh beans, fresh mind, fresh business."',
    '"The best ideas come from the bottom of an empty coffee cup."',
    '"Good coffee is a human right."',
  ];

  bestSellers: CoffeeSlide[] = [];

  constructor(private http: HttpClient, private store: Store, private router: Router) {}

  ngOnInit() {
    this.user$ = this.store.select(selectUser);
    
    this.fetchStats();
    this.fetchOrdersAndGenerateCharts();

    // Auto-rotate slides & quotes for Admin layout
    this.slideSub = interval(5000).subscribe(() => this.currentSlide = (this.currentSlide + 1) % this.coffeeSlides.length);
    this.quoteSub = interval(8000).subscribe(() => this.currentQuote = (this.currentQuote + 1) % this.coffeeQuotes.length);

    // Pick 4 best sellers randomly for demo (would be real data)
    this.bestSellers = [...this.coffeeSlides].sort(() => Math.random() - 0.5).slice(0, 4);
  }

  ngOnDestroy(): void {
    if (this.slideSub) this.slideSub.unsubscribe();
    if (this.quoteSub) this.quoteSub.unsubscribe();
  }

  ngAfterViewInit() {
    // Attempt init in case it's a SuperAdmin immediately.
    // If Admin, the elements won't exist but it's safe.
    setTimeout(() => {
      if (this.pieChartRef && this.barChartRef) {
        this.initPieChart([], []);
        this.initBarChart([], []);
      }
    }, 500);
  }

  getGreeting(): string {
    const hour = new Date().getHours();
    if (hour < 12) return 'Good Morning';
    if (hour < 17) return 'Good Afternoon';
    return 'Good Evening';
  }

  goSlide(i: number) { this.currentSlide = i; }
  goToCatalog() { this.router.navigate(['/admin/products']); }

  fetchStats() {
    this.http.get<any[]>('http://localhost:7101/api/admin/pending-clients').subscribe({
      next: (clients) => this.pendingClientsCount = clients.length,
      error: () => this.pendingClientsCount = 0
    });

    this.http.get<any[]>('http://localhost:7101/api/inventory').subscribe({
      next: (inventory) => {
        this.lowStockCount = inventory.filter(i => i.quantityAvailable <= (i.reorderLevel || 10)).length;
      },
      error: () => this.lowStockCount = 0
    });

    this.http.get<any[]>('http://localhost:7101/api/deliveries').subscribe({
      next: (deliveries) => {
        this.inTransitCount = deliveries.filter(d => d.status === 1 || d.status === 'InTransit' || d.status === 'Dispatched').length;
      },
      error: () => this.inTransitCount = 0
    });
  }

  fetchOrdersAndGenerateCharts() {
    this.http.get<any>('http://localhost:7101/api/orders?pageSize=1000').subscribe({
      next: (response) => {
        const orders = response.items || response;
        this.activeOrdersCount = Array.isArray(orders) ? orders.filter((o: any) => o.status !== 5 && o.status !== 'Cancelled' && o.status !== 'Delivered').length : 0;
        
        if (Array.isArray(orders)) {
          setTimeout(() => this.processChartData(orders), 500);
        }
      },
      error: () => {}
    });
  }

  processChartData(orders: any[]) {
    if (!this.pieChartRef || !this.barChartRef) return; // Not SuperAdmin

    // 1. Most Ordered Coffee variants
    const productCounts: { [key: string]: number } = {};
    orders.forEach(o => {
      o.items?.forEach((i: any) => {
        productCounts[i.productName] = (productCounts[i.productName] || 0) + i.quantity;
      });
    });

    const pieLabels = Object.keys(productCounts).slice(0, 5);
    const pieData = Object.values(productCounts).slice(0, 5);
    this.initPieChart(pieLabels, pieData);

    // 2. Revenue by date (Last 10 days)
    const revenueByDate: { [key: string]: number } = {};
    const today = new Date();
    
    // Initialize last 10 days with 0 to ensure continuity
    for(let i=9; i>=0; i--) {
      const d = new Date(today);
      d.setDate(today.getDate() - i);
      const dateStr = d.toLocaleDateString('default', { month: 'short', day: 'numeric' });
      revenueByDate[dateStr] = 0;
    }

    orders.forEach(o => {
      const d = new Date(o.placedAt);
      const dateStr = d.toLocaleDateString('default', { month: 'short', day: 'numeric' });
      // Only include if within our 10-day window
      if (revenueByDate.hasOwnProperty(dateStr)) {
        revenueByDate[dateStr] += o.totalAmount;
      }
    });

    const barLabels = Object.keys(revenueByDate);
    const barData = Object.values(revenueByDate);
    this.initBarChart(barLabels, barData);
  }

  initPieChart(labels: string[], data: number[]) {
    if (this.pieChartInstance) this.pieChartInstance.destroy();
    if (!this.pieChartRef) return;
    
    this.pieChartInstance = new Chart(this.pieChartRef.nativeElement, {
      type: 'doughnut',
      data: {
        labels,
        datasets: [{
          data,
          backgroundColor: ['#d18f52', '#8c5936', '#4a301e', '#e3a971', '#5e4331'],
          borderWidth: 0
        }]
      },
      options: {
        responsive: true,
        animation: {
          duration: 2000,
          easing: 'easeOutBounce',
          animateRotate: true,
          animateScale: true
        },
        plugins: {
          legend: { position: 'right', labels: { color: '#FDFCF5' } }
        }
      }
    });
  }

  initBarChart(labels: string[], data: number[]) {
    if (this.barChartInstance) this.barChartInstance.destroy();
    if (!this.barChartRef) return;

    this.barChartInstance = new Chart(this.barChartRef.nativeElement, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'Revenue ($)',
          data,
          backgroundColor: '#d18f52',
          borderRadius: 4
        }]
      },
      options: {
        responsive: true,
        animation: {
          duration: 2500,
          easing: 'easeOutElastic'
        },
        scales: {
          y: { ticks: { color: '#bbb' }, grid: { color: 'rgba(255,255,255,0.05)' } },
          x: { ticks: { color: '#bbb' }, grid: { display: false } }
        },
        plugins: {
          legend: { display: false }
        }
      }
    });
  }
}
