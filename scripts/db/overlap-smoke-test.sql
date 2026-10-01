-- VoltHub: proves the database itself rejects overlapping reservations.
-- Runs inside one transaction and rolls back, so it leaves no data behind.
-- Expected: steps 1, 2 and 5 succeed; steps 3 and 4 fail with the named exclusion constraint.
BEGIN;

INSERT INTO users (id, first_name, last_name, email, password_hash, role, is_active, created_at)
VALUES ('00000000-0000-7000-8000-000000000001', 'Smoke', 'Test', 'smoke@volthub.test', 'x', 'User', true, now());
INSERT INTO vehicles (id, make, model, battery_capacity_kwh, connector_type, created_at, user_id) VALUES
 ('00000000-0000-7000-8000-0000000000a1', 'Tesla', 'Model 3', 60, 'CCS', now(), '00000000-0000-7000-8000-000000000001'),
 ('00000000-0000-7000-8000-0000000000a2', 'Renault', 'Zoe', 52, 'Type2', now(), '00000000-0000-7000-8000-000000000001'),
 ('00000000-0000-7000-8000-0000000000a3', 'Nissan', 'Leaf', 40, 'CHAdeMO', now(), '00000000-0000-7000-8000-000000000001');
INSERT INTO charging_stations (id, name, address, city, latitude, longitude, description, created_at)
VALUES ('00000000-0000-7000-8000-0000000000b1', 'Test stanica', 'Bulevar 1', 'Novi Sad', 45.25, 19.84, NULL, now());
INSERT INTO chargers (id, code, connector_type, current_type, power_kw, price_per_kwh, status, status_changed_at, created_at, station_id) VALUES
 ('00000000-0000-7000-8000-0000000000c1', 'A1', 'CCS', 'DC', 50, 30, 'Available', now(), now(), '00000000-0000-7000-8000-0000000000b1'),
 ('00000000-0000-7000-8000-0000000000c2', 'A2', 'Type2', 'AC', 22, 25, 'Available', now(), now(), '00000000-0000-7000-8000-0000000000b1');

\echo '1) Charger A1, vehicle 1, 10:00-11:00  -> expect OK'
INSERT INTO reservations (id, start_time, end_time, status, created_at, vehicle_id, charger_id)
VALUES ('00000000-0000-7000-8000-0000000000d1', '2030-01-01 10:00+00', '2030-01-01 11:00+00', 'Active', now(), '00000000-0000-7000-8000-0000000000a1', '00000000-0000-7000-8000-0000000000c1');

\echo '2) Charger A1, vehicle 2, 11:00-12:00 (back-to-back) -> expect OK: ranges are [start, end)'
INSERT INTO reservations (id, start_time, end_time, status, created_at, vehicle_id, charger_id)
VALUES ('00000000-0000-7000-8000-0000000000d2', '2030-01-01 11:00+00', '2030-01-01 12:00+00', 'Active', now(), '00000000-0000-7000-8000-0000000000a2', '00000000-0000-7000-8000-0000000000c1');

\echo '3) Charger A1, vehicle 3, 10:30-10:45 -> expect ERROR ex_reservations_charger_overlap'
SAVEPOINT charger_overlap;
INSERT INTO reservations (id, start_time, end_time, status, created_at, vehicle_id, charger_id)
VALUES ('00000000-0000-7000-8000-0000000000d3', '2030-01-01 10:30+00', '2030-01-01 10:45+00', 'Active', now(), '00000000-0000-7000-8000-0000000000a3', '00000000-0000-7000-8000-0000000000c1');
ROLLBACK TO SAVEPOINT charger_overlap;

\echo '4) Charger A2, vehicle 1, 10:30-11:30 -> expect ERROR ex_reservations_vehicle_overlap'
SAVEPOINT vehicle_overlap;
INSERT INTO reservations (id, start_time, end_time, status, created_at, vehicle_id, charger_id)
VALUES ('00000000-0000-7000-8000-0000000000d4', '2030-01-01 10:30+00', '2030-01-01 11:30+00', 'Active', now(), '00000000-0000-7000-8000-0000000000a1', '00000000-0000-7000-8000-0000000000c2');
ROLLBACK TO SAVEPOINT vehicle_overlap;

\echo '5) Cancel reservation 1, retry step 3 -> expect OK: only Active reservations block a slot'
UPDATE reservations SET status = 'Cancelled' WHERE id = '00000000-0000-7000-8000-0000000000d1';
INSERT INTO reservations (id, start_time, end_time, status, created_at, vehicle_id, charger_id)
VALUES ('00000000-0000-7000-8000-0000000000d3', '2030-01-01 10:30+00', '2030-01-01 10:45+00', 'Active', now(), '00000000-0000-7000-8000-0000000000a3', '00000000-0000-7000-8000-0000000000c1');

ROLLBACK;
