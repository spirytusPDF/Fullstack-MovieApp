import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { LoginForm } from './login-form';

describe('LoginForm', () => {
  let component: LoginForm;
  let fixture: ComponentFixture<LoginForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginForm],
      providers: [provideHttpClient(), provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('immediately explains an invalid email', () => {
    component.loginForm.controls.Email.setValue('not-an-email');
    component.loginForm.controls.Email.markAsDirty();
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Enter a valid email address.');
  });

  it('shows all required reasons after one invalid submit attempt', () => {
    component.onSubmit();
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent;
    expect(text).toContain('Email is required.');
    expect(text).toContain('Password is required.');
  });
});
