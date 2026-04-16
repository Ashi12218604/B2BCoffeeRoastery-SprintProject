import { createReducer, on } from '@ngrx/store';
import { ProductState } from '../app.state';
import * as ProductActions from './products.actions';

export const initialProductState: ProductState = {
  products: [],
  selectedProduct: null,
  loading: false,
  error: null
};

export const productReducer = createReducer(
  initialProductState,

  on(ProductActions.loadProducts, (state) => ({ ...state, loading: true, error: null })),
  on(ProductActions.loadProductsSuccess, (state, { products }) => ({
    ...state, products, loading: false
  })),
  on(ProductActions.loadProductsFailure, (state, { error }) => ({
    ...state, loading: false, error
  })),

  on(ProductActions.loadProduct, (state) => ({ ...state, loading: true })),
  on(ProductActions.loadProductSuccess, (state, { product }) => ({
    ...state, selectedProduct: product, loading: false
  })),
  on(ProductActions.loadProductFailure, (state, { error }) => ({
    ...state, loading: false, error
  })),

  on(ProductActions.createProductSuccess, (state, { product }) => ({
    ...state, products: [product, ...state.products], loading: false
  })),

  on(ProductActions.updateProductSuccess, (state, { product }) => ({
    ...state,
    products: state.products.map(p => p.id === product.id ? product : p),
    selectedProduct: product,
    loading: false
  })),

  on(ProductActions.deleteProductSuccess, (state, { id }) => ({
    ...state, products: state.products.filter(p => p.id !== id), loading: false
  })),

  on(ProductActions.clearProductError, (state) => ({ ...state, error: null }))
);
