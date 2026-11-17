import { inject } from '@angular/core';
import { CanActivateFn, Router, Routes } from '@angular/router';
import { map } from 'rxjs';
import { RegisterForm } from './register-form/register-form';
import { LoginForm } from './login-form/login-form';
import { Cathalog } from './cathalog/cathalog';
import { MovieDisplay } from './movie-display/movie-display';
import { SessionDisplay } from './session-display/session-display';
import { AddMovieForm } from './add-movie-form/add-movie-form';
import { AddSessionForm } from './add-session-form/add-session-form';
import { AuthService } from './services/auth-service';

const requireAuth: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.checkAuth().pipe(
    map((user) => user ? true : router.createUrlTree(['/register']))
  );
};

const requireAdmin: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.checkAuth().pipe(
    map((user) => {
      if (!user) return router.createUrlTree(['/register']);
      return user.roles.includes('Admin') ? true : router.createUrlTree(['/all']);
    })
  );
};

export const routes: Routes = [
  {
    path: '',
    redirectTo: () => inject(AuthService).getAccessToken() ? '/all' : '/register',
    pathMatch: 'full'
  },
  { path: 'register', component: RegisterForm },
  { path: 'login', component: LoginForm },
  { path: 'all', component: Cathalog, canActivate: [requireAuth] },
  { path: 'newMovie', component: AddMovieForm, canActivate: [requireAdmin] },
  { path: 'newSession/:movieId', component: AddSessionForm, canActivate: [requireAdmin] },
  { path: 'sessions', component: SessionDisplay, canActivate: [requireAuth] },
  { path: ':id', component: MovieDisplay, canActivate: [requireAuth] },
];