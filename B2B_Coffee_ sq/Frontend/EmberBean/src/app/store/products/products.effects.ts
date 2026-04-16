import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError } from 'rxjs/operators';
import { ProductService } from '../../core/services/product.service';
import * as ProductActions from './products.actions';

@Injectable()
export class ProductEffects {
  private actions$ = inject(Actions);
  private productService = inject(ProductService);

  loadProducts$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProductActions.loadProducts),
      mergeMap(({ params }) =>
        this.productService.getAll(params).pipe(
          map(products => ProductActions.loadProductsSuccess({ products })),
          catchError(err => of(ProductActions.loadProductsFailure({
            error: err.error?.message || 'Failed to load products'
          })))
        )
      )
    )
  );

  loadProduct$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProductActions.loadProduct),
      mergeMap(({ id }) =>
        this.productService.getById(id).pipe(
          map(product => ProductActions.loadProductSuccess({ product })),
          catchError(err => of(ProductActions.loadProductFailure({
            error: err.error?.message || 'Failed to load product'
          })))
        )
      )
    )
  );

  createProduct$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProductActions.createProduct),
      mergeMap(({ product }) =>
        this.productService.create(product).pipe(
          map(created => ProductActions.createProductSuccess({ product: created })),
          catchError(err => of(ProductActions.createProductFailure({
            error: err.error?.message || 'Failed to create product'
          })))
        )
      )
    )
  );

  updateProduct$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProductActions.updateProduct),
      mergeMap(({ id, product }) =>
        this.productService.update(id, product).pipe(
          map(updated => ProductActions.updateProductSuccess({ product: updated })),
          catchError(err => of(ProductActions.updateProductFailure({
            error: err.error?.message || 'Failed to update product'
          })))
        )
      )
    )
  );

  deleteProduct$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProductActions.deleteProduct),
      mergeMap(({ id }) =>
        this.productService.delete(id).pipe(
          map(() => ProductActions.deleteProductSuccess({ id })),
          catchError(err => of(ProductActions.deleteProductFailure({
            error: err.error?.message || 'Failed to delete product'
          })))
        )
      )
    )
  );
}
