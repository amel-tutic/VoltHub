import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { WritableSignal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { StationDetails, StationInput } from '../../core/api/models';
import { StationForm } from './station-form';

// The model is protected (for the template only), so the test reaches it through this shape.
interface StationFormInternals { model: WritableSignal<StationInput>; }

describe('StationForm', () => {
  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      imports: [StationForm],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()]
    });
  });

  it('starts empty when creating a station', () => {
    const fixture = TestBed.createComponent(StationForm);
    fixture.detectChanges();
    TestBed.inject(HttpTestingController).expectNone(() => true);   // nothing to load
    expect((fixture.componentInstance as unknown as StationFormInternals).model().name).toBe('');
  });

  it('loads the station into the form when editing', async () => {
    const fixture = TestBed.createComponent(StationForm);
    fixture.componentRef.setInput('id', 's1');   // what the router does for /stations/s1/edit
    fixture.detectChanges();
    const station: StationDetails = {
      id: 's1', name: 'VoltHub Centar', address: 'Trg slobode 1', city: 'Novi Sad', latitude: 45.2551, longitude: 19.8451,
      description: null, averageRating: null, ratingCount: 0, chargers: []
    };
    TestBed.inject(HttpTestingController).expectOne('/api/stations/s1').flush(station);
    await fixture.whenStable();

    const model = (fixture.componentInstance as unknown as StationFormInternals).model();
    expect(model.name).toBe('VoltHub Centar');
    expect(model.description).toBe('');   // null from the API becomes an empty text field
  });
});
