-- Helper functions

CREATE OR REPLACE FUNCTION get_active_financial_year()
RETURNS smallint AS $$
    SELECT id FROM master.financial_year_master WHERE is_active = true LIMIT 1;
$$ LANGUAGE SQL STABLE;
