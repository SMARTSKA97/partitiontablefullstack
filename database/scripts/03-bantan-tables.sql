-- Bantan schema tables

CREATE TABLE IF NOT EXISTS bantan.ddo_allotment_transactions (
    id bigserial PRIMARY KEY,
    financial_year smallint NOT NULL,
    ddo_code varchar(9),
    treasury_code varchar(3),
    allotment_amount bigint NOT NULL,
    active_hoa_id integer,
    transaction_date date DEFAULT CURRENT_DATE,
    created_at timestamp DEFAULT now()
);

COMMENT ON TABLE bantan.ddo_allotment_transactions IS 'DDO budget allotment tracking';
