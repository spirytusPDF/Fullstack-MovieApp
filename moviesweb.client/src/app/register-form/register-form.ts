import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, getAuthErrorMessage } from '../services/auth-service';

@Component({
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  standalone:true,
  selector: 'app-register-form',
  styleUrl: './register-form.css',
  templateUrl: './register-form.html',
})
export class RegisterForm {
  private fb = inject(FormBuilder).nonNullable;
  private authService = inject(AuthService);
  private router = inject(Router);
  errorMessage: string | null = null;
  
  registerForm = this.fb.group({
    Login: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]],
    UserName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]],
    Surname: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]],
    Email: ['', [Validators.required, Validators.email, Validators.maxLength(40)]],
    Age: [18, [Validators.required, Validators.min(16), Validators.max(120)]],
    Password: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.maxLength(30),
      Validators.pattern(/^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[#?!@$%^&*-]).+$/)
    ]],
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
    this.authService.signUp(this.registerForm.getRawValue()).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: (error) => {
        this.errorMessage = getAuthErrorMessage(error, 'Failed to register. Check your input.');
      }
    });
  }
}
