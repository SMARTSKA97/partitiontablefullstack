-- Auto-partition creation function
CREATE OR REPLACE FUNCTION billing.auto_create_partition()
RETURNS TRIGGER AS $$
DECLARE
    partition_name TEXT;
    fy_code TEXT;
BEGIN
    -- Get FY code for partition naming
    SELECT financial_year_master.fy_code INTO fy_code
    FROM master.financial_year_master
    WHERE id = NEW.financial_year;
    
    IF fy_code IS NULL THEN
        RAISE EXCEPTION 'Financial year % not found', NEW.financial_year;
    END IF;
    
    partition_name := TG_TABLE_NAME || '_' || fy_code;
    
    -- Check if partition exists
    IF NOT EXISTS (
        SELECT 1 FROM pg_tables 
        WHERE schemaname = TG_TABLE_SCHEMA 
        AND tablename = partition_name
    ) THEN
        -- Create partition
        EXECUTE format(
            'CREATE TABLE %I.%I PARTITION OF %I.%I FOR VALUES IN (%L)',
            TG_TABLE_SCHEMA, partition_name,
            TG_TABLE_SCHEMA, TG_TABLE_NAME,
            NEW.financial_year
        );
        
        RAISE NOTICE 'Auto-created partition: %.%', TG_TABLE_SCHEMA, partition_name;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Active FY enforcement function
CREATE OR REPLACE FUNCTION billing.enforce_active_fy()
RETURNS TRIGGER AS $$
DECLARE
    is_fy_active BOOLEAN;
BEGIN
    SELECT is_active INTO is_fy_active
    FROM master.financial_year_master
    WHERE id = NEW.financial_year;
    
    IF NOT is_fy_active THEN
        RAISE EXCEPTION 'Cannot % records in inactive financial year %', TG_OP, NEW.financial_year
            USING HINT = 'Mark the financial year as active to perform write operations';
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create triggers on all partitioned tables
DO $$ 
DECLARE
    tbl TEXT;
BEGIN
    FOR tbl IN 
        SELECT tablename FROM pg_tables 
        WHERE schemaname = 'billing' 
        AND tablename NOT LIKE '%_%'  -- Parent tables only
    LOOP
        -- Auto-create partition trigger
        EXECUTE format(
            'CREATE TRIGGER auto_create_%s_partition 
             BEFORE INSERT ON billing.%I 
             FOR EACH ROW EXECUTE FUNCTION billing.auto_create_partition()',
            tbl, tbl
        );
        
        -- Active FY enforcement triggers
        EXECUTE format(
            'CREATE TRIGGER enforce_active_fy_insert_%s
             BEFORE INSERT ON billing.%I
             FOR EACH ROW EXECUTE FUNCTION billing.enforce_active_fy()',
            tbl, tbl
        );
        
        EXECUTE format(
            'CREATE TRIGGER enforce_active_fy_update_%s
             BEFORE UPDATE ON billing.%I
             FOR EACH ROW EXECUTE FUNCTION billing.enforce_active_fy()',
            tbl, tbl
        );
    END LOOP;
END $$;
