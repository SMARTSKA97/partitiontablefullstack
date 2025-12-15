-- =====================================================
-- Script: Functions and Procedures
-- Description: PostgreSQL functions for bill operations
-- =====================================================

-- =====================================================
-- FUNCTION: Get Financial Year ID from Date
-- =====================================================
CREATE OR REPLACE FUNCTION billing.get_financial_year_id_from_date(p_date date)
RETURNS smallint
LANGUAGE plpgsql
AS $$
DECLARE
    v_fy_id smallint;
    v_year integer;
    v_month integer;
    v_fy_string character(9);
BEGIN
    v_year := EXTRACT(YEAR FROM p_date);
    v_month := EXTRACT(MONTH FROM p_date);
    
    -- Financial year: April 1 to March 31
    IF v_month >= 4 THEN
        -- April to December: current year to next year
        v_fy_string := (v_year || '-' || (v_year + 1));
    ELSE
        -- January to March: previous year to current year
        v_fy_string := ((v_year - 1) || '-' || v_year);
    END IF;
    
    SELECT id INTO v_fy_id
    FROM master.financial_year_master
    WHERE financial_year = v_fy_string;
    
    RETURN v_fy_id;
END;
$$;

COMMENT ON FUNCTION billing.get_financial_year_id_from_date IS 'Returns financial year ID for a given date';

-- =====================================================
-- FUNCTION: Check if Financial Year is Active (Editable)
-- =====================================================
CREATE OR REPLACE FUNCTION billing.is_fy_active(p_fy_id smallint)
RETURNS boolean
LANGUAGE plpgsql
AS $$
DECLARE
    v_is_active boolean;
BEGIN
    SELECT is_active INTO v_is_active
    FROM master.financial_year_master
    WHERE id = p_fy_id;
    
    RETURN COALESCE(v_is_active, false);
END;
$$;

COMMENT ON FUNCTION billing.is_fy_active IS 'Returns true if financial year is active (editable)';

-- =====================================================
-- FUNCTION: Generate Bill Number
-- =====================================================
CREATE OR REPLACE FUNCTION billing.generate_bill_number(p_fy_id smallint)
RETURNS character(15)
LANGUAGE plpgsql
AS $$
DECLARE
    v_fy_string character(9);
    v_count integer;
    v_bill_no character(15);
BEGIN
    SELECT financial_year INTO v_fy_string
    FROM master.financial_year_master
    WHERE id = p_fy_id;
    
    -- Get count of bills for this FY
    SELECT COUNT(*) + 1 INTO v_count
    FROM billing.bill_details
    WHERE financial_year = p_fy_id;
    
    -- Format: FY2425-00001
    v_bill_no := 'FY' || SUBSTRING(v_fy_string, 3, 2) || SUBSTRING(v_fy_string, 8, 2) || '-' || LPAD(v_count::text, 5, '0');
    
    RETURN v_bill_no;
END;
$$;

COMMENT ON FUNCTION billing.generate_bill_number IS 'Generates bill number in format FY2425-00001';

-- =====================================================
-- PROCEDURE: Insert Bill with Components
-- =====================================================
CREATE OR REPLACE FUNCTION billing.insert_bill_with_components(
    p_bill_data jsonb,
    p_bt_data jsonb DEFAULT '[]'::jsonb,
    p_gst_data jsonb DEFAULT '[]'::jsonb,
    p_ecs_data jsonb DEFAULT '[]'::jsonb,
    p_allotment_data jsonb DEFAULT '[]'::jsonb,
    p_subvoucher_data jsonb DEFAULT '[]'::jsonb,
    p_user_id bigint DEFAULT NULL
)
RETURNS jsonb
LANGUAGE plpgsql
AS $$
DECLARE
    v_bill_id bigint;
    v_fy_id smallint;
    v_bill_no character(15);
    v_is_active boolean;
    v_bt_item jsonb;
    v_gst_item jsonb;
    v_ecs_item jsonb;
    v_allotment_item jsonb;
    v_subvoucher_item jsonb;
    v_result jsonb;
BEGIN
    -- Extract bill date and calculate financial year
    v_fy_id := billing.get_financial_year_id_from_date((p_bill_data->>'bill_date')::date);
    
    IF v_fy_id IS NULL THEN
        RAISE EXCEPTION 'Financial year not found for the given bill date';
    END IF;
    
    -- Check if financial year is active
    v_is_active := billing.is_fy_active(v_fy_id);
    IF NOT v_is_active THEN
        RAISE EXCEPTION 'Cannot create bills for inactive financial year';
    END IF;
    
    -- Generate bill number
    v_bill_no := billing.generate_bill_number(v_fy_id);
    
    -- Insert bill
    INSERT INTO billing.bill_details (
        bill_no, bill_date, bill_mode, reference_no, tr_master_id, payment_mode,
        financial_year, demand, major_head, sub_major_head, minor_head, plan_status,
        scheme_head, detail_head, voted_charged, gross_amount, net_amount, bt_amount,
        sanction_no, sanction_amt, sanction_date, sanction_by, remarks, ddo_code,
        treasury_code, status, is_gst, gst_amount, created_by_userid, created_at
    ) VALUES (
        v_bill_no,
        (p_bill_data->>'bill_date')::date,
        COALESCE((p_bill_data->>'bill_mode')::smallint, 0),
        p_bill_data->>'reference_no',
        (p_bill_data->>'tr_master_id')::smallint,
        (p_bill_data->>'payment_mode')::smallint,
        v_fy_id,
        p_bill_data->>'demand',
        p_bill_data->>'major_head',
        p_bill_data->>'sub_major_head',
        p_bill_data->>'minor_head',
        p_bill_data->>'plan_status',
        p_bill_data->>'scheme_head',
        p_bill_data->>'detail_head',
        p_bill_data->>'voted_charged',
        COALESCE((p_bill_data->>'gross_amount')::bigint, 0),
        COALESCE((p_bill_data->>'net_amount')::bigint, 0),
        COALESCE((p_bill_data->>'bt_amount')::bigint, 0),
        p_bill_data->>'sanction_no',
        COALESCE((p_bill_data->>'sanction_amt')::bigint, 0),
        (p_bill_data->>'sanction_date')::date,
        p_bill_data->>'sanction_by',
        p_bill_data->>'remarks',
        p_bill_data->>'ddo_code',
        p_bill_data->>'treasury_code',
        COALESCE((p_bill_data->>'status')::smallint, 1),
        COALESCE((p_bill_data->>'is_gst')::boolean, false),
        COALESCE((p_bill_data->>'gst_amount')::bigint, 0),
        p_user_id,
        now()
    ) RETURNING bill_id INTO v_bill_id;
    
    -- Insert BT details
    FOR v_bt_item IN SELECT * FROM jsonb_array_elements(p_bt_data)
    LOOP
        INSERT INTO billing.bill_btdetail (
            bill_id, financial_year, bt_serial, bt_type, amount, ddo_code,
            treasury_code, status, created_by, created_at
        ) VALUES (
            v_bill_id,
            v_fy_id,
            (v_bt_item->>'bt_serial')::integer,
            (v_bt_item->>'bt_type')::smallint,
            (v_bt_item->>'amount')::bigint,
            v_bt_item->>'ddo_code',
            v_bt_item->>'treasury_code',
            COALESCE((v_bt_item->>'status')::smallint, 1),
            p_user_id,
            now()
        );
    END LOOP;
    
    -- Insert GST details (optional)
    FOR v_gst_item IN SELECT * FROM jsonb_array_elements(p_gst_data)
    LOOP
        INSERT INTO billing.bill_gst (
            bill_id, financial_year, cpin_id, ddo_gstn, ddo_code, tr_id,
            created_by_userid, created_at
        ) VALUES (
            v_bill_id,
            v_fy_id,
            (v_gst_item->>'cpin_id')::bigint,
            v_gst_item->>'ddo_gstn',
            v_gst_item->>'ddo_code',
            (v_gst_item->>'tr_id')::smallint,
            p_user_id,
            now()
        );
    END LOOP;
    
    -- Insert ECS details
    FOR v_ecs_item IN SELECT * FROM jsonb_array_elements(p_ecs_data)
    LOOP
        INSERT INTO billing.bill_ecs_neft_details (
            bill_id, financial_year, payee_name, beneficiary_id, pan_no,
            contact_number, address, email, ifsc_code, bank_account_number,
            bank_name, amount, is_gst, created_by_userid, created_at
        ) VALUES (
            v_bill_id,
            v_fy_id,
            v_ecs_item->>'payee_name',
            v_ecs_item->>'beneficiary_id',
            v_ecs_item->>'pan_no',
            v_ecs_item->>'contact_number',
            v_ecs_item->>'address',
            v_ecs_item->>'email',
            v_ecs_item->>'ifsc_code',
            v_ecs_item->>'bank_account_number',
            v_ecs_item->>'bank_name',
            (v_ecs_item->>'amount')::bigint,
            COALESCE((v_ecs_item->>'is_gst')::boolean, false),
            p_user_id,
            now()
        );
    END LOOP;
    
    -- Insert Allotment details
    FOR v_allotment_item IN SELECT * FROM jsonb_array_elements(p_allotment_data)
    LOOP
        INSERT INTO billing.ddo_allotment_booked_bill (
            bill_id, financial_year, allotment_id, amount, ddo_code,
            treasury_code, active_hoa_id, allotment_received, progressive_expenses,
            created_by_userid, created_at
        ) VALUES (
            v_bill_id,
            v_fy_id,
            (v_allotment_item->>'allotment_id')::bigint,
            (v_allotment_item->>'amount')::bigint,
            v_allotment_item->>'ddo_code',
            v_allotment_item->>'treasury_code',
            (v_allotment_item->>'active_hoa_id')::bigint,
            COALESCE((v_allotment_item->>'allotment_received')::bigint, 0),
            COALESCE((v_allotment_item->>'progressive_expenses')::bigint, 0),
            p_user_id,
            now()
        );
    END LOOP;
    
    -- Insert Subvoucher details
    FOR v_subvoucher_item IN SELECT * FROM jsonb_array_elements(p_subvoucher_data)
    LOOP
        INSERT INTO billing.bill_subvoucher (
            bill_id, financial_year, subvoucher_no, subvoucher_date,
            subvoucher_amount, description, created_by, created_at
        ) VALUES (
            v_bill_id,
            v_fy_id,
            v_subvoucher_item->>'subvoucher_no',
            (v_subvoucher_item->>'subvoucher_date')::date,
            (v_subvoucher_item->>'subvoucher_amount')::bigint,
            v_subvoucher_item->>'description',
            p_user_id,
            now()
        );
    END LOOP;
    
    -- Return result
    v_result := jsonb_build_object(
        'success', true,
        'bill_id', v_bill_id,
        'bill_no', v_bill_no,
        'financial_year', v_fy_id,
        'message', 'Bill created successfully'
    );
    
    RETURN v_result;
    
EXCEPTION
    WHEN OTHERS THEN
        RETURN jsonb_build_object(
            'success', false,
            'error', SQLERRM
        );
END;
$$;

COMMENT ON FUNCTION billing.insert_bill_with_components IS 'Inserts bill with all sub-components in a single transaction';
