import { TestBed } from '@angular/core/testing';
import { HttpClient, HttpErrorResponse, provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { authInterceptor } from './auth-interceptor';
import { AuthService, getAuthErrorMessage } from './auth-service';

describe('AuthService', () => {
  let service: AuthService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(AuthService);
    httpTesting = TestBed.inject(HttpTestingController);
    localStorage.removeItem('movies.auth.token');
  });

  afterEach(() => {
    httpTesting.verify();
    localStorage.removeItem('movies.auth.token');
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('shows backend validation and credential errors instead of a generic fallback', () => {
    const error = new HttpErrorResponse({
      status: 400,
      error: { errors: ['Invalid email or password'] }
    });

    expect(getAuthErrorMessage(error, 'Could not update account.')).toBe('Invalid email or password');
  });

  it('requests the signed-in user profile', () => {
    const token = createToken({
      sub: 'user-id',
      login: 'person',
      email: 'person@example.com',
      exp: Math.floor(Date.now() / 1000) + 3600
    });
    localStorage.setItem('movies.auth.token', token);

    service.getCurrentProfile().subscribe((profile) => {
      expect(profile.login).toBe('person');
    });
    const request = httpTesting.expectOne('/api/auth/me');
    expect(request.request.method).toBe('GET');
    expect(request.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    request.flush({
      id: 'user-id',
      login: 'person',
      userName: 'Person',
      surname: 'Example',
      city: 'Helsinki',
      favouriteGenre: 'Drama'
    });
  });

  it('stores the returned token and restores the current user from its claims', () => {
    const token = createToken({
      sub: 'user-id',
      login: 'person',
      email: 'person@example.com',
      role: 'User',
      exp: Math.floor(Date.now() / 1000) + 3600
    });

    service.login({ Email: 'person@example.com', Password: 'Password1!' }).subscribe();
    const request = httpTesting.expectOne('/api/auth/login');
    expect(request.request.body).toEqual({ Email: 'person@example.com', Password: 'Password1!' });
    request.flush({ token });

    expect(localStorage.getItem('movies.auth.token')).toBe(token);
    expect(service.currentUser()).toEqual({
      id: 'user-id',
      login: 'person',
      email: 'person@example.com',
      roles: ['User']
    });
  });

  it('restores the current user from a persisted token on startup', () => {
    const token = createToken({
      sub: 'user-id',
      login: 'person',
      email: 'person@example.com',
      role: 'Admin',
      exp: Math.floor(Date.now() / 1000) + 3600
    });
    localStorage.setItem('movies.auth.token', token);

    service.checkAuth().subscribe();
    expect(service.currentUser()).toEqual({
      id: 'user-id',
      login: 'person',
      email: 'person@example.com',
      roles: ['Admin']
    });
  });

  it('does not authenticate a user after signup', () => {
    const token = createToken({
      sub: 'user-id',
      login: 'person',
      email: 'person@example.com',
      exp: Math.floor(Date.now() / 1000) + 3600
    });

    service.signUp({
      Login: 'person',
      Password: 'Password1!',
      ConfirmPassword: 'Password1!',
      UserName: 'Person',
      Surname: 'Example',
      Age: 18,
      Email: 'person@example.com'
    }).subscribe();
    httpTesting.expectOne('/api/auth/signup').flush({ token });

    expect(service.currentUser()).toBeNull();
    expect(localStorage.getItem('movies.auth.token')).toBeNull();
  });

  it('updates account details and refreshes the displayed login', () => {
    const token = createToken({
      sub: 'user-id',
      login: 'person',
      email: 'person@example.com',
      exp: Math.floor(Date.now() / 1000) + 3600
    });
    localStorage.setItem('movies.auth.token', token);
    service.checkAuth().subscribe();

    service.updateAccount({
      Email: 'person@example.com',
      Password: 'Password1!',
      NewLogin: 'new-person',
      NewCity: 'Helsinki',
      NewGenre: 'Drama'
    }).subscribe();
    const request = httpTesting.expectOne('/api/auth/update');
    expect(request.request.method).toBe('PUT');
    expect(request.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    request.flush(null);

    expect(service.currentUser()?.login).toBe('new-person');
    service.checkAuth().subscribe();
    expect(service.currentUser()?.login).toBe('new-person');
  });

  it('deletes the current account and clears its session', () => {
    const token = createToken({
      sub: 'user-id',
      login: 'person',
      email: 'person@example.com',
      exp: Math.floor(Date.now() / 1000) + 3600
    });
    localStorage.setItem('movies.auth.token', token);
    service.checkAuth().subscribe();

    service.deleteAccount({ Email: 'person@example.com', Password: 'Password1!' }).subscribe();
    const request = httpTesting.expectOne('/api/auth/delete');
    expect(request.request.method).toBe('DELETE');
    expect(request.request.body).toEqual({ Email: 'person@example.com', Password: 'Password1!' });
    request.flush(null);

    expect(service.currentUser()).toBeNull();
    expect(localStorage.getItem('movies.auth.token')).toBeNull();
  });

  it('adds the bearer token to API requests', () => {
    const token = createToken({
      sub: 'user-id',
      login: 'person',
      email: 'person@example.com',
      role: 'User',
      exp: Math.floor(Date.now() / 1000) + 3600
    });
    localStorage.setItem('movies.auth.token', token);

    TestBed.inject(HttpClient).get('/api/movies/view').subscribe();
    const request = httpTesting.expectOne('/api/movies/view');

    expect(request.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    request.flush([]);
  });
});

function createToken(claims: object): string {
  return `header.${btoa(JSON.stringify(claims))}.signature`;
}
