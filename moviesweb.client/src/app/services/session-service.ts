import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { MovieSessionResponse, SessionCreate, SessionFilters, SessionPage, SessionUpdate } from '../../DTOs/session.model';

export type SessionSortOrder = 'soon' | 'rating';



@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/sessions';

  getSessions(
    filters: SessionFilters = {},
    page = 1,
    pageSize = 9,
    sort: SessionSortOrder = 'soon'
  ): Observable<SessionPage> {
    let params = new HttpParams()
      .set('Page', page)
      .set('PageSize', pageSize)
      .set('Sort', sort);

    for (const [key, value] of Object.entries(filters)) {
      if (value !== undefined && value !== null && value !== '') {
        const parameterName = key[0].toUpperCase() + key.slice(1);
        params = params.set(parameterName, value);
      }
    }

    return this.http.get<MovieSessionResponse[]>(`${this.apiUrl}/view`, {
      params,
      observe: 'response'
    }).pipe(map((response) => this.readSessionPage(response)));
  }

  private readSessionPage(response: HttpResponse<MovieSessionResponse[]>): SessionPage {
    if (response.body === null) {
      throw new Error('The sessions endpoint returned an empty response body.');
    }

    const paginationHeader = response.headers.get('X-Pagination');
    if (paginationHeader === null) {
      throw new Error('The sessions endpoint did not return the X-Pagination header.');
    }

    let metadata: unknown;
    try {
      metadata = JSON.parse(paginationHeader);
    } catch (error) {
      throw new Error('The sessions endpoint returned invalid pagination metadata.', { cause: error });
    }

    if (typeof metadata !== 'object' || metadata === null) {
      throw new Error('The sessions endpoint returned invalid pagination metadata.');
    }

    const values = metadata as Record<string, unknown>;
    const totalCount = values['totalCount'] ?? values['TotalCount'];
    const page = values['page'] ?? values['Page'];
    const pageSize = values['pageSize'] ?? values['PageSize'];
    if (!Number.isInteger(totalCount) || (totalCount as number) < 0
      || !Number.isInteger(page) || (page as number) < 1
      || !Number.isInteger(pageSize) || (pageSize as number) < 1) {
      throw new Error('The sessions endpoint returned invalid pagination metadata.');
    }

    const totalPages = Math.max(1, Math.ceil((totalCount as number) / (pageSize as number)));
    return {
      sessions: response.body,
      pagination: {
        totalCount: totalCount as number,
        page: page as number,
        pageSize: pageSize as number,
        totalPages,
        hasPrevious: (page as number) > 1,
        hasNext: (page as number) < totalPages
      }
    };
  }

  createNewSession(model: SessionCreate, movieId: number): Observable<void> {
    const params = new HttpParams().set('movieId', movieId);
    return this.http.post<void>(`${this.apiUrl}/newSession`, model, { params });
  }

  updateSession(id: number, model: SessionUpdate): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/update/${id}`, model);
  }

  deleteSession(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/delete/${id}`, {});
  }
}
