import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { Product } from '../../../shared/models/product.model';
import { selectAllProducts, selectProductsLoading } from '../../../store/products/products.selectors';
import * as ProductActions from '../../../store/products/products.actions';
import { OrderService } from '../../../core/services/order.service';
import { PlaceOrderRequest } from '../../../shared/models/order.model';
import { ProductService } from '../../../core/services/product.service';
import { ProductReview } from '../../../shared/models/product.model';

@Component({
  selector: 'app-catalog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.scss']
})
export class CatalogComponent implements OnInit {
  products$!: Observable<Product[]>;
  loading$!: Observable<boolean>;
  searchTerm = '';
  selectedCategory = '';
  selectedRoast = '';

  categories = ['All', 'SingleOrigin', 'EspressoBlend', 'Decaf', 'ColdBrew', 'Single Origin'];
  roastLevels = ['All', 'Light', 'Medium', 'MediumDark', 'Dark'];

  // ── Cart state ──
  cart: { product: Product; quantity: number }[] = [];
  showCart = false;
  cartMessage = '';

  // ── Razorpay Checkout state ──
  showRazorpay = false;
  rpStep: 'address' | 'methods' | 'card' | 'upi' | 'netbanking' | 'processing' | 'success' | 'failed' = 'address';
  selectedPaymentMethod = 'card';

  // Shipping Address
  shippingAddress = '';
  shippingCity = '';
  shippingState = '';
  shippingPin = '';

  // Card form fields (demo pre-filled)
  cardNumber = '4111 1111 1111 1111';
  cardExpiry = '12/28';
  cardCvv = '123';
  cardName = 'Demo User';

  // UPI (demo pre-filled)
  upiId = 'success@razorpay';

  // Netbanking
  selectedBank = 'HDFC Bank';
  banks = ['State Bank of India', 'HDFC Bank', 'ICICI Bank', 'Axis Bank', 'Kotak Mahindra', 'Punjab National Bank'];

  // ── Reviews state ──
  showReviewsModal = false;
  selectedProductForReviews: Product | null = null;
  productReviews: ProductReview[] = [];
  loadingReviews = false;
  newReviewRating = 5;
  newReviewComment = '';

  constructor(private store: Store, private orderService: OrderService, private productService: ProductService) {}

  ngOnInit(): void {
    this.products$ = this.store.select(selectAllProducts);
    this.loading$ = this.store.select(selectProductsLoading);
    this.store.dispatch(ProductActions.loadProducts({}));
  }

  filterProducts(products: Product[]): Product[] {
    let filtered = products;
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(p =>
        p.name.toLowerCase().includes(term) ||
        p.origin.toLowerCase().includes(term) ||
        p.description.toLowerCase().includes(term)
      );
    }
    if (this.selectedCategory && this.selectedCategory !== 'All') {
      filtered = filtered.filter(p => p.category === this.selectedCategory);
    }
    if (this.selectedRoast && this.selectedRoast !== 'All') {
      filtered = filtered.filter(p => p.roastLevel === this.selectedRoast);
    }
    return filtered;
  }

  getRoastColor(roastLevel: string): string {
    const colors: Record<string, string> = {
      'Light': '#FFD54F',
      'Medium': '#FFB74D',
      'MediumDark': '#FF8A65',
      'Medium-Dark': '#FF8A65',
      'Dark': '#8D6E63'
    };
    return colors[roastLevel] || '#B87333';
  }

  // ── Cart methods ──
  addToCart(product: Product): void {
    const existing = this.cart.find(c => c.product.id === product.id);
    if (existing) {
      existing.quantity += (product.minimumOrderQuantity || 1);
    } else {
      this.cart.push({ product, quantity: product.minimumOrderQuantity || 1 });
    }
    this.cartMessage = `${product.name} added!`;
    setTimeout(() => this.cartMessage = '', 2000);
  }

  removeFromCart(index: number): void {
    this.cart.splice(index, 1);
  }

  updateQty(index: number, qty: number): void {
    const min = this.cart[index].product.minimumOrderQuantity || 1;
    this.cart[index].quantity = Math.max(min, qty);
  }

  get cartTotal(): number {
    return this.cart.reduce((sum, item) =>
      sum + ((item.product.discountedPrice || item.product.price) * item.quantity), 0);
  }

  // ── Reviews methods ──
  openReviews(product: Product): void {
    this.selectedProductForReviews = product;
    this.showReviewsModal = true;
    this.loadReviews(product.id);
  }

  closeReviews(): void {
    this.showReviewsModal = false;
    this.selectedProductForReviews = null;
    this.productReviews = [];
    this.newReviewComment = '';
    this.newReviewRating = 5;
  }

  loadReviews(productId: string): void {
    this.loadingReviews = true;
    this.productService.getReviews(productId).subscribe({
      next: (reviews) => {
        this.productReviews = reviews;
        this.loadingReviews = false;
      },
      error: (err) => {
        console.error('Failed to load reviews', err);
        this.loadingReviews = false;
      }
    });
  }

  submitReview(): void {
    if (!this.selectedProductForReviews || !this.newReviewComment.trim()) return;

    this.productService.addReview(this.selectedProductForReviews.id, {
      rating: this.newReviewRating,
      comment: this.newReviewComment
    }).subscribe({
      next: (review) => {
        this.productReviews.unshift(review);
        this.newReviewComment = '';
        this.newReviewRating = 5;
        // The store average rating won't update automatically here without a dispatch,
        // but the visual list updates immediately.
      },
      error: (err) => {
        console.error('Failed to add review', err);
        alert('Could not add review. Make sure you have permission to do so.');
      }
    });
  }

  get cartItemCount(): number {
    return this.cart.reduce((sum, item) => sum + item.quantity, 0);
  }

  // ── Razorpay Checkout ──
  openCheckout(): void {
    if (this.cart.length === 0) return;
    this.showCart = false;
    this.showRazorpay = true;
    this.rpStep = 'address';
    this.selectedPaymentMethod = 'card';
    // Reset form fields
    this.cardNumber = '';
    this.cardExpiry = '';
    this.cardCvv = '';
    this.cardName = '';
    this.upiId = '';
    this.selectedBank = '';
  }

  closeCheckout(): void {
    if (this.rpStep === 'processing') return;
    this.showRazorpay = false;
  }

  goToMethods(): void {
    if (!this.shippingAddress || !this.shippingCity || !this.shippingState || !this.shippingPin) {
      alert('Please fill out the complete shipping address.');
      return;
    }
    this.rpStep = 'methods';
  }

  selectMethod(method: string): void {
    this.selectedPaymentMethod = method;
    this.rpStep = method as any;
  }

  goBackToMethods(): void {
    this.rpStep = 'methods';
  }

  goBackToAddress(): void {
    this.rpStep = 'address';
  }

  processPayment(): void {
    this.rpStep = 'processing';

    let methodDetails = this.selectedPaymentMethod;
    if (this.selectedPaymentMethod === 'upi') {
      methodDetails = 'UPI (' + this.upiId + ')';
    } else if (this.selectedPaymentMethod === 'netbanking') {
      methodDetails = 'Netbanking (' + this.selectedBank + ')';
    } else if (this.selectedPaymentMethod === 'card') {
      methodDetails = 'Card (ending in ' + this.cardNumber.slice(-4) + ')';
    }

    const req: PlaceOrderRequest = {
      items: this.cart.map(c => ({
        productId: c.product.id,
        productName: c.product.name,
        sku: c.product.sku,
        quantity: c.quantity,
        unitPrice: c.product.discountedPrice || c.product.price
      })),
      notes: `Paid via Razorpay: ${methodDetails}`,
      deliveryAddress: this.shippingAddress,
      city: this.shippingCity,
      state: this.shippingState,
      pinCode: this.shippingPin
    };

    this.orderService.placeOrder(req).subscribe({
      next: (order) => {
        // After order placed, create payment + verify in sequence
        this.orderService.createPaymentOrder(order.id).subscribe({
          next: (payRes) => {
            const paymentId = (payRes as any).demoPaymentIdToUseForTest || (payRes as any).DemoPaymentIdToUseForTest || ('pay_DEMO_' + Math.random().toString(36).substr(2, 9));
            const signature = (payRes as any).demoSignatureToUseForTest || (payRes as any).DemoSignatureToUseForTest || 'sig_DEMO';

            this.orderService.verifyPayment(order.id, {
              razorpayPaymentId: paymentId,
              razorpaySignature: signature
            }).subscribe({
              next: () => {
                this.rpStep = 'success';
                this.cart = [];
              },
              error: (err) => {
                console.error('Payment verification failed:', err);
                this.rpStep = 'failed';
              }
            });
          },
          error: (err) => {
            console.error('Create payment order failed:', err);
            this.rpStep = 'failed';
          }
        });
      },
      error: (err) => {
        console.error('Place order failed:', err);
        this.rpStep = 'failed';
      }
    });
  }

  finishCheckout(): void {
    this.showRazorpay = false;
    this.rpStep = 'methods';
  }
}
