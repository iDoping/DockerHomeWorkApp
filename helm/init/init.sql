DO $$
BEGIN
  IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'exporter') THEN
    CREATE ROLE exporter LOGIN PASSWORD 'Qzwxec123';
	GRANT pg_monitor TO exporter;
  ELSE
    ALTER ROLE exporter WITH LOGIN PASSWORD 'Qzwxec123';
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

DO $do$
DECLARE
  v_order_user     text := '{{ .Values.pg.services.order.username }}';
  v_order_pass     text := '{{ .Values.pg.services.order.password }}';
  v_billing_user   text := '{{ .Values.pg.services.billing.username }}';
  v_billing_pass   text := '{{ .Values.pg.services.billing.password }}';
  v_notif_user     text := '{{ .Values.pg.services.notifications.username }}';
  v_notif_pass     text := '{{ .Values.pg.services.notifications.password }}';
  v_warehouse_user text := '{{ .Values.pg.services.warehouse.username }}';
  v_warehouse_pass text := '{{ .Values.pg.services.warehouse.password }}';
  v_delivery_user  text := '{{ .Values.pg.services.delivery.username }}';
  v_delivery_pass  text := '{{ .Values.pg.services.delivery.password }}';
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = v_order_user) THEN
    EXECUTE format('CREATE ROLE %I LOGIN PASSWORD %L', v_order_user, v_order_pass);
  ELSE
    EXECUTE format('ALTER ROLE %I WITH LOGIN PASSWORD %L', v_order_user, v_order_pass);
  END IF;

  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = v_billing_user) THEN
    EXECUTE format('CREATE ROLE %I LOGIN PASSWORD %L', v_billing_user, v_billing_pass);
  ELSE
    EXECUTE format('ALTER ROLE %I WITH LOGIN PASSWORD %L', v_billing_user, v_billing_pass);
  END IF;

  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = v_notif_user) THEN
    EXECUTE format('CREATE ROLE %I LOGIN PASSWORD %L', v_notif_user, v_notif_pass);
  ELSE
    EXECUTE format('ALTER ROLE %I WITH LOGIN PASSWORD %L', v_notif_user, v_notif_pass);
  END IF;

  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = v_warehouse_user) THEN
    EXECUTE format('CREATE ROLE %I LOGIN PASSWORD %L', v_warehouse_user, v_warehouse_pass);
  ELSE
    EXECUTE format('ALTER ROLE %I WITH LOGIN PASSWORD %L', v_warehouse_user, v_warehouse_pass);
  END IF;

  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = v_delivery_user) THEN
    EXECUTE format('CREATE ROLE %I LOGIN PASSWORD %L', v_delivery_user, v_delivery_pass);
  ELSE
    EXECUTE format('ALTER ROLE %I WITH LOGIN PASSWORD %L', v_delivery_user, v_delivery_pass);
  END IF;
END
$do$;

-- Creade DB IF NOT EXISTS
SELECT format('CREATE DATABASE %I OWNER %I',
              'users',
              '{{ .Values.pg.app.username }}')
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'users')
\gexec

SELECT format('CREATE DATABASE %I OWNER %I',
              '{{ .Values.pg.services.order.database }}',
              '{{ .Values.pg.services.order.username }}')
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = '{{ .Values.pg.services.order.database }}')
\gexec

SELECT format('CREATE DATABASE %I OWNER %I',
              '{{ .Values.pg.services.billing.database }}',
              '{{ .Values.pg.services.billing.username }}')
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = '{{ .Values.pg.services.billing.database }}')
\gexec

SELECT format('CREATE DATABASE %I OWNER %I',
              '{{ .Values.pg.services.notifications.database }}',
              '{{ .Values.pg.services.notifications.username }}')
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = '{{ .Values.pg.services.notifications.database }}')
\gexec

SELECT format('CREATE DATABASE %I OWNER %I',
              '{{ .Values.pg.services.warehouse.database }}',
              '{{ .Values.pg.services.warehouse.username }}')
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = '{{ .Values.pg.services.warehouse.database }}')
\gexec

SELECT format('CREATE DATABASE %I OWNER %I',
              '{{ .Values.pg.services.delivery.database }}',
              '{{ .Values.pg.services.delivery.username }}')
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = '{{ .Values.pg.services.delivery.database }}')
\gexec

--
DO $do$
DECLARE
  v_app_user       text := '{{ .Values.pg.app.username }}';
  v_order_user     text := '{{ .Values.pg.services.order.username }}';
  v_billing_user   text := '{{ .Values.pg.services.billing.username }}';
  v_notif_user     text := '{{ .Values.pg.services.notifications.username }}';
  v_warehouse_user text := '{{ .Values.pg.services.warehouse.username }}';
  v_delivery_user  text := '{{ .Values.pg.services.delivery.username }}';

  v_users_db      text := 'users';
  v_orders_db     text := '{{ .Values.pg.services.order.database }}';
  v_billing_db    text := '{{ .Values.pg.services.billing.database }}';
  v_notif_db      text := '{{ .Values.pg.services.notifications.database }}';
  v_warehouse_db  text := '{{ .Values.pg.services.warehouse.database }}';
  v_delivery_db   text := '{{ .Values.pg.services.delivery.database }}';
BEGIN
  -- users: app user + exporter
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO %I', v_users_db, 	   v_app_user);
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO %I', v_orders_db,     v_order_user);
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO %I', v_billing_db,    v_billing_user);
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO %I', v_notif_db,      v_notif_user);
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO %I', v_warehouse_db,  v_warehouse_user);
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO %I', v_delivery_db,   v_delivery_user);
  
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO exporter', 'postgres');
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO exporter', v_users_db);
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO exporter', v_orders_db);
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO exporter', v_billing_db);
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO exporter', v_notif_db);
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO exporter', v_warehouse_db);
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO exporter', v_delivery_db);

END
$do$;


--------------------------------
-- USERS
--------------------------------
\connect users

CREATE TABLE IF NOT EXISTS users (
  id            BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  email         TEXT NOT NULL UNIQUE,
  password_hash BYTEA NOT NULL,
  password_salt BYTEA NOT NULL,
  first_name    TEXT NULL,
  last_name     TEXT NULL,
  created_at    TIMESTAMPTZ NOT NULL DEFAULT now(),
  updated_at    TIMESTAMPTZ NOT NULL DEFAULT now()
);

GRANT USAGE   ON SCHEMA public TO exporter;
GRANT SELECT  ON ALL TABLES    IN SCHEMA public TO exporter;
GRANT SELECT  ON ALL SEQUENCES IN SCHEMA public TO exporter;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES    TO exporter;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON SEQUENCES TO exporter;

DO $do$
DECLARE
  v_app_user text := '{{ .Values.pg.app.username }}';
BEGIN
  EXECUTE format('GRANT USAGE ON SCHEMA public TO %I', v_app_user);
  EXECUTE format('GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO %I', v_app_user);
  EXECUTE format('GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA public TO %I', v_app_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO %I', v_app_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO %I', v_app_user);
END
$do$;

--------------------------------
-- ORDERS
--------------------------------
\connect {{ .Values.pg.services.order.database }}

CREATE TABLE IF NOT EXISTS orders (
  id               BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  user_id          BIGINT NOT NULL,
  amount           NUMERIC(18,2) NOT NULL,
  status           SMALLINT NOT NULL,
  idempotency_key  TEXT NULL,
  created_at       TIMESTAMPTZ NOT NULL DEFAULT now(),
  CONSTRAINT uq_orders_user_idempotency UNIQUE (user_id, idempotency_key)
);

CREATE TABLE IF NOT EXISTS order_request_keys (
  id          BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  request_key TEXT NOT NULL,
  user_id     BIGINT NOT NULL,
  amount      NUMERIC(18,2) NOT NULL,
  order_id    BIGINT NULL,
  created_at  TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_order_request_keys_request_key
  ON order_request_keys (request_key);

DO $do$
DECLARE
  v_order_user text := '{{ .Values.pg.services.order.username }}';
BEGIN
  EXECUTE format('GRANT USAGE ON SCHEMA public TO %I', v_order_user);
  EXECUTE format('GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO %I', v_order_user);
  EXECUTE format('GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA public TO %I', v_order_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO %I', v_order_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO %I', v_order_user);
END
$do$;


--------------------------------
-- BILLING
--------------------------------
\connect {{ .Values.pg.services.billing.database }}

CREATE TABLE IF NOT EXISTS accounts (
  id         BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  user_id    BIGINT NOT NULL,
  balance    NUMERIC(18,2) NOT NULL DEFAULT 0,
  updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  CONSTRAINT uq_accounts_user UNIQUE(user_id)
);

CREATE TABLE IF NOT EXISTS billing_transactions (
  id          BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  account_id  BIGINT NOT NULL,
  amount      NUMERIC(18,2) NOT NULL,
  type        TEXT NOT NULL,
  created_at  TIMESTAMPTZ NOT NULL DEFAULT now()
);

DO $do$
DECLARE
  v_billing_user text := '{{ .Values.pg.services.billing.username }}';
BEGIN
  EXECUTE format('GRANT USAGE ON SCHEMA public TO %I', v_billing_user);
  EXECUTE format('GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO %I', v_billing_user);
  EXECUTE format('GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA public TO %I', v_billing_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO %I', v_billing_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO %I', v_billing_user);
END
$do$;

--------------------------------
-- NOTIFICATIONS
--------------------------------
\connect {{ .Values.pg.services.notifications.database }}

CREATE TABLE IF NOT EXISTS notifications (
  id          BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  user_id     BIGINT NULL,
  email       TEXT NOT NULL,
  subject     TEXT NOT NULL,
  body        TEXT NOT NULL,
  status      TEXT NOT NULL,
  created_at  TIMESTAMPTZ NOT NULL DEFAULT now()
);

DO $do$
DECLARE
  v_notif_user text := '{{ .Values.pg.services.notifications.username }}';
BEGIN
  EXECUTE format('GRANT USAGE ON SCHEMA public TO %I', v_notif_user);
  EXECUTE format('GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO %I', v_notif_user);
  EXECUTE format('GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA public TO %I', v_notif_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO %I', v_notif_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO %I', v_notif_user);
END
$do$;

--------------------------------
-- WAREHOUSE
--------------------------------
\connect {{ .Values.pg.services.warehouse.database }}

CREATE TABLE IF NOT EXISTS warehouse_items (
  id                BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  sku               TEXT NOT NULL UNIQUE,
  name              TEXT NOT NULL,
  total_quantity    INT NOT NULL,
  reserved_quantity INT NOT NULL DEFAULT 0,
  created_at        TIMESTAMPTZ NOT NULL DEFAULT now(),
  updated_at        TIMESTAMPTZ NOT NULL DEFAULT now()
);

DO $do$
DECLARE
  v_warehouse_user text := '{{ .Values.pg.services.warehouse.username }}';
BEGIN
  EXECUTE format('GRANT USAGE ON SCHEMA public TO %I', v_warehouse_user);
  EXECUTE format('GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO %I', v_warehouse_user);
  EXECUTE format('GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA public TO %I', v_warehouse_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO %I', v_warehouse_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO %I', v_warehouse_user);
END
$do$;

--------------------------------
-- DELIVERY
--------------------------------
\connect {{ .Values.pg.services.delivery.database }}

CREATE TABLE IF NOT EXISTS delivery_slots (
  id             BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  start_at       TIMESTAMPTZ NOT NULL,
  end_at         TIMESTAMPTZ NOT NULL,
  capacity       INT NOT NULL,
  reserved_count INT NOT NULL DEFAULT 0,
  created_at     TIMESTAMPTZ NOT NULL DEFAULT now(),
  updated_at     TIMESTAMPTZ NOT NULL DEFAULT now()
);

DO $do$
DECLARE
  v_delivery_user text := '{{ .Values.pg.services.delivery.username }}';
BEGIN
  EXECUTE format('GRANT USAGE ON SCHEMA public TO %I', v_delivery_user);
  EXECUTE format('GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO %I', v_delivery_user);
  EXECUTE format('GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA public TO %I', v_delivery_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO %I', v_delivery_user);
  EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO %I', v_delivery_user);
END
$do$;
