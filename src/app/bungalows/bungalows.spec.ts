import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Bungalows } from './bungalows';

describe('Bungalows', () => {
  let component: Bungalows;
  let fixture: ComponentFixture<Bungalows>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Bungalows],
    }).compileComponents();

    fixture = TestBed.createComponent(Bungalows);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
