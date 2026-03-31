# MT-005 Dry Run Plan

Run this on a production-like copy before applying to production.

## 1) Pre-checks

- Confirm backup/snapshot exists and restore test is validated.
- Confirm there are no long-running writes during migration window.
- Record row counts for tenant-enabled tables.

## 2) Migration Dry Run

- Apply migrations to the copied DB.
- Verify default company exists:
  - `SELECT Id, Name, Slug FROM Companies WHERE Id = '00000000-0000-0000-0000-000000000000';`
- Verify no null `CompanyId` remains:
  - Run: `SELECT COUNT(*) FROM <table> WHERE CompanyId IS NULL` for all tenant tables.

## 3) FK and Constraint Validation

- Verify `CompanyId` columns are `NOT NULL`.
- Verify foreign keys to `Companies(Id)` are valid and trusted.
- Smoke-test core read/write paths for Installation, Maintenance, Faults, Emergency, and Auth users.

## 4) Downtime Measurement

- Capture migration start/end timestamps.
- Capture longest blocking statement and lock waits.
- Record total elapsed time and define rollback threshold.

## 5) Go/No-Go

- Go if: no null `CompanyId`, no FK violations, and migration time within approved window.
- No-Go if: any failed constraint checks, lock timeout risk, or unacceptable downtime.
