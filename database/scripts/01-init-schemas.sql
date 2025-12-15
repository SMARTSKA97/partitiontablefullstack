-- =====================================================
-- Script: Initialize Database Schemas
-- Description: Creates all required schemas for the billing system
-- =====================================================

-- Create master schema for reference tables
CREATE SCHEMA IF NOT EXISTS master;

-- Create billing schema for partitioned tables
CREATE SCHEMA IF NOT EXISTS billing;

-- Create billing_master schema for billing configuration tables
CREATE SCHEMA IF NOT EXISTS billing_master;

-- Create billing_log schema for audit logging
CREATE SCHEMA IF NOT EXISTS billing_log;

-- Create bantan schema for allotment management
CREATE SCHEMA IF NOT EXISTS bantan;

COMMENT ON SCHEMA master IS 'Master reference tables (financial year, treasury, ddo, etc.)';
COMMENT ON SCHEMA billing IS 'Main billing tables with partitioning by financial year';
COMMENT ON SCHEMA billing_master IS 'Billing configuration and lookup tables';
COMMENT ON SCHEMA billing_log IS 'Audit log tables and functions';
COMMENT ON SCHEMA bantan IS 'Budget allotment management tables';
