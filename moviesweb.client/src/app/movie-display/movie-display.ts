import { Component, inject, OnInit, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MovieService } from '../services/movie-service';
import { CommentService } from '../services/comment-service';
import { AuthService } from '../services/auth-service';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-movie-display',
  styleUrl: './movie-display.css',
  templateUrl: './movie-display.html',
})
export class MovieDisplay implements OnInit {
  private movieService = inject(MovieService);
  private commentService = inject(CommentService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  isLoading = signal<boolean>(true);
  errorMessage: string | null = null;
  isEditing = signal(false);
  isSaving = signal(false);
  isDeletingMovie = signal(false);
  deleteMovieError = signal<string | null>(null);
  isSubmittingComment = signal(false);
  deletingCommentId = signal<number | null>(null);
  editError = signal<string | null>(null);
  commentError = signal<string | null>(null);
  deleteCommentError = signal<string | null>(null);

  movie = signal<MovieResponse | undefined>(undefined);
  private id = 0;
  commentForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(50)]],
    text: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(250)]]
  });
  editForm = this.fb.nonNullable.group({
    description: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(400)]],
    genre: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]],
    releaseYear: [new Date().getFullYear(), [Validators.required, Validators.min(1890), Validators.max(new Date().getFullYear() + 1)]],
    rating: [8, [Validators.required, Validators.min(0.1), Validators.max(10)]],
    posterUrl: [''],
    movieUrl: ['']
  });

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = Number(params.get('id'));
      if (!Number.isInteger(id) || id < 1) {
        this.errorMessage = 'Invalid movie ID.';
        this.isLoading.set(false);
        return;
      }

      this.id = id;
      this.loadMovie();
    });
  }

  loadMovie(): void {
    this.isLoading.set(true);
    this.errorMessage = null;

    this.movieService.getMovieById(this.id).subscribe({
      next: (data: MovieResponse) => {
        this.movie.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error fetching movie:', err);
        this.errorMessage = 'Failed to load movie. Please try again later.';
        this.isLoading.set(false);
      }
    });
  }

  startEditing(): void {
    const movie = this.movie();
    if (!movie) return;

    this.editForm.patchValue({
      description: movie.description,
      genre: movie.genre,
      releaseYear: movie.releaseYear,
      rating: movie.rating,
      posterUrl: movie.posterUrl ?? '',
      movieUrl: movie.movieUrl ?? ''
    });
    this.editError.set(null);
    this.isEditing.set(true);
  }

  saveMovie(): void {
    const movie = this.movie();
    if (!movie) return;
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    const value = this.editForm.getRawValue();
    const update: MovieUpdate = {
      description: value.description.trim(),
      genre: value.genre.trim(),
      releaseYear: value.releaseYear,
      rating: value.rating,
      posterUrl: value.posterUrl.trim() || null,
      movieUrl: value.movieUrl.trim() || null
    };

    this.isSaving.set(true);
    this.editError.set(null);
    this.movieService.updateMovie(update, movie.id).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.isEditing.set(false);
        this.loadMovie();
      },
      error: (error: unknown) => {
        console.error('Error updating movie:', error);
        this.editError.set('Could not update movie. Check the values and try again.');
        this.isSaving.set(false);
      }
    });
  }

  cancelEditing(): void {
    this.isEditing.set(false);
    this.editError.set(null);
  }

  deleteMovie(): void {
    const movie = this.movie();
    if (!movie || this.isDeletingMovie()) return;
    if (!window.confirm(`Delete "${movie.title}"? This cannot be undone.`)) return;

    this.isDeletingMovie.set(true);
    this.deleteMovieError.set(null);
    this.movieService.deleteMovie(movie.id).subscribe({
      next: () => {
        this.isDeletingMovie.set(false);
        this.router.navigate(['/all']);
      },
      error: (error: unknown) => {
        console.error('Error deleting movie:', error);
        this.deleteMovieError.set('Could not delete movie. Please try again.');
        this.isDeletingMovie.set(false);
      }
    });
  }

  submitComment(): void {
    const movie = this.movie();
    const user = this.authService.currentUser();
    if (!movie) return;
    if (this.commentForm.invalid) {
      this.commentForm.markAllAsTouched();
      return;
    }
    if (!user) {
      this.commentError.set('Sign in to add a comment.');
      return;
    }

    const userId = Number(user.id);
    if (!Number.isSafeInteger(userId) || userId < 1) {
      this.commentError.set('Your user session is invalid. Sign in again.');
      return;
    }

    const value = this.commentForm.getRawValue();
    const title = value.title.trim();
    const text = value.text.trim();
    if (!title || text.length < 2) {
      this.commentError.set('Enter a comment title and at least two characters of text.');
      return;
    }

    this.isSubmittingComment.set(true);
    this.commentError.set(null);
    this.commentService.addComment(
      { title, text },
      movie.id,
      userId
    ).subscribe({
      next: () => {
        this.commentForm.reset();
        this.isSubmittingComment.set(false);
        this.loadMovie();
      },
      error: (error: unknown) => {
        console.error('Error adding comment:', error);
        this.commentError.set('Could not add comment. Check the title and text, then try again.');
        this.isSubmittingComment.set(false);
      }
    });
  }

  deleteComment(commentId: number): void {
    if (!Number.isSafeInteger(commentId) || commentId < 1) return;
    if (!window.confirm('Delete this comment?')) return;

    this.deletingCommentId.set(commentId);
    this.deleteCommentError.set(null);
    this.commentService.deleteComment(commentId).subscribe({
      next: () => {
        this.deletingCommentId.set(null);
        this.loadMovie();
      },
      error: (error: unknown) => {
        console.error('Error deleting comment:', error);
        this.deleteCommentError.set('Could not delete comment. Please try again.');
        this.deletingCommentId.set(null);
      }
    });
  }
}
