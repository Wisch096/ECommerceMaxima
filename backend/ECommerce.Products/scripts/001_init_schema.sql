CREATE TABLE IF NOT EXISTS departments (
                                           code VARCHAR(3) PRIMARY KEY,
    description VARCHAR(100) NOT NULL
    );

CREATE TABLE IF NOT EXISTS products (
                                        id UUID PRIMARY KEY,
                                        code VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(255) NOT NULL,
    department_code VARCHAR(3) NOT NULL REFERENCES departments(code),
    price NUMERIC(18,2) NOT NULL CHECK (price >= 0),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    deleted_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
    );

CREATE INDEX IF NOT EXISTS ix_products_department ON products(department_code);
CREATE INDEX IF NOT EXISTS ix_products_is_active ON products(is_active) WHERE deleted_at IS NULL;