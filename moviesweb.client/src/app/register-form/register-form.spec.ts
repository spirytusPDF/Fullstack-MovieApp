import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { RegisterForm } from './register-form';

describe('RegisterForm', () => {
  let component: RegisterForm;
  let fixture: ComponentFixture<RegisterForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterForm],
      providers: [provideHttpClient(), provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('immediately explains a too-short login', () => {
    component.registerForm.controls.Login.setValue('a');
    component.registerForm.controls.Login.markAsDirty();
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Login must be at least 2 characters.');
  });

  it('shows all required reasons after one invalid submit attempt', () => {
    component.onSubmit();
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent;
    expect(text).toContain('Login is required.');
    expect(text).toContain('Name is required.');
    expect(text).toContain('Surname is required.');
    expect(text).toContain('Email is required.');
    expect(text).toContain('Password is required.');
    expect(text).toContain('Please confirm your password.');
  });

  it('immediately explains a password confirmation mismatch', () => {
    component.registerForm.controls.Password.setValue('Password1!');
    component.registerForm.controls.ConfirmPassword.setValue('Password2!');
    component.registerForm.controls.ConfirmPassword.markAsDirty();
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Passwords do not match.');
  });
});
