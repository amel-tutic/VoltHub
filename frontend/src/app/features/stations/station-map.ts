import { ChangeDetectionStrategy, Component, DestroyRef, ElementRef, afterNextRender, effect, inject, input, output, viewChild } from '@angular/core';
import * as L from 'leaflet';
import { StationSummary } from '../../core/api/models';

// Leaflet + OpenStreetMap. Circle markers (drawn as vectors) avoid Leaflet's marker-image path issues with bundlers.
@Component({
  selector: 'app-station-map',
  template: `<div #mapHost class="map"></div>`,
  styles: `.map { height: 340px; border-radius: 12px; overflow: hidden; margin-bottom: 8px; }`,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StationMap {
  readonly stations = input.required<StationSummary[]>();
  readonly stationSelected = output<string>();

  private readonly host = viewChild.required<ElementRef<HTMLDivElement>>('mapHost');
  private readonly markers = L.layerGroup();
  private map: L.Map | null = null;

  constructor() {
    // The map needs a real DOM element, so it is created after the first render.
    afterNextRender(() => {
      this.map = L.map(this.host().nativeElement).setView([44.6, 20.3], 7);
      L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; OpenStreetMap contributors'
      }).addTo(this.map);
      this.markers.addTo(this.map);
      this.render(this.stations());
    });

    // Re-draw markers whenever the station list changes (e.g. after filtering).
    effect(() => {
      const stations = this.stations();
      if (this.map) this.render(stations);
    });

    inject(DestroyRef).onDestroy(() => this.map?.remove());
  }

  private render(stations: StationSummary[]): void {
    this.markers.clearLayers();
    for (const station of stations) {
      L.circleMarker([station.latitude, station.longitude], {
        radius: 10,
        weight: 2,
        color: '#ffffff',
        fillOpacity: 0.9,
        fillColor: station.availableChargerCount > 0 ? '#2e7d32' : '#9e9e9e'
      })
        .bindTooltip(`${station.name}: ${station.availableChargerCount}/${station.chargerCount} free`)
        .on('click', () => this.stationSelected.emit(station.id))
        .addTo(this.markers);
    }
    if (this.map && stations.length > 0) {
      const bounds = L.latLngBounds(stations.map(s => [s.latitude, s.longitude] as L.LatLngTuple));
      this.map.fitBounds(bounds, { padding: [30, 30], maxZoom: 13 });
    }
  }
}
