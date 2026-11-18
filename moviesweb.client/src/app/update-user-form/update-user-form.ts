import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, getAuthErrorMessage } from '../services/auth-service';
import { UserProfile } from '../../DTOs/user.models';

@Component({
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  standalone: true,
  selector: 'app-update-user-form',
  styleUrl: './update-user-form.css',
  templateUrl: './update-user-form.html',
})
export class UpdateUserForm implements OnInit {
  private fb = inject(FormBuilder).nonNullable;
  private authService = inject(AuthService);
  private router = inject(Router);
  user = this.authService.currentUser;
  profile = signal<UserProfile | null>(null);
  profileLoading = signal(true);
  profileErrorMessage = signal<string | null>(null);
  errorMessage: string | null = null;
  successMessage: string | null = null;
  submitted = false;

  updateForm = this.fb.group({
    Email: ['', [
      Validators.required,
      Validators.email,
      (control: AbstractControl) => {
        const currentEmail = this.user()?.email;
        const enteredEmail = typeof control.value === 'string' ? control.value.trim() : '';
        return !enteredEmail || !currentEmail || enteredEmail.toLowerCase() === currentEmail.toLowerCase()
          ? null
          : { emailMismatch: true };
      }
    ]],
    Password: ['', [
      Validators.required,
      Validators.minLength(6),
      Validators.maxLength(30),
      Validators.pattern(/^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[#?!@$%^&*-]).+$/)
    ]],
    NewLogin: ['', [Validators.minLength(2), Validators.maxLength(70)]],
    NewCity: ['', [Validators.minLength(2), Validators.maxLength(70)]],
    NewGenre: ['', [Validators.minLength(2), Validators.maxLength(70)]],
    NewPassword: ['', [
      Validators.minLength(6),
      Validators.maxLength(30),
      Validators.pattern(/^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[#?!@$%^&*-]).+$/)
    ]]
  }, { validators: this.requireProfileChange });

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.profileLoading.set(true);
    this.profileErrorMessage.set(null);
    this.authService.getCurrentProfile().subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.profileLoading.set(false);
      },
      error: (error) => {
        this.profileErrorMessage.set(getAuthErrorMessage(error, 'Could not load account details. Try again.'));
        this.profileLoading.set(false);
      }
    });
  }

  requireProfileChange(control: AbstractControl): ValidationErrors | null {
    const { NewLogin, NewCity, NewGenre, NewPassword } = control.value;
    return [NewLogin, NewCity, NewGenre, NewPassword].some((value) => typeof value === 'string' && value.trim().length > 0)
      ? null
      : { noChanges: true };
  }

  clearMessages(): void {
    this.errorMessage = null;
    this.successMessage = null;
  }

  onSubmit(): void {
    this.submitted = true;
    if (this.updateForm.invalid || !this.user()) {
      this.updateForm.markAllAsTouched();
      return;
    }

    this.errorMessage = null;
    this.successMessage = null;
    const formValue = this.updateForm.getRawValue();
    this.authService.updateAccount({
      Email: formValue.Email.trim(),
      Password: formValue.Password,
      NewLogin: formValue.NewLogin.trim() || null,
      NewCity: formValue.NewCity.trim() || null,
      NewGenre: formValue.NewGenre.trim() || null,
      NewPassword: formValue.NewPassword || null
    }).subscribe({
      next: () => {
        this.successMessage = 'Your account has been updated.';
        this.submitted = false;
        const profile = this.profile();
        if (profile) {
          this.profile.set({
            ...profile,
            login: formValue.NewLogin.trim() || profile.login,
            city: formValue.NewCity.trim() || profile.city,
            favouriteGenre: formValue.NewGenre.trim() || profile.favouriteGenre
          });
        }
        this.updateForm.controls.Password.reset();
        this.updateForm.controls.NewPassword.reset();
        this.updateForm.controls.NewLogin.reset();
        this.updateForm.controls.NewCity.reset();
        this.updateForm.controls.NewGenre.reset();
      },
      error: (error) => {
        this.errorMessage = getAuthErrorMessage(error, 'Could not update the account. Check the email and current password, then try again.');
      }
    });
  }

  onDeleteAccount(): void {
    this.submitted = false;
    const user = this.user();
    const emailControl = this.updateForm.controls.Email;
    const password = this.updateForm.controls.Password.value;
    if (!user || emailControl.invalid || this.updateForm.controls.Password.invalid) {
      emailControl.markAsTouched();
      this.updateForm.controls.Password.markAsTouched();
      return;
    }

    if (!window.confirm('Are you sure you want to permanently delete your account?')) return;

    this.errorMessage = null;
    this.successMessage = null;
    this.authService.deleteAccount({ Email: emailControl.value.trim(), Password: password }).subscribe({
      next: () => this.router.navigate(['/register']),
      error: (error) => {
        this.errorMessage = getAuthErrorMessage(error, 'Could not delete the account. Check the email and password, then try again.');
      }
    });
  }
}
