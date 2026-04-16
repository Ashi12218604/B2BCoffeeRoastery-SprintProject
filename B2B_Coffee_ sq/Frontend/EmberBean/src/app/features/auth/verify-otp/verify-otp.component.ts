import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import * as AuthActions from '../../../store/auth/auth.actions';
import { selectAuthLoading, selectAuthError, selectOtpEmail, selectOtpVerified } from '../../../store/auth/auth.selectors';

@Component({
  selector: 'app-verify-otp',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './verify-otp.component.html',
  styleUrls: ['./verify-otp.component.scss']
})
export class VerifyOtpComponent implements OnInit, OnDestroy {
  otpForm!: FormGroup;
  loading$!: Observable<boolean>;
  error$!: Observable<string | null>;
  otpEmail$!: Observable<string | null>;
  otpVerified$!: Observable<boolean>;
  private destroy$ = new Subject<void>();

  constructor(private fb: FormBuilder, private store: Store) {}

  ngOnInit(): void {
    this.otpForm = this.fb.group({
      otpCode: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
    });

    this.loading$ = this.store.select(selectAuthLoading);
    this.error$ = this.store.select(selectAuthError);
    this.otpEmail$ = this.store.select(selectOtpEmail);
    this.otpVerified$ = this.store.select(selectOtpVerified);
  }

  onSubmit(): void {
    if (this.otpForm.valid) {
      let email = '';
      this.otpEmail$.pipe(takeUntil(this.destroy$)).subscribe(e => email = e || '');
      this.store.dispatch(AuthActions.verifyOtp({
        request: { email, otpCode: this.otpForm.value.otpCode }
      }));
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
