-- Create all schemas

CREATE SCHEMA IF NOT EXISTS master;
CREATE SCHEMA IF NOT EXISTS billing;
CREATE SCHEMA IF NOT EXISTS billing_master;
CREATE SCHEMA IF NOT EXISTS billing_log;
CREATE SCHEMA IF NOT EXISTS bantan;

COMMENT ON SCHEMA master IS 'Master reference data';
COMMENT ON SCHEMA billing IS 'Billing transactional data (partitioned)';
COMMENT ON SCHEMA billing_master IS 'Billing configuration/master data';
COMMENT ON SCHEMA billing_log IS 'Audit and logging tables';
COMMENT ON SCHEMA bantan IS 'Budget allotment transactions';
