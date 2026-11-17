import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MovieSessionResponse, SessionFilters, SessionUpdate } from '../../DTOs/session.model';
import { AuthService } from '../services/auth-service';
import { SessionService, SessionSortOrder } from '../services/session-service';

function toLocalDateTimeValue(value: string): string {
  if (!/(Z|[+-]\d{2}:\d{2})$/i.test(value)) return value.slice(0, 16);

  const date = new Date(value);
  const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60_000);
  return localDate.toISOString().slice(0, 16);
}

@Component({
  imports: [DatePipe, DecimalPipe, ReactiveFormsModule, RouterLink],
  selector: 'app-session-display',
  styleUrl: './session-display.css',
  templateUrl: './session-display.html'
})
export class SessionDisplay implements OnInit {
  private readonly sessionService = inject(SessionService);
  private readonly route = inject(ActivatedRoute);
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private activeFilters: SessionFilters = {};
  private activeSort: SessionSortOrder = 'soon';

  readonly isAdmin = computed(() => this.authService.currentUser()?.roles.includes('Admin') ?? false);
  readonly movieId = signal<number | null>(null);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly updateError = signal<string | null>(null);
  readonly isUpdating = signal(false);
  readonly deleteError = signal<string | null>(null);
  readonly deletingSessionId = signal<number | null>(null);
  readonly sessions = signal<MovieSessionResponse[]>([]);
  readonly selectedSession = signal<MovieSessionResponse | null>(null);
  readonly editingSession = signal<MovieSessionResponse | null>(null);
  readonly currentPage = signal(1);
  readonly totalCount = signal(0);
  readonly totalPages = signal(1);
  readonly hasPrevious = signal(false);
  readonly hasNext = signal(false);
  readonly pageNumbers = computed(() => {
    const total = this.totalPages();
    const current = this.currentPage();
    const start = Math.max(1, Math.min(current - 2, total - 4));
    const end = Math.min(total, start + 4);
    return Array.from({ length: end - start + 1 }, (_, index) => start + index);
  });

  readonly filterForm = this.formBuilder.group({
    movieTitle: [''],
    hallName: [''],
    ticketPrice: [null as number | null, [Validators.min(0)]],
    availableSeats: [null as number | null, [Validators.min(0), Validators.pattern(/^\d+$/)]],
    sort: ['soon' as SessionSortOrder]
  });
  readonly editForm = this.formBuilder.nonNullable.group({
    startsAt: ['', Validators.required],
    hallName: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(400)]],
    ticketPrice: [0.1, [Validators.required, Validators.min(0.01)]]
  });

  ngOnInit(): void {
    const params = this.route.snapshot.queryParamMap;
    const movieId = Number(params.get('movieId'));
    this.movieId.set(Number.isSafeInteger(movieId) && movieId > 0 ? movieId : null);
    this.filterForm.patchValue({
      movieTitle: params.get('movieTitle') ?? '',
      sort: 'soon'
    });
    this.applyFilters();
  }

  applyFilters(): void {
    if (this.filterForm.invalid) {
      this.filterForm.markAllAsTouched();
      return;
    }

    const values = this.filterForm.getRawValue();
    const filters: SessionFilters = {};
    const movieTitle = values.movieTitle?.trim();
    const hallName = values.hallName?.trim();
    if (movieTitle) filters.movieTitle = movieTitle;
    if (hallName) filters.hallName = hallName;
    if (values.ticketPrice !== null) filters.ticketPrice = values.ticketPrice;
    if (values.availableSeats !== null) filters.availableSeats = values.availableSeats;

    this.activeFilters = filters;
    this.activeSort = values.sort ?? 'soon';
    this.loadSessions(1);
  }

  clearFilters(): void {
    this.filterForm.reset({
      movieTitle: '',
      hallName: '',
      ticketPrice: null,
      availableSeats: null,
      sort: 'soon'
    });
    this.applyFilters();
  }

  loadSessions(page = this.currentPage()): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.deleteError.set(null);
    this.selectedSession.set(null);
    this.editingSession.set(null);

    this.sessionService.getSessions(this.activeFilters, page, 9, this.activeSort).subscribe({
      next: (data) => {
        this.sessions.set(data.sessions);
        this.currentPage.set(data.pagination.page);
        this.totalCount.set(data.pagination.totalCount);
        this.totalPages.set(data.pagination.totalPages);
        this.hasPrevious.set(data.pagination.hasPrevious);
        this.hasNext.set(data.pagination.hasNext);
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        console.error('Error fetching sessions:', error);
        this.errorMessage.set('Failed to load sessions. Please try again later.');
        this.isLoading.set(false);
      }
    });
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages() && page !== this.currentPage()) {
      this.loadSessions(page);
    }
  }

  selectSession(session: MovieSessionResponse): void {
    this.editingSession.set(null);
    this.selectedSession.set(session);
  }

  startEditing(session: MovieSessionResponse): void {
    this.editForm.patchValue({
      startsAt: toLocalDateTimeValue(session.startsAt),
      hallName: session.hallName,
      ticketPrice: session.ticketPrice
    });
    this.selectedSession.set(null);
    this.editingSession.set(session);
    this.updateError.set(null);
  }

  cancelEditing(): void {
    this.editingSession.set(null);
    this.updateError.set(null);
  }

  saveSessionChanges(): void {
    const session = this.editingSession();
    if (!session) return;
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    const values = this.editForm.getRawValue();
    const startsAt = new Date(values.startsAt);
    const maxDate = new Date();
    maxDate.setFullYear(maxDate.getFullYear() + 1);
    if (!Number.isFinite(startsAt.getTime()) || startsAt <= new Date() || startsAt >= maxDate) {
      this.editForm.controls.startsAt.setErrors({ dateRange: true });
      this.editForm.controls.startsAt.markAsTouched();
      return;
    }

    const update: SessionUpdate = {
      startsAt: values.startsAt,
      hallName: values.hallName.trim(),
      ticketPrice: values.ticketPrice
    };

    this.isUpdating.set(true);
    this.updateError.set(null);
    this.sessionService.updateSession(session.id, update).subscribe({
      next: () => {
        this.isUpdating.set(false);
        this.editingSession.set(null);
        this.loadSessions();
      },
      error: (error: unknown) => {
        console.error('Error updating session:', error);
        this.updateError.set('Could not update session. Please check the values and try again.');
        this.isUpdating.set(false);
      }
    });
  }

  deleteSession(session: MovieSessionResponse): void {
    if (this.deletingSessionId() !== null) return;
    if (!window.confirm(`Delete the session for "${session.movie.title}"? This cannot be undone.`)) return;

    this.deletingSessionId.set(session.id);
    this.deleteError.set(null);
    this.sessionService.deleteSession(session.id).subscribe({
      next: () => {
        this.deletingSessionId.set(null);
        this.loadSessions();
      },
      error: (error: unknown) => {
        console.error('Error deleting session:', error);
        this.deleteError.set('Could not delete session. Please try again.');
        this.deletingSessionId.set(null);
      }
    });
  }
}
