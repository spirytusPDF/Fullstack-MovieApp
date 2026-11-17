import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SessionCreate } from '../../DTOs/session.model';
import { SessionService } from '../services/session-service';

function toLocalDateTime(date: Date): string {
  const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60_000);
  return localDate.toISOString().slice(0, 16);
}

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-add-session-form',
  styleUrl: './add-session-form.css',
  templateUrl: './add-session-form.html',
})
export class AddSessionForm implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly sessionService = inject(SessionService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  movieId: number | null = null;
  movieTitle = '';
  isSubmitting = false;
  errorMessage: string | null = null;
  readonly minStartsAt = toLocalDateTime(new Date(Date.now() + 60_000));
  readonly maxStartsAt = (() => {
    const maxDate = new Date();
    maxDate.setFullYear(maxDate.getFullYear() + 1);
    return toLocalDateTime(maxDate);
  })();

  readonly sessionForm = this.fb.nonNullable.group({
    startsAt: ['', Validators.required],
    hallName: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(400)]],
    ticketPrice: [1, [Validators.required, Validators.min(0.01)]],
    availableSeats: [1, [Validators.required, Validators.min(1)]],
  });

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('movieId'));
    if (!Number.isSafeInteger(id) || id < 1) {
      this.errorMessage = 'Invalid movie ID.';
      return;
    }

    this.movieId = id;
    this.movieTitle = this.route.snapshot.queryParamMap.get('movieTitle') ?? '';
  }

  onSubmit(): void {
    if (!this.movieId) {
      this.errorMessage = 'A movie is required to add a session.';
      return;
    }
    if (this.sessionForm.invalid) {
      this.sessionForm.markAllAsTouched();
      return;
    }

    const value = this.sessionForm.getRawValue();
    const startsAt = new Date(value.startsAt);
    const maxDate = new Date();
    maxDate.setFullYear(maxDate.getFullYear() + 1);
    if (!Number.isFinite(startsAt.getTime()) || startsAt <= new Date() || startsAt >= maxDate) {
      this.sessionForm.controls.startsAt.setErrors({ dateRange: true });
      this.sessionForm.controls.startsAt.markAsTouched();
      return;
    }

    const session: SessionCreate = {
      startsAt: value.startsAt,
      hallName: value.hallName.trim(),
      ticketPrice: Number(value.ticketPrice),
      availableSeats: Number(value.availableSeats),
    };

    this.isSubmitting = true;
    this.errorMessage = null;
    this.sessionService.createNewSession(session, this.movieId).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/', this.movieId]);
      },
      error: (error: HttpErrorResponse) => {
        console.error('Error adding session:', error);
        this.errorMessage = this.getServerErrorMessage(error);
        this.isSubmitting = false;
      }
    });
  }

  onCancel(): void {
    if (this.movieId) {
      this.router.navigate(['/', this.movieId]);
    } else {
      this.router.navigate(['/all']);
    }
  }

  private getServerErrorMessage(error: HttpErrorResponse): string {
    if (Array.isArray(error.error)) {
      const messages = error.error
        .map((item: unknown) => {
          if (typeof item !== 'object' || item === null) return null;
          const message = (item as Record<string, unknown>)['errorMessage'];
          return typeof message === 'string' ? message : null;
        })
        .filter((message): message is string => message !== null);

      if (messages.length > 0) return messages.join(' ');
    }

    if (typeof error.error === 'string') return error.error;
    return 'Could not save session. Please check the form and try again.';
  }
}
