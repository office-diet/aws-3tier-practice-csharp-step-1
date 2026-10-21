CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS counters (
    id              UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    accessed_at     TIMESTAMP NOT NULL DEFAULT NOW()
);