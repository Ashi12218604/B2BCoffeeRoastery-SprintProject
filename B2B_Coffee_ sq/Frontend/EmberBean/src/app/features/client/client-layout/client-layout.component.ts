import { Component, OnInit, OnDestroy, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, interval, Subscription } from 'rxjs';
import { selectUser } from '../../../store/auth/auth.selectors';
import { User } from '../../../shared/models/user.model';
import * as AuthActions from '../../../store/auth/auth.actions';

@Component({
  selector: 'app-client-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './client-layout.component.html',
  styleUrls: ['./client-layout.component.scss']
})
export class ClientLayoutComponent implements OnInit, OnDestroy {
  sidebarCollapsed = false;
  user$!: Observable<User | null>;
  
  private quoteSub!: Subscription;
  currentQuote = 0;
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

  constructor(
    private store: Store,
    private router: Router,
    private el: ElementRef
  ) {}

  get greeting(): string {
    const hour = new Date().getHours();
    if (hour < 12) return 'Good Morning';
    if (hour < 17) return 'Good Afternoon';
    return 'Good Evening';
  }

  onMouseMove(event: MouseEvent) {
    const container = this.el.nativeElement.querySelector('.dashboard-shell');
    if (container) {
      const x = (event.clientX / window.innerWidth) * 100;
      const y = (event.clientY / window.innerHeight) * 100;
      container.style.setProperty('--mouse-x', `${x}%`);
      container.style.setProperty('--mouse-y', `${y}%`);
    }
  }

  ngOnInit(): void {
    this.user$ = this.store.select(selectUser);
    
    this.quoteSub = interval(6000).subscribe(() => {
      this.currentQuote = (this.currentQuote + 1) % this.coffeeQuotes.length;
    });

    setTimeout(() => {
      const navItems = this.el.nativeElement.querySelectorAll('.nav-item');
      navItems.forEach((item: any) => {
        item.addEventListener('click', () => this.playClick());
      });
    }, 100);
  }

  ngOnDestroy(): void {
    if (this.quoteSub) {
      this.quoteSub.unsubscribe();
    }
  }

  toggleSidebar(): void {
    this.sidebarCollapsed = !this.sidebarCollapsed;
    this.playClick();
  }

  logout(): void {
    this.playClick();
    this.store.dispatch(AuthActions.logout());
  }

  playClick() {
    try {
      const AudioContextClass = window.AudioContext || (window as any).webkitAudioContext;
      if (!AudioContextClass) return;
      const ctx = new AudioContextClass();
      const osc = ctx.createOscillator();
      const gain = ctx.createGain();
      osc.connect(gain);
      gain.connect(ctx.destination);
      osc.type = 'sine';
      osc.frequency.setValueAtTime(800, ctx.currentTime);
      osc.frequency.exponentialRampToValueAtTime(300, ctx.currentTime + 0.05);
      gain.gain.setValueAtTime(0.1, ctx.currentTime);
      gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.05);
      osc.start();
      osc.stop(ctx.currentTime + 0.05);
    } catch (e) {}
  }
}
