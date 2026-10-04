import { ChangeDetectionStrategy, Component, DestroyRef, ElementRef, afterNextRender, effect, inject, input, output, viewChild } from '@angular/core';
import * as L from 'leaflet';

export interface MapPoint { latitude: number; longitude: number; }

// SSA 6.1: pick a station's location by clicking the map. The coordinates come in as inputs,
// so typed values and a loaded station move the pin; a click sends the new point out.
@Component({
  selector: 'app-location-picker',
  template: `
    <div #mapHost class="map"></div>
    <p class="muted hint">Click the map to place the station, or type the coordinates.</p>
  `,
  styles: `.map { height: 380px; border-radius: 12px; overflow: hidden; } .hint { margin: 4px 0 0; }`,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LocationPicker {
  readonly latitude = input.required<number>();
  readonly longitude = input.required<number>();
  readonly picked = output<MapPoint>();

  private readonly host = viewChild.required<ElementRef<HTMLDivElement>>('mapHost');
  private map: L.Map | null = null;
  private pin: L.CircleMarker | null = null;

  constructor() {
    // The map needs a real DOM element, so it is created after the first render.
    afterNextRender(() => {
      const start: L.LatLngTuple = [this.latitude(), this.longitude()];
      this.map = L.map(this.host().nativeElement).setView(start, 13);
      L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; OpenStreetMap contributors'
      }).addTo(this.map);
      this.pin = L.circleMarker(start, { radius: 10, weight: 3, color: '#ffffff', fillColor: '#2e7d32', fillOpacity: 0.95 })
        .addTo(this.map);
      this.map.on('click', (event: L.LeafletMouseEvent) =>
        this.picked.emit({ latitude: round6(event.latlng.lat), longitude: round6(event.latlng.lng) }));
    });

    // Typed coordinates, or a station loaded in edit mode: move the pin and keep it in view.
    effect(() => {
      const point: L.LatLngTuple = [this.latitude(), this.longitude()];
      if (!this.map || !this.pin || !isValid(point)) return;
      this.pin.setLatLng(point);
      if (!this.map.getBounds().contains(point)) this.map.panTo(point);
    });

    inject(DestroyRef).onDestroy(() => this.map?.remove());
  }
}

// Six decimals is about 10 cm: precise enough, and tidy in the form.
const round6 = (value: number) => Math.round(value * 1e6) / 1e6;
const isValid = ([lat, lng]: L.LatLngTuple) =>
  Number.isFinite(lat) && Number.isFinite(lng) && Math.abs(lat) <= 90 && Math.abs(lng) <= 180;
