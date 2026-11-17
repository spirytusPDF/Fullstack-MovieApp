import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { SessionCreate, SessionPage, SessionUpdate } from '../../DTOs/session.model';
import { SessionService } from './session-service';

describe('SessionService', () => {
  let service: SessionService;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(SessionService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('sends session filters and parses pagination metadata', () => {
    let result: SessionPage | undefined;
    service.getSessions({ movieTitle: 'Example Movie', hallName: 'Room 2' }).subscribe((page) => {
      result = page;
    });

    const request = httpTestingController.expectOne((req) =>
      req.url === '/api/sessions/view'
      && req.params.get('MovieTitle') === 'Example Movie'
      && req.params.get('HallName') === 'Room 2'
      && req.params.get('Page') === '1'
      && req.params.get('PageSize') === '9'
      && req.params.get('Sort') === 'soon'
    );

    expect(request.request.method).toBe('GET');
    request.flush([], {
      headers: { 'X-Pagination': '{"totalCount":0,"page":1,"pageSize":9}' }
    });
    expect(result?.sessions).toEqual([]);
    expect(result?.pagination.totalPages).toBe(1);
  });

  it('creates a session for the movie ID query parameter', () => {
    const session: SessionCreate = {
      startsAt: '2026-12-01T18:00:00.000Z',
      hallName: 'Room 2',
      ticketPrice: 12.5,
      availableSeats: 40
    };
    service.createNewSession(session, 27).subscribe();

    const request = httpTestingController.expectOne((req) =>
      req.url === '/api/sessions/newSession' && req.params.get('movieId') === '27'
    );
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(session);
    request.flush(null);
  });

  it('updates a session by its ID', () => {
    const update: SessionUpdate = {
      startsAt: '2026-12-01T18:00:00.000Z',
      hallName: 'Room 3',
      ticketPrice: 15
    };
    service.updateSession(8, update).subscribe();

    const request = httpTestingController.expectOne('/api/sessions/update/8');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(update);
    request.flush(null);
  });

  it('deletes a session by its ID', () => {
    service.deleteSession(8).subscribe();

    const request = httpTestingController.expectOne('/api/sessions/delete/8');
    expect(request.request.method).toBe('POST');
    request.flush(null);
  });
});
