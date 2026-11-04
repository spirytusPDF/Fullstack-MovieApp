import { Routes } from '@angular/router';
import { RegisterForm } from './register-form/register-form';
import { LoginForm } from './login-form/login-form';
import { Cathalog } from './cathalog/cathalog';
import { MovieDisplay } from './movie-display/movie-display';
import { AddMovieForm } from './add-movie-form/add-movie-form';


export const routes: Routes = [
  
  { path: '', redirectTo: 'register', pathMatch: 'full' },
  { path: 'register', component: RegisterForm },
  { path: 'login', component: LoginForm },
  { path: 'all', component: Cathalog },
  { path: 'newMovie', component: AddMovieForm },
  { path: ':id', component: MovieDisplay },
];