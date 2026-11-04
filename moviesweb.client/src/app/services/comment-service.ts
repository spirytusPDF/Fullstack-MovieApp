import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private apiUrl = '/api/comments';

  constructor(private http: HttpClient) {}

  addComment(comment: CommentAdd, movieId: number, userId: number): Observable<void> {
    const params = new HttpParams()
      .set('movieId', movieId)
      .set('userId', userId);

    return this.http.post<void>(this.apiUrl, comment, { params, withCredentials: true });
  }

  deleteComment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { withCredentials: true });
  }
}