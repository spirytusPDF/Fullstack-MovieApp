import { Component, inject } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MovieService } from '../services/movie-service';
import { Router } from '@angular/router';

@Component({
  imports: [FormsModule, ReactiveFormsModule],
  selector: 'app-add-movie-form',
  styleUrl: './add-movie-form.css',
  templateUrl: './add-movie-form.html',
})
export class AddMovieForm {
  private fb = inject(FormBuilder);
  private movieService = inject(MovieService);
  private router = inject(Router);

  isSubmitting = false;
  errorMessage: string | null = null;

  movieForm: FormGroup = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    description: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(400)]],
    genre: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]],
    releaseYear: [new Date().getFullYear(), [Validators.required, Validators.min(1890), Validators.max(new Date().getFullYear() + 1)]],
    rating: [8.0, [Validators.required, Validators.min(0.1), Validators.max(10)]],
    posterUrl: [null],
    movieUrl: [null]
  });

  onSubmit(): void {
    if (this.movieForm.invalid) {
      this.movieForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = null;

    this.movieService.createMovie(this.movieForm.value).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/all']);
      },
      error: (err: HttpErrorResponse) => {
        console.error('Error adding movie:', err);
        this.errorMessage = this.getServerErrorMessage(err);
        this.isSubmitting = false;
      }
    });
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
    return 'Could not save movie. Please check the form and try again.';
  }

  onCancel(): void {
    this.router.navigate(['/all']);
  }
}
