import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth-service';
import { Router, RouterLink } from '@angular/router';
import { MovieService } from '../services/movie-service';

@Component({
  imports: [CommonModule, FormsModule, RouterLink],
  selector: 'app-header',
  standalone: true,
  styleUrl: './header.css',
  templateUrl: './header.html',
})
export class Header {
  searchQuery!:string;
  router = inject(Router);
  private authService = inject(AuthService);
  private movieService = inject(MovieService);
  user = this.authService.currentUser;
  onSearch(){
    this.movieService.setSearchTerm(this.searchQuery);
  }
  openAddMovieModal() {
    this.router.navigate(['/newMovie'])
  }
  onLogout() {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/login']),
      error: (error: unknown) => console.error('Could not log out:', error)
    });
  }
}
