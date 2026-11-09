import { Component, computed, DestroyRef, inject, signal } from '@angular/core';
import { MovieCard } from '../movie-card/movie-card';
import { MoviePage, MovieService } from '../services/movie-service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { EMPTY, catchError, debounceTime, switchMap, tap } from 'rxjs';

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
  currentPage = signal(1);
  pageSize = signal(6);
  totalCount = signal(0);
  totalPages = signal(1);
  hasPrevious = signal(false);
  hasNext = signal(false);
  pageNumbers = computed(() => {
    const total = this.totalPages();
    const current = this.currentPage();
    const start = Math.max(1, Math.min(current - 2, total - 4));
    const end = Math.min(total, start + 4);
    return Array.from({ length: end - start + 1 }, (_, index) => start + index);
  });

  ngOnInit(): void {
    this.movieService.catalogQuery.pipe(
      debounceTime(250),
      tap(() => {
        this.isLoading.set(true);
        this.errorMessage = null;
      }),
      switchMap(({ filters, sort, page, pageSize }) => {
        return this.movieService.getFilteredMovies(filters, page, pageSize, sort).pipe(
          tap((data: MoviePage) => {
            this.movies.set(data.movies);
            this.currentPage.set(data.pagination.page);
            this.pageSize.set(data.pagination.pageSize);
            this.totalCount.set(data.pagination.totalCount);
            this.totalPages.set(data.pagination.totalPages);
            this.hasPrevious.set(data.pagination.hasPrevious);
            this.hasNext.set(data.pagination.hasNext);
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
    this.movieService.refreshCatalog();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages() && page !== this.currentPage()) {
      this.movieService.setPage(page);
    }
  }
}
