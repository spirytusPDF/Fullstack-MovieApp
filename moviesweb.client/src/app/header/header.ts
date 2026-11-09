import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth-service';
import { Router, RouterLink } from '@angular/router';
import { MovieService } from '../services/movie-service';

type MovieSearchField = 'Key' | 'Id' | 'Title' | 'Genre' | 'Description' | 'Year' | 'Rating';

@Component({
  imports: [CommonModule, FormsModule, RouterLink],
  selector: 'app-header',
  standalone: true,
  styleUrl: './header.css',
  templateUrl: './header.html',
})
export class Header {
  router = inject(Router);
  private authService = inject(AuthService);
  private movieService = inject(MovieService);
  user = this.authService.currentUser;
  searchQuery = '';
  searchField: MovieSearchField = 'Key';
  selectedGenre = '';
  readonly genres = ['Drama', 'Comedy', 'Detective', 'Thriller', 'Musical', 'Romantic', 'Fantasy', 'Action'];

  onSearch(): void {
    const query = this.searchQuery.trim();
    this.selectedGenre = '';
    if (!query) {
      this.movieService.setFilters({});
      return;
    }

    switch (this.searchField) {
      case 'Id': {
        const id = Number(query);
        this.movieService.setFilters(Number.isInteger(id) && id > 0 ? { Id: id } : {});
        break;
      }
      case 'Title':
        this.movieService.setFilters({ Title: query });
        break;
      case 'Genre':
        this.movieService.setFilters({ Genre: query });
        break;
      case 'Description':
        this.movieService.setFilters({ Description: query });
        break;
      case 'Year':
        this.movieService.setFilters({ ReleaseYear: query });
        break;
      case 'Rating': {
        const rating = Number(query);
        this.movieService.setFilters(Number.isFinite(rating) ? { Rating: rating } : {});
        break;
      }
      default:
        this.movieService.setFilters({ Key: query });
    }
  }

  onGenreChange(genre: string): void {
    this.selectedGenre = genre;
    this.searchQuery = '';
    this.searchField = 'Key';
    if (genre) {
      this.movieService.setFilters({ Genre: genre });
    } else {
      this.movieService.resetCatalog();
    }
  }

  setSort(sort: 'latest' | 'older' | 'rating'): void {
    this.movieService.setSort(sort);
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
