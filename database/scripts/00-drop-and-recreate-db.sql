-- Drop and recreate database
\connect postgres

DROP DATABASE IF EXISTS billing_system;

CREATE DATABASE billing_system
    ENCODING = 'UTF8'
    OWNER = postgres;

\connect billing_system

COMMENT ON DATABASE billing_system IS 'Billing system with declarative partitioning';
