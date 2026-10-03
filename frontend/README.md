# VoltHub web (Angular 22)

The browser client for the VoltHub API: drivers browse stations on a map, book chargers, charge and pay; operators change charger status and prices.

## Run it

Requirements: **Node.js 24 LTS** (Angular CLI 22.2 needs Node ≥ 22.22.3 or ≥ 24.15) and the VoltHub API running.

1. In `proxy.conf.json`, set `target` to the API's HTTPS address (see `src/VoltHub.Api/Properties/launchSettings.json`, the `https` profile).
2. `npm install`
3. `npm start` → open http://localhost:4200

The dev server forwards every `/api/...` call to the API (proxy), so the browser sees one origin and no CORS setup is needed during development.

`npm test` runs the unit tests (Vitest). `npm run build` creates the production bundle in `dist/`.

## What goes where

```
src/app/
  app.ts / app.html        shell: toolbar with role-aware navigation, <router-outlet>
  app.config.ts            providers: router, HttpClient + auth interceptor
  app.routes.ts            pages, lazy-loaded, each protected by a guard
  core/                    shared by every page
    api/models.ts          TypeScript copies of the API's JSON contracts
    api/api-error.ts       RFC 7807 problem response → one readable message
    auth/auth.service.ts   login state as signals; login, register, refresh, logout
    auth/auth.interceptor  adds the Bearer token; on 401 refreshes once and retries
    auth/auth.guards.ts    who may open which page (the API still checks everything)
    services/*.service.ts  one class per API area (stations, vehicles, reservations, maintenance, reports…)
    ui/                    status chip, notifications, date helpers
  features/                one folder per page
    auth/                  login, register (Signal Forms)
    stations/              list + Leaflet map + nearest-station recommendation,
                           station detail (owner: reserve, rate, report a problem; operator: status and price),
                           reservation dialog, problem-report dialog, ratings panel (child component)
    vehicles/              my vehicles + add form
    reservations/          my reservations, cancel, start charging
    charging/              live session (polled every 5 s), stop, history
    invoices/              invoices and payment
    statistics/            owner: consumption statistics and the problems they reported
    account/               profile for every role: name and password (cross-field "repeat password" rule)
    operator/              maintenance (open work, charger history, one form for four actions), problem reports
    notifications/         operator notifications; the toolbar bell polls the unread count every 30 s
    admin/                 users (accounts, new operators) and reports (overview, load, revenue, faults)
```

## Design choices

- **Standalone components, signals, zoneless change detection** — the Angular 22 defaults: state lives in signals, and only what reads a changed signal re-renders.
- **Signal Forms** (`form()`, `[formField]`, stable since Angular 22) for every form, with Angular Material fields.
- **Lazy-loaded pages** — Leaflet is only downloaded with the stations page.
- **Business rules stay in the API.** The UI mirrors a few of them (which buttons to show), but the API decides; its error messages are shown as they come.
- **Token storage (demo trade-off):** tokens are kept in `localStorage` so a page reload keeps you logged in. Hardening for production: the refresh token in an httpOnly cookie set by the API.
