BEGIN;

DROP TABLE IF EXISTS users CASCADE;

CREATE TABLE users (
  id            BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  email         TEXT NOT NULL UNIQUE,
  password_hash BYTEA NOT NULL,
  password_salt BYTEA NOT NULL,
  first_name    TEXT NULL,
  last_name     TEXT NULL,
  created_at    TIMESTAMPTZ NOT NULL DEFAULT now(),
  updated_at    TIMESTAMPTZ NOT NULL DEFAULT now()
);

DO $$
BEGIN
  IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'exporter') THEN
    CREATE ROLE exporter LOGIN PASSWORD 'Qzwxec123';
  ELSE
    ALTER ROLE exporter WITH PASSWORD 'Qzwxec123';
  END IF;
END
$$;

DO $do$
DECLARE
  v_app_user text := '{{ .Values.pg.app.username }}';
  v_app_pass text := '{{ default "Qzwxec123" .Values.pg.app.password }}';
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = v_app_user) THEN
    EXECUTE format('CREATE ROLE %I LOGIN PASSWORD %L', v_app_user, v_app_pass);
  ELSE
    EXECUTE format('ALTER ROLE %I WITH LOGIN PASSWORD %L', v_app_user, v_app_pass);
  END IF;
END
$do$;

GRANT CONNECT ON DATABASE "Users" TO exporter;
GRANT USAGE   ON SCHEMA public TO exporter;
GRANT SELECT  ON ALL TABLES    IN SCHEMA public TO exporter;
GRANT SELECT  ON ALL SEQUENCES IN SCHEMA public TO exporter;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES    TO exporter;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON SEQUENCES TO exporter;

DO $do$
DECLARE
  v_app_user text := '{{ .Values.pg.app.username }}';
BEGIN
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO %I', 'Users', v_app_user);
END
$do$;

DO $do$
DECLARE
  v_app_user text := '{{ .Values.pg.app.username }}';
BEGIN
  EXECUTE format('GRANT USAGE ON SCHEMA public TO %I', v_app_user);
  EXECUTE format('GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO %I', v_app_user);
  EXECUTE format('GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA public TO %I', v_app_user);
END
$do$;

DO $do$
DECLARE
  v_app_user text := '{{ .Values.pg.app.username }}';
BEGIN
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO %I', v_app_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO %I', v_app_user);
END
$do$;

COMMIT;
