import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth-service';

@Component({
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  standalone:true,
  selector: 'app-login-form',
  styleUrl: './login-form.css',
  templateUrl: './login-form.html',
})
export class LoginForm {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  errorMessage: string | null = null;

  loginForm = this.fb.group({
    login: ['', Validators.required],
    password: ['', Validators.required],
     email: ['', Validators.required]
  });

  onSubmit() {
    console.log("Entered onsubmit");
    
    if (this.loginForm.invalid) return;

    this.errorMessage = null;
    this.authService.login(this.loginForm.value as any).subscribe({
      next: () => {
        console.log('Login succeeded; navigating to /all');
        this.router.navigate(['/all']).then(
        (navigated) => console.log('Navigation completed:', navigated),
        (error) => console.error('Navigation failed:', error)
        );
      },
      error: (err) => {
    console.error('Login request error:', err);
    this.errorMessage = err.error?.message || 'Invalid login or password.';
    }
    });
  }
}
