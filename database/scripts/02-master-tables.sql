-- =====================================================
-- Script: Create Master Reference Tables
-- Description: Creates simplified master tables for the demo
-- =====================================================

-- Financial Year Master Table
CREATE TABLE IF NOT EXISTS master.financial_year_master
(
    id smallint NOT NULL GENERATED ALWAYS AS IDENTITY,
    financial_year character(9) COLLATE pg_catalog."default" NOT NULL UNIQUE,
    is_active boolean NOT NULL DEFAULT false,
    created_by_userid bigint,
    created_at timestamp without time zone DEFAULT now(),
    updated_by_userid bigint,
    updated_at timestamp without time zone,
    CONSTRAINT financial_year_master_pkey PRIMARY KEY (id)
);

COMMENT ON TABLE master.financial_year_master IS 'Financial year configuration - only one can be active at a time';
COMMENT ON COLUMN master.financial_year_master.financial_year IS 'Format: YYYY-YYYY (e.g., 2024-2025)';
COMMENT ON COLUMN master.financial_year_master.is_active IS 'Only current FY should be active (true) - bills can only be created for active FY';

-- Treasury Master
CREATE TABLE IF NOT EXISTS master.treasury
(
    code character(3) COLLATE pg_catalog."default" NOT NULL,
    name character varying(100) COLLATE pg_catalog."default" NOT NULL,
    is_active boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT now(),
    CONSTRAINT treasury_pkey PRIMARY KEY (code)
);

COMMENT ON TABLE master.treasury IS 'Treasury office codes';

-- DDO (Drawing & Disbursing Officer) Master
CREATE TABLE IF NOT EXISTS master.ddo
(
    ddo_code character(9) COLLATE pg_catalog."default" NOT NULL,
    ddo_name character varying(200) COLLATE pg_catalog."default" NOT NULL,
    treasury_code character(3) COLLATE pg_catalog."default",
    is_active boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT now(),
    CONSTRAINT ddo_pkey PRIMARY KEY (ddo_code),
    CONSTRAINT ddo_treasury_code_fkey FOREIGN KEY (treasury_code)
        REFERENCES master.treasury (code)
);

COMMENT ON TABLE master.ddo IS 'Drawing and Disbursing Officer codes';

-- Department Master
CREATE TABLE IF NOT EXISTS master.department
(
    demand_code character(2) COLLATE pg_catalog."default" NOT NULL,
    department_name character varying(200) COLLATE pg_catalog."default" NOT NULL,
    is_active boolean DEFAULT true,
    CONSTRAINT department_pkey PRIMARY KEY (demand_code)
);

COMMENT ON TABLE master.department IS 'Department/Demand codes';

-- RBI IFSC Master (simplified)
CREATE TABLE IF NOT EXISTS master.rbi_ifsc_stock
(
    ifsc character(11) COLLATE pg_catalog."default" NOT NULL,
    bank_name character varying(100) COLLATE pg_catalog."default",
    branch_name character varying(100) COLLATE pg_catalog."default",
    CONSTRAINT rbi_ifsc_stock_pkey PRIMARY KEY (ifsc)
);

COMMENT ON TABLE master.rbi_ifsc_stock IS 'Bank IFSC codes for ECS/NEFT';

-- Active HOA (Head of Account) Master
CREATE TABLE IF NOT EXISTS master.active_hoa_mst
(
    id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
    demand character(2) COLLATE pg_catalog."default",
    major_head character(4) COLLATE pg_catalog."default",
    sub_major_head character(2) COLLATE pg_catalog."default",
    minor_head character(3) COLLATE pg_catalog."default",
    plan_status character(2) COLLATE pg_catalog."default",
    scheme_head character(3) COLLATE pg_catalog."default",
    detail_head character(2) COLLATE pg_catalog."default",
    voted_charged character(1) COLLATE pg_catalog."default",
    hoa_description character varying(500) COLLATE pg_catalog."default",
    is_active boolean DEFAULT true,
    CONSTRAINT active_hoa_mst_pkey PRIMARY KEY (id)
);

COMMENT ON TABLE master.active_hoa_mst IS 'Active Head of Account master for budget allocation';
