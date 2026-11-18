import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, getAuthErrorMessage } from '../services/auth-service';

@Component({
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  standalone:true,
  selector: 'app-login-form',
  styleUrl: './login-form.css',
  templateUrl: './login-form.html',
})
export class LoginForm {
  private fb = inject(FormBuilder).nonNullable;
  private authService = inject(AuthService);
  private router = inject(Router);

  errorMessage: string | null = null;
  loginForm = this.fb.group({
    Email: ['', [Validators.required, Validators.email, Validators.minLength(2), Validators.maxLength(40)]],
    Password: ['', [
      Validators.required,
      Validators.minLength(6),
      Validators.maxLength(30),
      Validators.pattern(/^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[#?!@$%^&*-]).+$/)
    ]]
  });

  onSubmit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.errorMessage = null;
    this.authService.login(this.loginForm.getRawValue()).subscribe({
      next: () => {
        this.router.navigate(['/all']);
      },
      error: (error) => {
        this.errorMessage = getAuthErrorMessage(error, 'Could not sign in. Check your email and password, then try again.');
      }
    });
  }

  clearMessages(): void {
    this.errorMessage = null;
  }
}
