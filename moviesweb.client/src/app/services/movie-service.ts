import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MovieService {
  private apiUrl = '/api/movies';
  searchTerm = new BehaviorSubject('');

  constructor(private http: HttpClient) {}

  getMovies(): Observable<MovieResponse[]> {
    return this.http.get<MovieResponse[]>(`${this.apiUrl}/all`,{ withCredentials: true });
  }

  getMovieById(id: number): Observable<MovieResponse> {
    return this.http.get<MovieResponse>(`${this.apiUrl}/${id}`);
  }
  searchMovies(key: string): Observable<MovieResponse[]> {
    const params = new HttpParams().set('keyword', key);
    return this.http.get<MovieResponse[]>(`${this.apiUrl}/search`, { params, withCredentials: true });
  }

  setSearchTerm(term: string): void {
    this.searchTerm.next(term);
  }

  createMovie(movie: MovieAdd): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/newMovie`, movie, { withCredentials: true });
  }
  updateMovie(movie: MovieUpdate, id:number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/update/${id}`, movie, { withCredentials: true });
  }
  deleteMovie(id:number): Observable<void>{
    return this.http.post<void>(`${this.apiUrl}/delete/${id}`, {}, { withCredentials: true });
  }
}