-- =====================================================
-- Script: Create Billing Master Tables
-- Description: Configuration tables for billing module
-- =====================================================

-- Bill Status Master
CREATE TABLE IF NOT EXISTS billing_master.bill_status_master
(
    status_id smallint NOT NULL,
    status_name character varying(100) COLLATE pg_catalog."default" NOT NULL,
    status_description character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT bill_status_master_pkey PRIMARY KEY (status_id)
);

COMMENT ON TABLE billing_master.bill_status_master IS 'Bill status lookup table';

-- TR (Treasury Receipt) Master
CREATE TABLE IF NOT EXISTS billing_master.tr_master
(
    id smallint NOT NULL GENERATED ALWAYS AS IDENTITY,
    tr_code character varying(10) COLLATE pg_catalog."default" NOT NULL,
    tr_name character varying(100) COLLATE pg_catalog."default" NOT NULL,
    is_active boolean DEFAULT true,
    CONSTRAINT tr_master_pkey PRIMARY KEY (id)
);

COMMENT ON TABLE billing_master.tr_master IS 'Treasury Receipt type master';

-- BT (Budget Transfer) Details Master
CREATE TABLE IF NOT EXISTS billing_master.bt_details
(
    bt_serial integer NOT NULL,
    bt_type_name character varying(100) COLLATE pg_catalog."default",
    is_active boolean DEFAULT true,
    CONSTRAINT bt_details_pkey PRIMARY KEY (bt_serial)
);

COMMENT ON TABLE billing_master.bt_details IS 'Budget Transfer serial/type master';

-- CPIN (Challan Identification Number) Master for GST
CREATE TABLE IF NOT EXISTS billing_master.cpin_master
(
    id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
    cpin_number character varying(50) COLLATE pg_catalog."default" NOT NULL UNIQUE,
    cpin_date date NOT NULL,
    amount bigint NOT NULL,
    is_used boolean DEFAULT false,
    created_at timestamp without time zone DEFAULT now(),
    CONSTRAINT cpin_master_pkey PRIMARY KEY (id)
);

COMMENT ON TABLE billing_master.cpin_master IS 'CPIN master for GST payment tracking';
