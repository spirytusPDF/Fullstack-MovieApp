import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { AuthService } from '../services/auth-service';
import { UpdateUserForm } from './update-user-form';

describe('UpdateUserForm', () => {
  let component: UpdateUserForm;
  let fixture: ComponentFixture<UpdateUserForm>;
  let httpTesting: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateUserForm],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])]
    }).compileComponents();

    httpTesting = TestBed.inject(HttpTestingController);
    TestBed.inject(AuthService).currentUser.set({
      id: 'user-id',
      login: 'person',
      email: 'person@example.com',
      roles: ['User']
    });

    fixture = TestBed.createComponent(UpdateUserForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
    httpTesting.expectOne('/api/auth/me').flush({
      id: 'user-id',
      login: 'person',
      userName: 'Person',
      surname: 'Example',
      city: 'Helsinki',
      favouriteGenre: 'Drama'
    });
    await fixture.whenStable();
    expect(component.profileLoading()).toBe(false);
    fixture.detectChanges();
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('immediately explains an email that does not match the signed-in account', () => {
    const emailControl = component.updateForm.controls.Email;
    emailControl.setValue('other@example.com');
    emailControl.markAsDirty();
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain(
      'Enter the email for the signed-in account (person@example.com).'
    );
  });

  it('shows all missing required reasons after an invalid submit attempt', () => {
    component.onSubmit();
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent;
    expect(text).toContain('Enter your account email.');
    expect(text).toContain('Enter your current password.');
    expect(text).toContain('Enter at least one profile change.');
  });

  it('displays the profile properties returned by the API', () => {
    const text = fixture.nativeElement.textContent;

    expect(text).toContain('person');
    expect(text).toContain('Person');
    expect(text).toContain('Example');
    expect(text).toContain('Helsinki');
    expect(text).toContain('Drama');
  });
});
