-- =====================================================
-- Script: Create Partitioned Tables
-- Description: Creates partitioned bill tables by financial year
-- =====================================================

-- =====================================================
-- MAIN BILL DETAILS TABLE (PARTITIONED BY FY)
-- =====================================================

CREATE TABLE IF NOT EXISTS billing.bill_details
(
    bill_id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
    bill_no character(15) COLLATE pg_catalog."default",
    bill_date date NOT NULL,
    bill_mode smallint DEFAULT 0,
    reference_no character(20) COLLATE pg_catalog."default",
    tr_master_id smallint NOT NULL,
    payment_mode smallint NOT NULL,
    financial_year smallint NOT NULL,
    demand character(2) COLLATE pg_catalog."default",
    major_head character(4) COLLATE pg_catalog."default",
    sub_major_head character(2) COLLATE pg_catalog."default",
    minor_head character(3) COLLATE pg_catalog."default",
    plan_status character(2) COLLATE pg_catalog."default",
    scheme_head character(3) COLLATE pg_catalog."default",
    detail_head character(2) COLLATE pg_catalog."default",
    voted_charged character(1) COLLATE pg_catalog."default",
    gross_amount bigint DEFAULT 0,
    net_amount bigint DEFAULT 0,
    bt_amount bigint DEFAULT 0,
    sanction_no character varying(25) COLLATE pg_catalog."default",
    sanction_amt bigint DEFAULT 0,
    sanction_date date,
    sanction_by character varying(100) COLLATE pg_catalog."default",
    remarks character varying(100) COLLATE pg_catalog."default",
    ddo_code character(9) COLLATE pg_catalog."default",
    treasury_code character(3) COLLATE pg_catalog."default",
    status smallint NOT NULL,
    is_deleted boolean NOT NULL DEFAULT false,
    is_gst boolean DEFAULT false,
    gst_amount bigint DEFAULT 0,
    created_by_userid bigint,
    created_at timestamp without time zone DEFAULT now(),
    updated_by_userid bigint,
    updated_at timestamp without time zone,
    CONSTRAINT bill_details_pkey PRIMARY KEY (bill_id, financial_year),
    CONSTRAINT bill_details_financial_year_fkey FOREIGN KEY (financial_year)
        REFERENCES master.financial_year_master (id),
    CONSTRAINT bill_details_tr_master_id_fkey FOREIGN KEY (tr_master_id)
        REFERENCES billing_master.tr_master (id),
    CONSTRAINT bill_details_status_fkey FOREIGN KEY (status)
        REFERENCES billing_master.bill_status_master (status_id),
    CONSTRAINT bill_details_ddo_code_fkey FOREIGN KEY (ddo_code)
        REFERENCES master.ddo (ddo_code),
    CONSTRAINT bill_details_treasury_code_fkey FOREIGN KEY (treasury_code)
        REFERENCES master.treasury (code),
    CONSTRAINT bill_details_demand_fkey FOREIGN KEY (demand)
        REFERENCES master.department (demand_code)
) PARTITION BY LIST (financial_year);

CREATE INDEX idx_bill_details_bill_no ON billing.bill_details (bill_no) WHERE is_deleted = false;
CREATE INDEX idx_bill_details_bill_date ON billing.bill_details (bill_date);
CREATE INDEX idx_bill_details_ddo_code ON billing.bill_details (ddo_code);

COMMENT ON TABLE billing.bill_details IS 'Main bill table partitioned by financial year';

-- Create partitions for FY 2425, 2526, 2627
CREATE TABLE IF NOT EXISTS billing.bill_details_2425 PARTITION OF billing.bill_details FOR VALUES IN (1);
CREATE TABLE IF NOT EXISTS billing.bill_details_2526 PARTITION OF billing.bill_details FOR VALUES IN (2);
CREATE TABLE IF NOT EXISTS billing.bill_details_2627 PARTITION OF billing.bill_details FOR VALUES IN (3);

-- =====================================================
-- BILL BT DETAIL TABLE (PARTITIONED BY FY)
-- =====================================================

CREATE TABLE IF NOT EXISTS billing.bill_btdetail
(
    id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
    bill_id bigint NOT NULL,
    financial_year smallint NOT NULL,
    bt_serial integer,
    bt_type smallint,
    amount bigint,
    ddo_code character(9) COLLATE pg_catalog."default",
    treasury_code character(3) COLLATE pg_catalog."default",
    status smallint,
    created_by bigint,
    created_at timestamp without time zone DEFAULT now(),
    updated_by bigint,
    updated_at timestamp without time zone,
    CONSTRAINT bill_btdetail_pkey PRIMARY KEY (id, financial_year),
    CONSTRAINT bill_btdetail_bill_id_fkey FOREIGN KEY (bill_id, financial_year)
        REFERENCES billing.bill_details (bill_id, financial_year) ON DELETE CASCADE,
    CONSTRAINT bill_btdetail_bt_serial_fkey FOREIGN KEY (bt_serial)
        REFERENCES billing_master.bt_details (bt_serial),
    CONSTRAINT bill_btdetail_financial_year_fkey FOREIGN KEY (financial_year)
        REFERENCES master.financial_year_master (id)
) PARTITION BY LIST (financial_year);

CREATE INDEX idx_bill_btdetail_bill_id ON billing.bill_btdetail (bill_id);

COMMENT ON TABLE billing.bill_btdetail IS 'Bill BT details partitioned by financial year';

CREATE TABLE IF NOT EXISTS billing.bill_btdetail_2425 PARTITION OF billing.bill_btdetail FOR VALUES IN (1);
CREATE TABLE IF NOT EXISTS billing.bill_btdetail_2526 PARTITION OF billing.bill_btdetail FOR VALUES IN (2);
CREATE TABLE IF NOT EXISTS billing.bill_btdetail_2627 PARTITION OF billing.bill_btdetail FOR VALUES IN (3);

-- =====================================================
-- BILL GST TABLE (PARTITIONED BY FY)
-- =====================================================

CREATE TABLE IF NOT EXISTS billing.bill_gst
(
    id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
    bill_id bigint NOT NULL,
    financial_year smallint NOT NULL,
    cpin_id bigint,
    ddo_gstn character varying(255) COLLATE pg_catalog."default",
    ddo_code character(9) COLLATE pg_catalog."default",
    tr_id smallint,
    is_deleted boolean DEFAULT false,
    created_by_userid bigint,
    created_at timestamp without time zone DEFAULT now(),
    updated_by_userid bigint,
    updated_at timestamp without time zone,
    CONSTRAINT bill_gst_pkey PRIMARY KEY (id, financial_year),
    CONSTRAINT bill_gst_bill_id_fkey FOREIGN KEY (bill_id, financial_year)
        REFERENCES billing.bill_details (bill_id, financial_year) ON DELETE CASCADE,
    CONSTRAINT bill_gst_cpin_id_fkey FOREIGN KEY (cpin_id)
        REFERENCES billing_master.cpin_master (id),
    CONSTRAINT bill_gst_financial_year_fkey FOREIGN KEY (financial_year)
        REFERENCES master.financial_year_master (id)
) PARTITION BY LIST (financial_year);

CREATE INDEX idx_bill_gst_bill_id ON billing.bill_gst (bill_id);

COMMENT ON TABLE billing.bill_gst IS 'Bill GST/CPIN mapping partitioned by financial year (optional)';

CREATE TABLE IF NOT EXISTS billing.bill_gst_2425 PARTITION OF billing.bill_gst FOR VALUES IN (1);
CREATE TABLE IF NOT EXISTS billing.bill_gst_2526 PARTITION OF billing.bill_gst FOR VALUES IN (2);
CREATE TABLE IF NOT EXISTS billing.bill_gst_2627 PARTITION OF billing.bill_gst FOR VALUES IN (3);

-- =====================================================
-- BILL ECS/NEFT DETAILS TABLE (PARTITIONED BY FY)
-- =====================================================

CREATE TABLE IF NOT EXISTS billing.bill_ecs_neft_details
(
    id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
    bill_id bigint NOT NULL,
    financial_year smallint NOT NULL,
    payee_name character varying(100) COLLATE pg_catalog."default",
    beneficiary_id character varying(100) COLLATE pg_catalog."default",
    pan_no character(10) COLLATE pg_catalog."default",
    contact_number character(15) COLLATE pg_catalog."default",
    address character varying(200) COLLATE pg_catalog."default",
    email character varying(60) COLLATE pg_catalog."default",
    ifsc_code character(11) COLLATE pg_catalog."default",
    bank_account_number character(20) COLLATE pg_catalog."default",
    bank_name character varying(50) COLLATE pg_catalog."default",
    amount bigint,
    status smallint DEFAULT 1,
    is_active smallint DEFAULT 1,
    is_gst boolean DEFAULT false,
    created_by_userid bigint,
    created_at timestamp without time zone DEFAULT now(),
    updated_by_userid bigint,
    updated_at timestamp without time zone,
    CONSTRAINT bill_ecs_neft_details_pkey PRIMARY KEY (id, financial_year),
    CONSTRAINT bill_ecs_neft_details_bill_id_fkey FOREIGN KEY (bill_id, financial_year)
        REFERENCES billing.bill_details (bill_id, financial_year) ON DELETE CASCADE,
    CONSTRAINT bill_ecs_neft_details_ifsc_code_fkey FOREIGN KEY (ifsc_code)
        REFERENCES master.rbi_ifsc_stock (ifsc),
    CONSTRAINT bill_ecs_neft_details_financial_year_fkey FOREIGN KEY (financial_year)
        REFERENCES master.financial_year_master (id)
) PARTITION BY LIST (financial_year);

CREATE INDEX idx_bill_ecs_bill_id ON billing.bill_ecs_neft_details (bill_id);

COMMENT ON TABLE billing.bill_ecs_neft_details IS 'Bill ECS/NEFT beneficiary details partitioned by financial year';

CREATE TABLE IF NOT EXISTS billing.bill_ecs_neft_details_2425 PARTITION OF billing.bill_ecs_neft_details FOR VALUES IN (1);
CREATE TABLE IF NOT EXISTS billing.bill_ecs_neft_details_2526 PARTITION OF billing.bill_ecs_neft_details FOR VALUES IN (2);
CREATE TABLE IF NOT EXISTS billing.bill_ecs_neft_details_2627 PARTITION OF billing.bill_ecs_neft_details FOR VALUES IN (3);

-- =====================================================
-- ALLOTMENT BOOKED BILL TABLE (PARTITIONED BY FY)
-- =====================================================

-- First create the bantan.ddo_allotment_transactions table (simplified for demo)
CREATE TABLE IF NOT EXISTS bantan.ddo_allotment_transactions
(
    allotment_id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
    ddo_code character(9) COLLATE pg_catalog."default",
    amount bigint,
    financial_year smallint,
    created_at timestamp without time zone DEFAULT now(),
    CONSTRAINT ddo_allotment_transactions_pkey PRIMARY KEY (allotment_id)
);

COMMENT ON TABLE bantan.ddo_allotment_transactions IS 'DDO allotment transactions (simplified)';

CREATE TABLE IF NOT EXISTS billing.ddo_allotment_booked_bill
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY,
    bill_id bigint NOT NULL,
    financial_year smallint NOT NULL,
    allotment_id bigint,
    amount bigint NOT NULL,
    ddo_code character(9) COLLATE pg_catalog."default",
    treasury_code character(3) COLLATE pg_catalog."default",
    active_hoa_id bigint NOT NULL,
    allotment_received bigint DEFAULT 0,
    progressive_expenses bigint DEFAULT 0,
    created_by_userid bigint,
    created_at timestamp without time zone DEFAULT now(),
    updated_by_userid bigint,
    updated_at timestamp without time zone,
    CONSTRAINT ddo_allotment_booked_bill_pkey PRIMARY KEY (id, financial_year),
    CONSTRAINT ddo_allotment_booked_bill_bill_id_fkey FOREIGN KEY (bill_id, financial_year)
        REFERENCES billing.bill_details (bill_id, financial_year) ON DELETE CASCADE,
    CONSTRAINT ddo_allotment_booked_bill_allotment_id_fkey FOREIGN KEY (allotment_id)
        REFERENCES bantan.ddo_allotment_transactions (allotment_id),
    CONSTRAINT ddo_allotment_booked_bill_active_hoa_id_fkey FOREIGN KEY (active_hoa_id)
        REFERENCES master.active_hoa_mst (id),
    CONSTRAINT ddo_allotment_booked_bill_ddo_code_fkey FOREIGN KEY (ddo_code)
        REFERENCES master.ddo (ddo_code),
    CONSTRAINT ddo_allotment_booked_bill_treasury_code_fkey FOREIGN KEY (treasury_code)
        REFERENCES master.treasury (code),
    CONSTRAINT ddo_allotment_booked_bill_financial_year_fkey FOREIGN KEY (financial_year)
        REFERENCES master.financial_year_master (id)
) PARTITION BY LIST (financial_year);

CREATE INDEX idx_allotment_bill_id ON billing.ddo_allotment_booked_bill (bill_id);

COMMENT ON TABLE billing.ddo_allotment_booked_bill IS 'Allotment booking against bills partitioned by financial year';

CREATE TABLE IF NOT EXISTS billing.ddo_allotment_booked_bill_2425 PARTITION OF billing.ddo_allotment_booked_bill FOR VALUES IN (1);
CREATE TABLE IF NOT EXISTS billing.ddo_allotment_booked_bill_2526 PARTITION OF billing.ddo_allotment_booked_bill FOR VALUES IN (2);
CREATE TABLE IF NOT EXISTS billing.ddo_allotment_booked_bill_2627 PARTITION OF billing.ddo_allotment_booked_bill FOR VALUES IN (3);

-- =====================================================
-- BILL SUBVOUCHER TABLE (PARTITIONED BY FY)
-- =====================================================

CREATE TABLE IF NOT EXISTS billing.bill_subvoucher
(
    id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
    bill_id bigint NOT NULL,
    financial_year smallint NOT NULL,
    subvoucher_no character varying(50) COLLATE pg_catalog."default",
    subvoucher_date date,
    subvoucher_amount bigint,
    description character varying(255) COLLATE pg_catalog."default",
    created_by bigint,
    created_at timestamp without time zone DEFAULT now(),
    updated_by bigint,
    updated_at timestamp without time zone,
    CONSTRAINT bill_subvoucher_pkey PRIMARY KEY (id, financial_year),
    CONSTRAINT bill_subvoucher_bill_id_fkey FOREIGN KEY (bill_id, financial_year)
        REFERENCES billing.bill_details (bill_id, financial_year) ON DELETE CASCADE,
    CONSTRAINT bill_subvoucher_financial_year_fkey FOREIGN KEY (financial_year)
        REFERENCES master.financial_year_master (id)
) PARTITION BY LIST (financial_year);

CREATE INDEX idx_bill_subvoucher_bill_id ON billing.bill_subvoucher (bill_id);

COMMENT ON TABLE billing.bill_subvoucher IS 'Bill subvoucher details partitioned by financial year';

CREATE TABLE IF NOT EXISTS billing.bill_subvoucher_2425 PARTITION OF billing.bill_subvoucher FOR VALUES IN (1);
CREATE TABLE IF NOT EXISTS billing.bill_subvoucher_2526 PARTITION OF billing.bill_subvoucher FOR VALUES IN (2);
CREATE TABLE IF NOT EXISTS billing.bill_subvoucher_2627 PARTITION OF billing.bill_subvoucher FOR VALUES IN (3);
