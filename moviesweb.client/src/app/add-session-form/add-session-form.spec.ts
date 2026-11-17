import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { of } from 'rxjs';
import { vi } from 'vitest';
import { SessionService } from '../services/session-service';
import { AddSessionForm } from './add-session-form';

describe('AddSessionForm', () => {
  let component: AddSessionForm;
  let fixture: ComponentFixture<AddSessionForm>;
  let sessionService: { createNewSession: ReturnType<typeof vi.fn> };
  let router: { navigate: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    sessionService = { createNewSession: vi.fn().mockReturnValue(of(void 0)) };
    router = { navigate: vi.fn() };
    await TestBed.configureTestingModule({
      imports: [AddSessionForm],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: convertToParamMap({ movieId: '42' }),
              queryParamMap: convertToParamMap({ movieTitle: 'Example Movie' })
            }
          }
        },
        { provide: Router, useValue: router },
        { provide: SessionService, useValue: sessionService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AddSessionForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('creates a session for the movie from the route', () => {
    const future = new Date(Date.now() + 10 * 60_000);
    const localDate = new Date(future.getTime() - future.getTimezoneOffset() * 60_000);
    const startsAt = localDate.toISOString().slice(0, 16);
    component.sessionForm.setValue({
      startsAt,
      hallName: '  Room 1  ',
      ticketPrice: 12.5,
      availableSeats: 30
    });

    component.onSubmit();

    expect(sessionService.createNewSession).toHaveBeenCalledWith({
      startsAt,
      hallName: 'Room 1',
      ticketPrice: 12.5,
      availableSeats: 30
    }, 42);
    expect(router.navigate).toHaveBeenCalledWith(['/', 42]);
  });
});
