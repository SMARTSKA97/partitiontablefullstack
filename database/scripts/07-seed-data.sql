-- Seed data for master tables

INSERT INTO master.financial_year_master (financial_year, fy_code, is_active) VALUES
    ('2024-2025', '2425', true),
    ('2025-2026', '2526', false),
    ('2026-2027', '2627', false)
ON CONFLICT DO NOTHING;

INSERT INTO master.treasury (treasury_code, treasury_name) VALUES
    ('001', 'Main Treasury'),
    ('CAF', 'CAF Treasury')
ON CONFLICT (treasury_code) DO NOTHING;

INSERT INTO master.ddo (ddo_code, ddo_name) VALUES
    ('DDO000001', 'DDO Office 1'),
    ('CAFPNA001', 'CAF DDO')
ON CONFLICT DO NOTHING;

INSERT INTO billing_master.bill_status_master (status_id, status_name) VALUES
    (1, 'Draft'),
    (2, 'Submitted'),
    (3, 'Approved'),
    (4, 'Rejected'),
    (5, 'Processing'),
    (106, 'Forwarded to Treasury')
ON CONFLICT DO NOTHING;

INSERT INTO billing_master.tr_master (tr_name) VALUES
    ('Salary'),
    ('Pension'),
    ('TA/DA')
ON CONFLICT (tr_name) DO NOTHING;

INSERT INTO billing_master.bt_details (bt_serial, bt_type_name) VALUES
    (1, 'BT Type 1'),
    (2, 'BT Type 2')
ON CONFLICT DO NOTHING;

INSERT INTO master.active_hoa_mst (demand, major_head, hoa_description) VALUES
    ('01', '2071', 'Pension')
ON CONFLICT DO NOTHING;
