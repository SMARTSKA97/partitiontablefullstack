-- Create initial partitions for active FY (enables EF Core to see the tables)
-- These partitions are empty but allow scaffolding to work

CREATE TABLE IF NOT EXISTS billing.bill_details_2425 
PARTITION OF billing.bill_details FOR VALUES IN (1);

CREATE TABLE IF NOT EXISTS billing.bill_btdetail_2425 
PARTITION OF billing.bill_btdetail FOR VALUES IN (1);

CREATE TABLE IF NOT EXISTS billing.bill_gst_2425 
PARTITION OF billing.bill_gst FOR VALUES IN (1);

CREATE TABLE IF NOT EXISTS billing.bill_ecs_neft_details_2425 
PARTITION OF billing.bill_ecs_neft_details FOR VALUES IN (1);

CREATE TABLE IF NOT EXISTS billing.ddo_allotment_booked_bill_2425 
PARTITION OF billing.ddo_allotment_booked_bill FOR VALUES IN (1);

CREATE TABLE IF NOT EXISTS billing.bill_subvoucher_2425 
PARTITION OF billing.bill_subvoucher FOR VALUES IN (1);
