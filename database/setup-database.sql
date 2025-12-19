-- Main setup script - runs all sub-scripts in order

\echo ''
\echo '======================================='
\echo 'Database Setup Starting...'
\echo '======================================='

\echo ''
\echo 'Step 0: Drop and Create DB...'
\i scripts/00-drop-and-recreate-db.sql

\echo ''
\echo 'Step 1: Creating schemas...'
\i scripts/01-init-schemas.sql

\echo ''
\echo 'Step 2: Creating master tables...'
\i scripts/02-master-tables.sql

\echo ''
\echo 'Step 3: Creating billing master tables...'
\i scripts/03-billing-master-tables.sql

\echo ''
\echo 'Step 4: Creating bantan tables...'
\i scripts/03-bantan-tables.sql

\echo ''
\echo 'Step 5: Creating partitioned parent tables...'
\i scripts/04-partitioned-tables.sql

\echo ''
\echo 'Step 6: Creating triggers...'
\i scripts/05-triggers.sql

\echo ''
\echo 'Step 7: Creating functions...'
\i scripts/06-functions-and-procedures.sql

\echo ''
\echo 'Step 8: Inserting seed data...'
\i scripts/07-seed-data.sql

\echo ''
\echo 'Step 9: Creating initial partitions (for EF Core scaffolding)...'
\i scripts/08-initial-partitions.sql

\echo ''
\echo '======================================='
\echo 'Setup Complete!'
\echo '======================================='
