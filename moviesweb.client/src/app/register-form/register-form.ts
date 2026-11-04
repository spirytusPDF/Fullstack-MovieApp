import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth-service';

@Component({
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  standalone:true,
  selector: 'app-register-form',
  styleUrl: './register-form.css',
  templateUrl: './register-form.html',
})
export class RegisterForm {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  errorMessage: string | null = null;
  
  registerForm = this.fb.group({
    Login: ['', [Validators.required, Validators.minLength(3)]],
    Name: ['', Validators.required],
    Surname: ['', Validators.required],
    Email: ['', [Validators.required, Validators.email]],
    Age: [18, [Validators.required, Validators.min(10)]],
    Password: ['', [Validators.required, Validators.minLength(6)]],
    ConfirmPassword: ['', Validators.required]
  }, { validators: this.passwordMatchValidator });

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('Password')?.value;
    const confirmPassword = control.get('ConfirmPassword')?.value;
    return password === confirmPassword ? null : { passwordsMismatch: true };
  }
  onSubmit() {
    if (this.registerForm.invalid) return;

    this.errorMessage = null;
    this.authService.signUp(this.registerForm.value as any).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to register. Check your input.';
      }
    });
  }
}
