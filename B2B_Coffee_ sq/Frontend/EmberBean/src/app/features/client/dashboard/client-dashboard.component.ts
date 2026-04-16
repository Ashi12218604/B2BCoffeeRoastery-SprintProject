import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, interval, Subscription } from 'rxjs';
import { selectUser } from '../../../store/auth/auth.selectors';
import { selectAllOrders } from '../../../store/orders/orders.selectors';
import { User } from '../../../shared/models/user.model';
import { Order } from '../../../shared/models/order.model';
import { FilterStatusPipe } from '../../../shared/pipes/filter-status.pipe';
import * as OrderActions from '../../../store/orders/orders.actions';

interface CoffeeSlide {
  name: string;
  origin: string;
  description: string;
  imageUrl: string;
}

@Component({
  selector: 'app-client-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, FilterStatusPipe],
  templateUrl: './client-dashboard.component.html',
  styleUrls: ['./client-dashboard.component.scss']
})
export class ClientDashboardComponent implements OnInit, OnDestroy {
  private ordersCache: Order[] = [];
  private slideSub!: Subscription;
  private quoteSub!: Subscription;

  user$!: Observable<User | null>;
  orders$!: Observable<Order[]>;

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
    '"Coffee is a language in itself." — Jackie Chan',
    '"Life is too short for bad coffee."',
    '"Behind every successful person is a substantial amount of coffee."',
    '"But first, coffee." — Every morning, everywhere.',
    '"Adventure in life is good; consistency in coffee even better."',
    '"Coffee — because adulting is hard."',
    '"A morning without coffee is like sleep."',
    '"Good ideas start with brainstorming. Great ideas start with coffee."',
    '"Espresso yourself."',
    '"Today\'s good mood is sponsored by coffee."',
  ];

  bestSellers: CoffeeSlide[] = [];

  constructor(private store: Store, private router: Router) {}

  ngOnInit(): void {
    this.user$ = this.store.select(selectUser);
    this.orders$ = this.store.select(selectAllOrders);
    this.orders$.subscribe(orders => this.ordersCache = orders);
    this.store.dispatch(OrderActions.loadMyOrders());

    // Auto-rotate slides every 5 seconds
    this.slideSub = interval(5000).subscribe(() => {
      this.currentSlide = (this.currentSlide + 1) % this.coffeeSlides.length;
    });

    // Auto-rotate quotes every 8 seconds
    this.quoteSub = interval(8000).subscribe(() => {
      this.currentQuote = (this.currentQuote + 1) % this.coffeeQuotes.length;
    });

    // Pick 4 best sellers (random for demo, in production would use order data)
    this.bestSellers = [...this.coffeeSlides]
      .sort(() => Math.random() - 0.5)
      .slice(0, 4);
  }

  ngOnDestroy(): void {
    this.slideSub?.unsubscribe();
    this.quoteSub?.unsubscribe();
  }

  getGreeting(): string {
    const hour = new Date().getHours();
    if (hour < 12) return 'Good Morning';
    if (hour < 17) return 'Good Afternoon';
    return 'Good Evening';
  }

  getTotalSpending(): number {
    return this.ordersCache.reduce((sum, o) => sum + (o.totalAmount || 0), 0);
  }

  goSlide(i: number): void {
    this.currentSlide = i;
  }

  goToCatalog(): void {
    this.router.navigate(['/client/catalog']);
  }
}
