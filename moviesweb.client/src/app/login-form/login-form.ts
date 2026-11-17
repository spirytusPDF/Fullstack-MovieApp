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
    Email: ['', [Validators.required, Validators.email]],
    Password: ['', Validators.required]
  });

  onSubmit() {
    if (this.loginForm.invalid) return;

    this.errorMessage = null;
    this.authService.login(this.loginForm.getRawValue()).subscribe({
      next: () => {
        this.router.navigate(['/all']);
      },
      error: (error) => {
        this.errorMessage = getAuthErrorMessage(error, 'Invalid email or password.');
      }
    });
  }
}
