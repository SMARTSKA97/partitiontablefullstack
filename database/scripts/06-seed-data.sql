-- =====================================================
-- Script: Seed Data
-- Description: Insert sample master data for testing
-- =====================================================

-- Financial Years (ID 1=2425, ID 2=2526, ID 3=2627)
INSERT INTO master.financial_year_master (id, financial_year, is_active, created_at)
OVERRIDING SYSTEM VALUE
VALUES 
    (1, '2024-2025', true, now()),   -- Active FY
    (2, '2025-2026', false, now()),  -- Future FY (inactive)
    (3, '2026-2027', false, now())   -- Future FY (inactive)
ON CONFLICT (id) DO NOTHING;

-- Reset sequence
SELECT setval('master.financial_year_master_id_seq', 3, true);

-- Treasuries
INSERT INTO master.treasury (code, name, is_active, created_at)
VALUES 
    ('001', 'Treasury Office Kolkata', true, now()),
    ('002', 'Treasury Office Howrah', true, now()),
    ('003', 'Treasury Office Darjeeling', true, now())
ON CONFLICT (code) DO NOTHING;

-- DDO Codes
INSERT INTO master.ddo (ddo_code, ddo_name, treasury_code, is_active, created_at)
VALUES 
    ('DDO000001', 'DDO Office Education Dept', '001', true, now()),
    ('DDO000002', 'DDO Office Health Dept', '001', true, now()),
    ('DDO000003', 'DDO Office PWD', '002', true, now())
ON CONFLICT (ddo_code) DO NOTHING;

-- Departments
INSERT INTO master.department (demand_code, department_name, is_active)
VALUES 
    ('01', 'Education Department', true),
    ('02', 'Health Department', true),
    ('03', 'Public Works Department', true)
ON CONFLICT (demand_code) DO NOTHING;

-- Sample IFSC Codes
INSERT INTO master.rbi_ifsc_stock (ifsc, bank_name, branch_name)
VALUES 
    ('SBIN0000001', 'State Bank of India', 'Kolkata Main Branch'),
    ('HDFC0000001', 'HDFC Bank', 'Park Street Branch'),
    ('ICIC0000001', 'ICICI Bank', 'Salt Lake Branch')
ON CONFLICT (ifsc) DO NOTHING;

-- Active HOA (Head of Account)
INSERT INTO master.active_hoa_mst (demand, major_head, sub_major_head, minor_head, plan_status, scheme_head, detail_head, voted_charged, hoa_description, is_active)
VALUES 
    ('01', '2202', '01', '001', '01', '001', '01', 'V', 'Primary Education - Salaries', true),
    ('02', '2210', '01', '001', '01', '001', '01', 'V', 'Medical Services - Salaries', true),
    ('03', '3054', '01', '001', '01', '001', '01', 'V', 'Roads and Bridges - Construction', true);

-- Bill Status Master
INSERT INTO billing_master.bill_status_master (status_id, status_name, status_description)
VALUES 
    (1, 'Draft', 'Bill created but not submitted'),
    (2, 'Submitted', 'Bill submitted for approval'),
    (3, 'Approved', 'Bill approved by authority'),
    (4, 'Rejected', 'Bill rejected'),
    (5, 'Processing', 'Bill under processing'),
    (106, 'Forwarded to Treasury', 'Bill forwarded to treasury')
ON CONFLICT (status_id) DO NOTHING;

-- TR Master
INSERT INTO billing_master.tr_master (tr_code, tr_name, is_active)
VALUES 
    ('TR01', 'Treasury Receipt Type 1', true),
    ('TR02', 'Treasury Receipt Type 2', true),
    ('TR03', 'Treasury Receipt Type 3', true);

-- BT Details
INSERT INTO billing_master.bt_details (bt_serial, bt_type_name, is_active)
VALUES 
    (1, 'Budget Transfer Type 1', true),
    (2, 'Budget Transfer Type 2', true),
    (3, 'Budget Transfer Type 3', true)
ON CONFLICT (bt_serial) DO NOTHING;

-- Sample CPIN Master
INSERT INTO billing_master.cpin_master (cpin_number, cpin_date, amount, is_used, created_at)
VALUES 
    ('CPIN001234567890', '2024-04-01', 50000, false, now()),
    ('CPIN001234567891', '2024-05-15', 75000, false, now()),
    ('CPIN001234567892', '2024-06-20', 100000, false, now());

-- Sample Allotment Transactions
INSERT INTO bantan.ddo_allotment_transactions (ddo_code, amount, financial_year, created_at)
VALUES 
    ('DDO000001', 10000000, 1, now()),
    ('DDO000002', 15000000, 1, now()),
    ('DDO000003', 20000000, 1, now());

COMMENT ON DATABASE postgres IS 'Billing system database with partitioned tables by financial year';
