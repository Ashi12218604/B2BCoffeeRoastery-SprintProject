import { ApplicationConfig, provideZoneChangeDetection, isDevMode } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { authReducer } from './store/auth/auth.reducer';
import { productReducer } from './store/products/products.reducer';
import { orderReducer } from './store/orders/orders.reducer';
import { inventoryReducer } from './store/inventory/inventory.reducer';
import { deliveryReducer } from './store/deliveries/deliveries.reducer';
import { AuthEffects } from './store/auth/auth.effects';
import { ProductEffects } from './store/products/products.effects';
import { OrderEffects } from './store/orders/orders.effects';
import { InventoryEffects } from './store/inventory/inventory.effects';
import { DeliveryEffects } from './store/deliveries/deliveries.effects';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([authInterceptor])),

    // NgRx Store — all feature reducers
    provideStore({
      auth: authReducer,
      products: productReducer,
      orders: orderReducer,
      inventory: inventoryReducer,
      deliveries: deliveryReducer
    }),

    // NgRx Effects — all side-effect handlers
    provideEffects([
      AuthEffects,
      ProductEffects,
      OrderEffects,
      InventoryEffects,
      DeliveryEffects
    ]),

    // NgRx DevTools — only in development
    provideStoreDevtools({
      maxAge: 25,
      logOnly: !isDevMode(),
      autoPause: true,
      trace: false,
      traceLimit: 75
    })
  ]
};
