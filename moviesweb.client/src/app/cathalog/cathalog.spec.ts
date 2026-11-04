import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Cathalog } from './cathalog';

describe('Cathalog', () => {
  let component: Cathalog;
  let fixture: ComponentFixture<Cathalog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Cathalog],
    }).compileComponents();

    fixture = TestBed.createComponent(Cathalog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
