import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpResponse } from '@angular/common/http';
import { BehaviorSubject, map, Observable } from 'rxjs';

export type MovieSortOrder = 'none' | 'latest' | 'older' | 'rating';

export interface MovieCatalogQuery {
  filters: Partial<MovieFilterParams>;
  sort: MovieSortOrder;
  page: number;
  pageSize: number;
}

export interface MoviePagination {
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface MoviePage {
  movies: MovieResponse[];
  pagination: MoviePagination;
}

@Injectable({
  providedIn: 'root'
})
export class MovieService {
  private apiUrl = '/api/movies';
  private readonly catalogQuerySubject = new BehaviorSubject<MovieCatalogQuery>({
    filters: {},
    sort: 'none',
    page: 1,
    pageSize: 6
  });
  readonly catalogQuery = this.catalogQuerySubject.asObservable();
  constructor(private http: HttpClient) {}

  getMovieById(id: number): Observable<MovieResponse> {
    return this.http.get<MovieResponse>(`${this.apiUrl}/${id}`);
  }
  getFilteredMovies(
    filters: Partial<MovieFilterParams> = {},
    page = 1,
    pageSize = 6,
    sort: MovieSortOrder = 'none'
  ): Observable<MoviePage> {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(filters)) {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, value);
      }
    }
    params = params
      .set('Page', page)
      .set('PageSize', pageSize)
      .set('Sort', sort);
    return this.http.get<MovieResponse[]>(`${this.apiUrl}/view`, {
      params,
      withCredentials: true,
      observe: 'response'
    }).pipe(
      map((response) => this.readMoviePage(response))
    );
  }

  setFilters(filters: Partial<MovieFilterParams>): void {
    this.catalogQuerySubject.next({ ...this.catalogQuerySubject.value, filters, page: 1 });
  }

  setSort(sort: MovieSortOrder): void {
    this.catalogQuerySubject.next({ ...this.catalogQuerySubject.value, sort, page: 1 });
  }

  setPage(page: number): void {
    this.catalogQuerySubject.next({ ...this.catalogQuerySubject.value, page });
  }

  resetCatalog(): void {
    this.catalogQuerySubject.next({
      filters: {},
      sort: 'none',
      page: 1,
      pageSize: this.catalogQuerySubject.value.pageSize
    });
  }

  refreshCatalog(): void {
    this.catalogQuerySubject.next({ ...this.catalogQuerySubject.value });
  }

  private readMoviePage(response: HttpResponse<MovieResponse[]>): MoviePage {
    if (response.body === null) {
      throw new Error('The movies endpoint returned an empty response body.');
    }

    const paginationHeader = response.headers.get('X-Pagination');
    if (paginationHeader === null) {
      throw new Error('The movies endpoint did not return the X-Pagination header.');
    }

    let metadata: unknown;
    try {
      metadata = JSON.parse(paginationHeader);
    } catch (error) {
      throw new Error('The movies endpoint returned invalid pagination metadata.', { cause: error });
    }

    if (!this.isPaginationMetadata(metadata)) {
      throw new Error('The movies endpoint returned invalid pagination metadata.');
    }

    const totalPages = Math.max(1, Math.ceil(metadata.totalCount / metadata.pageSize));
    return {
      movies: response.body,
      pagination: {
        ...metadata,
        totalPages,
        hasPrevious: metadata.page > 1,
        hasNext: metadata.page < totalPages
      }
    };
  }

  private isPaginationMetadata(value: unknown): value is Pick<MoviePagination, 'totalCount' | 'page' | 'pageSize'> {
    if (typeof value !== 'object' || value === null) return false;
    const metadata = value as Record<string, unknown>;
    const totalCount = metadata['totalCount'] ?? metadata['TotalCount'];
    const page = metadata['page'] ?? metadata['Page'];
    const pageSize = metadata['pageSize'] ?? metadata['PageSize'];
    if (!Number.isInteger(totalCount) || (totalCount as number) < 0
      || !Number.isInteger(page) || (page as number) < 1
      || !Number.isInteger(pageSize) || (pageSize as number) < 1) {
      return false;
    }

    metadata['totalCount'] = totalCount;
    metadata['page'] = page;
    metadata['pageSize'] = pageSize;
    return true;
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