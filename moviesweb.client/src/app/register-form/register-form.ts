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
    Email: ['', [Validators.required, Validators.email, Validators.minLength(2), Validators.maxLength(40)]],
    Age: [18, [Validators.required, Validators.min(16), Validators.max(120)]],
    City: ['', [Validators.minLength(2), Validators.maxLength(40)]],
    FavouriteGenre: ['', [Validators.minLength(2), Validators.maxLength(70)]],
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
    return !confirmPassword || password === confirmPassword ? null : { passwordsMismatch: true };
  }

  clearMessages(): void {
    this.errorMessage = null;
  }

  onSubmit() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.errorMessage = null;
    const formValue = this.registerForm.getRawValue();
    this.authService.signUp({
      ...formValue,
      City: formValue.City.trim() || null,
      FavouriteGenre: formValue.FavouriteGenre.trim() || null
    }).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: (error) => {
        this.errorMessage = getAuthErrorMessage(error, 'Could not create your account. Review the fields and try again.');
      }
    });
  }
}
