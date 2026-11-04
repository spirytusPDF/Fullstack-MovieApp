import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddMovieForm } from './add-movie-form';

describe('AddMovieForm', () => {
  let component: AddMovieForm;
  let fixture: ComponentFixture<AddMovieForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddMovieForm],
    }).compileComponents();

    fixture = TestBed.createComponent(AddMovieForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
