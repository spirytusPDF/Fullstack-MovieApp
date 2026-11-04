import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MovieDisplay } from './movie-display';

describe('MovieDisplay', () => {
  let component: MovieDisplay;
  let fixture: ComponentFixture<MovieDisplay>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MovieDisplay],
    }).compileComponents();

    fixture = TestBed.createComponent(MovieDisplay);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
