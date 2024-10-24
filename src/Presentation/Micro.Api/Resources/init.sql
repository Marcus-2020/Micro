-- Criação do esquema 'inventory' e das tabelas associadas
CREATE SCHEMA IF NOT EXISTS inventory;

CREATE TABLE IF NOT EXISTS inventory.categories
(
    id          UUID PRIMARY KEY,
    name        TEXT      NOT NULL,
    description TEXT      NOT NULL DEFAULT '',
    created_at  TIMESTAMP NOT NULL,
    updated_at  TIMESTAMP DEFAULT NULL,
    deleted_at  TIMESTAMP DEFAULT NULL,
    is_active   BOOLEAN   NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS inventory.units
(
    id          UUID PRIMARY KEY,
    name        TEXT      NOT NULL,
    description TEXT      NOT NULL DEFAULT '',
    created_at  TIMESTAMP NOT NULL,
    updated_at  TIMESTAMP DEFAULT NULL,
    deleted_at  TIMESTAMP DEFAULT NULL,
    is_active   BOOLEAN   NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS inventory.products
(
    id            UUID PRIMARY KEY,
    sku           TEXT      NOT NULL,
    name          TEXT      NOT NULL,
    description   TEXT      NOT NULL DEFAULT '',
    product_type  INT       NOT NULL,
    category_id   UUID      NOT NULL,
    unit_id       UUID      NOT NULL,
    cost_price    DECIMAL   NOT NULL DEFAULT 0.0,
    profit_margin DECIMAL   NOT NULL DEFAULT 0.0,
    selling_price DECIMAL   NOT NULL DEFAULT 0.0,
    created_at    TIMESTAMP NOT NULL,
    updated_at    TIMESTAMP DEFAULT NULL,
    deleted_at    TIMESTAMP DEFAULT NULL,
    is_active     BOOLEAN   NOT NULL DEFAULT FALSE,

    FOREIGN KEY (category_id) REFERENCES inventory.categories (id),
    FOREIGN KEY (unit_id) REFERENCES inventory.units (id)
);

CREATE TABLE IF NOT EXISTS inventory.storages
(
    id          UUID PRIMARY KEY,
    name        TEXT      NOT NULL,
    description TEXT      NOT NULL DEFAULT '',
    created_at  TIMESTAMP NOT NULL,
    updated_at  TIMESTAMP DEFAULT NULL,
    deleted_at  TIMESTAMP DEFAULT NULL,
    is_active   BOOLEAN   NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS inventory.storage_addresses
(
    id             UUID PRIMARY KEY,
    storage_id     UUID,
    street         TEXT      NOT NULL,
    city           TEXT      NOT NULL,
    state_province TEXT      NOT NULL,
    country        TEXT      NOT NULL,
    postal_code    TEXT      NOT NULL,
    address_number TEXT      NOT NULL,
    created_at     TIMESTAMP NOT NULL,
    updated_at     TIMESTAMP DEFAULT NULL,
    deleted_at     TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (storage_id) REFERENCES inventory.storages (id)
);

CREATE TABLE IF NOT EXISTS inventory.stocks
(
    id         UUID PRIMARY KEY,
    product_id UUID      NOT NULL,
    storage_id UUID      NOT NULL,
    quantity   INT       NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP DEFAULT NULL,
    deleted_at TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (product_id) REFERENCES inventory.products (id),
    FOREIGN KEY (storage_id) REFERENCES inventory.storages (id)
);

CREATE TABLE IF NOT EXISTS inventory.stock_movements
(
    id            UUID PRIMARY KEY,
    stock_id      UUID      NOT NULL,
    quantity      INT       NOT NULL,
    movement_type INT       NOT NULL,
    created_at    TIMESTAMP NOT NULL,
    updated_at    TIMESTAMP DEFAULT NULL,
    deleted_at    TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (stock_id) REFERENCES inventory.stocks (id)
);

-- Criação do esquema 'sales' e das tabelas associadas
CREATE SCHEMA IF NOT EXISTS sales;

CREATE TABLE IF NOT EXISTS sales.customers
(
    id            UUID PRIMARY KEY,
    customer_type INT       NOT NULL,
    name          TEXT      NOT NULL,
    document      TEXT      NOT NULL,
    email         TEXT      NOT NULL,
    phone         TEXT      NOT NULL,
    created_at    TIMESTAMP NOT NULL,
    updated_at    TIMESTAMP DEFAULT NULL,
    deleted_at    TIMESTAMP DEFAULT NULL,
    is_active     BOOLEAN   NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS sales.customer_addresses
(
    id             UUID PRIMARY KEY,
    customer_id    UUID      NOT NULL,
    street         TEXT      NOT NULL,
    city           TEXT      NOT NULL,
    state_province TEXT      NOT NULL,
    country        TEXT      NOT NULL,
    postal_code    TEXT      NOT NULL,
    address_number TEXT      NOT NULL,
    created_at     TIMESTAMP NOT NULL,
    updated_at     TIMESTAMP DEFAULT NULL,
    deleted_at     TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (customer_id) REFERENCES sales.customers (id),
    UNIQUE (customer_id)
);

CREATE TABLE IF NOT EXISTS sales.categories
(
    id          UUID PRIMARY KEY,
    name        TEXT      NOT NULL,
    description TEXT      NOT NULL DEFAULT '',
    created_at  TIMESTAMP NOT NULL,
    updated_at  TIMESTAMP DEFAULT NULL,
    deleted_at  TIMESTAMP DEFAULT NULL,
    is_active   BOOLEAN   NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS sales.units
(
    id          UUID PRIMARY KEY,
    name        TEXT      NOT NULL,
    description TEXT      NOT NULL DEFAULT '',
    created_at  TIMESTAMP NOT NULL,
    updated_at  TIMESTAMP DEFAULT NULL,
    deleted_at  TIMESTAMP DEFAULT NULL,
    is_active   BOOLEAN   NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS sales.products
(
    id            UUID PRIMARY KEY,
    sku           TEXT      NOT NULL,
    name          TEXT      NOT NULL,
    description   TEXT      NOT NULL DEFAULT '',
    product_type  INT       NOT NULL,
    category_id   UUID      NOT NULL,
    unit_id       UUID      NOT NULL,
    selling_price DECIMAL   NOT NULL DEFAULT 0.0,
    created_at    TIMESTAMP NOT NULL,
    updated_at    TIMESTAMP DEFAULT NULL,
    deleted_at    TIMESTAMP DEFAULT NULL,
    is_active     BOOLEAN   NOT NULL DEFAULT FALSE,

    FOREIGN KEY (category_id) REFERENCES sales.categories (id),
    FOREIGN KEY (unit_id) REFERENCES sales.units (id)
);

CREATE TABLE IF NOT EXISTS sales.orders
(
    id           UUID PRIMARY KEY,
    customer_id  UUID      NOT NULL,
    order_number TEXT      NOT NULL,
    order_date   TIMESTAMP NOT NULL,
    order_status INT       NOT NULL,
    subtotal     DECIMAL   NOT NULL,
    discount      DECIMAL   NOT NULL,
    tax          DECIMAL   NOT NULL,
    other_costs  DECIMAL   NOT NULL,
    total        DECIMAL   NOT NULL,
    created_at   TIMESTAMP NOT NULL,
    updated_at   TIMESTAMP DEFAULT NULL,
    cancelled_at TIMESTAMP DEFAULT NULL,
    deleted_at   TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (customer_id) REFERENCES sales.customers (id),
    UNIQUE (order_number)
);

CREATE TABLE IF NOT EXISTS sales.order_items
(
    id          UUID PRIMARY KEY,
    order_id    UUID      NOT NULL,
    product_id  UUID      NOT NULL,
    quantity    INT       NOT NULL,
    price       DECIMAL   NOT NULL,
    total       DECIMAL   NOT NULL,
    discount    DECIMAL   NOT NULL,
    tax         DECIMAL   NOT NULL,
    other_costs DECIMAL   NOT NULL,
    created_at  TIMESTAMP NOT NULL,
    updated_at  TIMESTAMP DEFAULT NULL,
    deleted_at  TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (order_id) REFERENCES sales.orders (id),
    FOREIGN KEY (product_id) REFERENCES sales.products (id)
);

CREATE TABLE IF NOT EXISTS sales.sales
(
    id           UUID PRIMARY KEY,
    customer_id  UUID      NOT NULL,
    sale_number  TEXT      NOT NULL,
    sale_date    TIMESTAMP NOT NULL,
    sale_status  INT       NOT NULL,
    subtotal     DECIMAL   NOT NULL,
    discount      DECIMAL   NOT NULL,
    tax          DECIMAL   NOT NULL,
    other_costs  DECIMAL   NOT NULL,
    total        DECIMAL   NOT NULL,
    created_at   TIMESTAMP NOT NULL,
    updated_at   TIMESTAMP DEFAULT NULL,
    cancelled_at TIMESTAMP DEFAULT NULL,
    deleted_at   TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (customer_id) REFERENCES sales.customers (id),
    UNIQUE (sale_number)
);

CREATE TABLE IF NOT EXISTS sales.sale_items
(
    id          UUID PRIMARY KEY,
    sale_id     UUID      NOT NULL,
    product_id  UUID      NOT NULL,
    quantity    INT       NOT NULL,
    price       DECIMAL   NOT NULL,
    total       DECIMAL   NOT NULL,
    discount    DECIMAL   NOT NULL,
    tax         DECIMAL   NOT NULL,
    other_costs DECIMAL   NOT NULL,
    created_at  TIMESTAMP NOT NULL,
    updated_at  TIMESTAMP DEFAULT NULL,
    deleted_at  TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (sale_id) REFERENCES sales.sales (id),
    FOREIGN KEY (product_id) REFERENCES sales.products (id)
);

CREATE TABLE IF NOT EXISTS sales.payments
(
    id             UUID PRIMARY KEY,
    sale_id        UUID      NOT NULL,
    due_date       TIMESTAMP NOT NULL,
    payment_date   TIMESTAMP DEFAULT NULL,
    payment_method INT       NOT NULL,
    payment_status INT       NOT NULL,
    amount         DECIMAL   NOT NULL,
    created_at     TIMESTAMP NOT NULL,
    updated_at     TIMESTAMP DEFAULT NULL,
    cancelled_at   TIMESTAMP DEFAULT NULL,
    deleted_at     TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (sale_id) REFERENCES sales.sales (id)
);

-- Criação do esquema 'purchases' e das tabelas associadas
CREATE SCHEMA IF NOT EXISTS purchases;

CREATE TABLE IF NOT EXISTS purchases.suppliers
(
    id            UUID PRIMARY KEY,
    supplier_type INT       NOT NULL,
    name          TEXT      NOT NULL UNIQUE,
    document      TEXT      NOT NULL UNIQUE,
    email         TEXT      NOT NULL UNIQUE,
    phone         TEXT      NOT NULL,
    created_at    TIMESTAMP NOT NULL,
    updated_at    TIMESTAMP DEFAULT NULL,
    deleted_at    TIMESTAMP DEFAULT NULL,
    is_active     BOOLEAN   NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS purchases.supplier_addresses
(
    id             UUID PRIMARY KEY,
    supplier_id    UUID      NOT NULL,
    street         TEXT      NOT NULL,
    city           TEXT      NOT NULL,
    state_province TEXT      NOT NULL,
    country        TEXT      NOT NULL,
    postal_code    TEXT      NOT NULL,
    address_number TEXT      NOT NULL,
    created_at     TIMESTAMP NOT NULL,
    updated_at     TIMESTAMP DEFAULT NULL,
    deleted_at     TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (supplier_id) REFERENCES purchases.suppliers (id),
    UNIQUE (supplier_id)
);

CREATE TABLE IF NOT EXISTS purchases.categories
(
    id          UUID PRIMARY KEY,
    name        TEXT      NOT NULL,
    description TEXT      NOT NULL DEFAULT '',
    created_at  TIMESTAMP NOT NULL,
    updated_at  TIMESTAMP DEFAULT NULL,
    deleted_at  TIMESTAMP DEFAULT NULL,
    is_active   BOOLEAN   NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS purchases.units
(
    id          UUID PRIMARY KEY,
    name        TEXT      NOT NULL,
    description TEXT      NOT NULL DEFAULT '',
    created_at  TIMESTAMP NOT NULL,
    updated_at  TIMESTAMP DEFAULT NULL,
    deleted_at  TIMESTAMP DEFAULT NULL,
    is_active   BOOLEAN   NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS purchases.products
(
    id            UUID PRIMARY KEY,
    sku           TEXT      NOT NULL,
    name          TEXT      NOT NULL,
    description   TEXT      NOT NULL DEFAULT '',
    product_type  INT       NOT NULL,
    category_id   UUID      NOT NULL,
    unit_id       UUID      NOT NULL,
    selling_price DECIMAL   NOT NULL DEFAULT 0.0,
    created_at    TIMESTAMP NOT NULL,
    updated_at    TIMESTAMP DEFAULT NULL,
    deleted_at    TIMESTAMP DEFAULT NULL,
    is_active     BOOLEAN   NOT NULL DEFAULT FALSE,

    FOREIGN KEY (category_id) REFERENCES purchases.categories (id),
    FOREIGN KEY (unit_id) REFERENCES purchases.units (id)
);

CREATE TABLE IF NOT EXISTS purchases.orders
(
    id           UUID PRIMARY KEY,
    supplier_id  UUID      NOT NULL,
    order_number TEXT      NOT NULL,
    order_date   TIMESTAMP NOT NULL,
    order_status INT       NOT NULL,
    subtotal     DECIMAL   NOT NULL,
    discount      DECIMAL   NOT NULL,
    tax          DECIMAL   NOT NULL,
    other_costs  DECIMAL   NOT NULL,
    total        DECIMAL   NOT NULL,
    created_at   TIMESTAMP NOT NULL,
    updated_at   TIMESTAMP DEFAULT NULL,
    cancelled_at TIMESTAMP DEFAULT NULL,
    deleted_at   TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (supplier_id) REFERENCES purchases.suppliers (id),
    UNIQUE (order_number)
);

CREATE TABLE IF NOT EXISTS purchases.order_items
(
    id          UUID PRIMARY KEY,
    order_id    UUID      NOT NULL,
    product_id  UUID      NOT NULL,
    quantity    INT       NOT NULL,
    price       DECIMAL   NOT NULL,
    total       DECIMAL   NOT NULL,
    discount    DECIMAL   NOT NULL,
    tax         DECIMAL   NOT NULL,
    other_costs DECIMAL   NOT NULL,
    created_at  TIMESTAMP NOT NULL,
    updated_at  TIMESTAMP DEFAULT NULL,
    deleted_at  TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (order_id) REFERENCES purchases.orders (id),
    FOREIGN KEY (product_id) REFERENCES purchases.products (id)
);

CREATE TABLE IF NOT EXISTS purchases.purchases
(
    id              UUID PRIMARY KEY,
    supplier_id     UUID      NOT NULL,
    purchase_number TEXT      NOT NULL,
    purchase_date   TIMESTAMP NOT NULL,
    purchase_status INT       NOT NULL,
    subtotal        DECIMAL   NOT NULL,
    discount        DECIMAL   NOT NULL,
    tax             DECIMAL   NOT NULL,
    other_costs     DECIMAL   NOT NULL,
    total           DECIMAL   NOT NULL,
    created_at      TIMESTAMP NOT NULL,
    updated_at      TIMESTAMP DEFAULT NULL,
    cancelled_at    TIMESTAMP DEFAULT NULL,
    deleted_at      TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (supplier_id) REFERENCES purchases.suppliers (id),
    UNIQUE (purchase_number)
);

CREATE TABLE IF NOT EXISTS purchases.purchase_items
(
    id          UUID PRIMARY KEY,
    purchase_id UUID      NOT NULL,
    product_id  UUID      NOT NULL,
    quantity    INT       NOT NULL,
    price       DECIMAL   NOT NULL,
    total       DECIMAL   NOT NULL,
    discount    DECIMAL   NOT NULL,
    tax         DECIMAL   NOT NULL,
    other_costs DECIMAL   NOT NULL,
    created_at  TIMESTAMP NOT NULL,
    updated_at  TIMESTAMP DEFAULT NULL,
    deleted_at  TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (purchase_id) REFERENCES purchases.purchases (id),
    FOREIGN KEY (product_id) REFERENCES purchases.products (id)
);

CREATE TABLE IF NOT EXISTS purchases.payments
(
    id             UUID PRIMARY KEY,
    purchase_id    UUID      NOT NULL,
    due_date       TIMESTAMP NOT NULL,
    payment_date   TIMESTAMP DEFAULT NULL,
    payment_method INT       NOT NULL,
    payment_status INT       NOT NULL,
    amount         DECIMAL   NOT NULL,
    created_at     TIMESTAMP NOT NULL,
    updated_at     TIMESTAMP DEFAULT NULL,
    cancelled_at   TIMESTAMP DEFAULT NULL,
    deleted_at     TIMESTAMP DEFAULT NULL,

    FOREIGN KEY (purchase_id) REFERENCES purchases.purchases (id)
);