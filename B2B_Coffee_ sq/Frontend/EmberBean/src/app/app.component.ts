import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Store } from '@ngrx/store';
import * as AuthActions from './store/auth/auth.actions';
import { ChatbotComponent } from './shared/components/chatbot/chatbot.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ChatbotComponent],
  template: `
    <router-outlet></router-outlet>
    <app-chatbot></app-chatbot>
  `,
  styles: [':host { display: block; height: 100%; }']
})

export class AppComponent implements OnInit {
  constructor(private store: Store) {}

  ngOnInit(): void {
    // Attempt auto-login from stored JWT token
    this.store.dispatch(AuthActions.autoLogin());
  }
}
