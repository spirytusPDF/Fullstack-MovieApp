import { Component, DestroyRef, inject, signal } from '@angular/core';
import { MovieCard } from '../movie-card/movie-card';
import { MovieService } from '../services/movie-service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { EMPTY, catchError, debounceTime, map, of, switchMap, tap } from 'rxjs';

@Component({
  imports: [MovieCard],
  standalone:true,
  selector: 'app-cathalog',
  styleUrl: './cathalog.css',
  templateUrl: './cathalog.html',
})
export class Cathalog {
  private movieService = inject(MovieService);
  private destroyRef = inject(DestroyRef);
  movies = signal<MovieResponse[]>([]);
  isLoading = signal<boolean>(true);
  errorMessage: string | null = null;
  private allMovies: MovieResponse[] | null = null;
  ngOnInit(): void {
    this.movieService.searchTerm.pipe(
      map((term) => term.trim()),
      debounceTime(250),
      tap(() => {
        this.isLoading.set(true);
        this.errorMessage = null;
      }),
      switchMap((term) => {
        const request = this.allMovies !== null
          ? of(this.allMovies)
          : this.movieService.getMovies().pipe(
            tap((data) => this.allMovies = data)
          );

        return request.pipe(
          map((data) => this.filterMovies(data, term)),
          tap((data) => {
            this.movies.set(data);
            this.isLoading.set(false);
          }),
          catchError((err: unknown) => {
            console.error('Error fetching movies:', err);
            this.errorMessage = 'Failed to load movies. Please try again later.';
            this.isLoading.set(false);
            return EMPTY;
          })
        );
      })
    ).pipe(takeUntilDestroyed(this.destroyRef)).subscribe();
  }

  loadMovies(): void {
    this.movieService.setSearchTerm(this.movieService.searchTerm.value);
  }

  private filterMovies(movies: MovieResponse[], term: string): MovieResponse[] {
    if (!term) return movies;

    const query = term.toLocaleLowerCase();
    return movies.filter((movie) =>
      movie.title.toLocaleLowerCase().includes(query) ||
      movie.genre.toLocaleLowerCase().includes(query)
    );
  }
}
