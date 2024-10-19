CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Schema inventory
INSERT INTO inventory.categories (id, name, description, created_at, is_active)
VALUES
    (uuid_generate_v4(), 'Electronics', 'Electronic devices and gadgets', NOW(), TRUE),
    (uuid_generate_v4(), 'Furniture', 'Various kinds of furniture', NOW(), TRUE);

INSERT INTO inventory.units (id, name, description, created_at, is_active)
VALUES
    (uuid_generate_v4(), 'Piece', 'Individual piece count', NOW(), TRUE),
    (uuid_generate_v4(), 'Box', 'Box containing multiple pieces', NOW(), TRUE);

INSERT INTO inventory.products (id, sku, name, description, product_type, category_id, unit_id, cost_price, profit_margin, selling_price, created_at, is_active)
VALUES
    (uuid_generate_v4(), 'SKU001', 'Laptop', '15 inch screen laptop', 1, (SELECT id FROM inventory.categories WHERE name = 'Electronics'), (SELECT id FROM inventory.units WHERE name = 'Piece'), 500.00, 0.20, 600.00, NOW(), TRUE),
    (uuid_generate_v4(), 'SKU002', 'Desk', 'Wooden office desk', 1, (SELECT id FROM inventory.categories WHERE name = 'Furniture'), (SELECT id FROM inventory.units WHERE name = 'Piece'), 150.00, 0.30, 195.00, NOW(), TRUE);

INSERT INTO inventory.storages (id, name, description, created_at, is_active)
VALUES
    (uuid_generate_v4(), 'Warehouse 1', 'Main storage warehouse', NOW(), TRUE),
    (uuid_generate_v4(), 'Store 1', 'Retail store', NOW(), TRUE);

INSERT INTO inventory.storage_addresses (id, storage_id, street, city, state_province, country, postal_code, address_number, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM inventory.storages WHERE name = 'Warehouse 1'), '1234 Warehouse St', 'CityA', 'StateA', 'CountryA', '12345', '1', NOW()),
    (uuid_generate_v4(), (SELECT id FROM inventory.storages WHERE name = 'Store 1'), '5678 Store Ave', 'CityB', 'StateB', 'CountryB', '67890', '10', NOW());

INSERT INTO inventory.stocks (id, product_id, storage_id, quantity, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM inventory.products WHERE name = 'Laptop'), (SELECT id FROM inventory.storages WHERE name = 'Warehouse 1'), 100, NOW()),
    (uuid_generate_v4(), (SELECT id FROM inventory.products WHERE name = 'Desk'), (SELECT id FROM inventory.storages WHERE name = 'Store 1'), 50, NOW());

INSERT INTO inventory.stock_movements (id, stock_id, quantity, movement_type, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM inventory.stocks WHERE product_id = (SELECT id FROM inventory.products WHERE name = 'Laptop')), 10, 1, NOW()),
    (uuid_generate_v4(), (SELECT id FROM inventory.stocks WHERE product_id = (SELECT id FROM inventory.products WHERE name = 'Desk')), 5, 2, NOW());

-- Schema sales
INSERT INTO sales.customers (id, customer_type, name, document, email, phone, created_at, is_active)
VALUES
    (uuid_generate_v4(), 1, 'Acme Corp', '123456789', 'contact@acmecorp.com', '+1234567890', NOW(), TRUE),
    (uuid_generate_v4(), 1, 'Global Inc.', '987654321', 'info@globalinc.com', '+0987654321', NOW(), TRUE);

INSERT INTO sales.customer_addresses (id, customer_id, street, city, state_province, country, postal_code, address_number, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM sales.customers WHERE name = 'Acme Corp'), '101 Business Rd', 'Metropolis', 'District', 'Country', '10101', '101', NOW()),
    (uuid_generate_v4(), (SELECT id FROM sales.customers WHERE name = 'Global Inc.'), '202 Corporate Ave', 'Capital', 'Province', 'Country', '20202', '202', NOW());

INSERT INTO sales.categories (id, name, description, created_at, is_active)
VALUES
    (uuid_generate_v4(), 'Services', 'Various services provided', NOW(), TRUE);

INSERT INTO sales.units (id, name, description, created_at, is_active)
VALUES
    (uuid_generate_v4(), 'Hour', 'Service provided per hour', NOW(), TRUE);

INSERT INTO sales.products (id, sku, name, description, product_type, category_id, unit_id, selling_price, created_at, is_active)
VALUES
    (uuid_generate_v4(), 'SVC001', 'Consulting Service', 'Consulting provided per hour', 2, (SELECT id FROM sales.categories WHERE name = 'Services'), (SELECT id FROM sales.units WHERE name = 'Hour'), 150.00, NOW(), TRUE);

INSERT INTO sales.orders (id, customer_id, order_number, order_date, order_status, subtotal, discout, tax, other_costs, total, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM sales.customers WHERE name = 'Acme Corp'), 'ORD001', NOW(), 1, 1000.00, 50.00, 150.00, 0.00, 1100.00, NOW());

INSERT INTO sales.order_items (id, order_id, product_id, quantity, price, total, discount, tax, other_costs, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM sales.orders WHERE order_number = 'ORD001'), (SELECT id FROM sales.products WHERE name = 'Consulting Service'), 10, 150.00, 1500.00, 50.00, 150.00, 0.00, NOW());

INSERT INTO sales.sales (id, customer_id, sale_number, sale_date, sale_status, subtotal, discout, tax, other_costs, total, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM sales.customers WHERE name = 'Global Inc.'), 'SAL001', NOW(), 1, 2000.00, 100.00, 200.00, 50.00, 2150.00, NOW());

INSERT INTO sales.sale_items (id, sale_id, product_id, quantity, price, total, discount, tax, other_costs, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM sales.sales WHERE sale_number = 'SAL001'), (SELECT id FROM sales.products WHERE name = 'Consulting Service'), 15, 150.00, 2250.00, 100.00, 200.00, 50.00, NOW());

-- Schema purchases
INSERT INTO purchases.suppliers (id, supplier_type, name, document, email, phone, created_at, is_active)
VALUES
    (uuid_generate_v4(), 1, 'Hardware Supplies Inc.', '123789456', 'sales@hwsupplies.com', '+1237894560', NOW(), TRUE),
    (uuid_generate_v4(), 1, 'Office Furniture Co.', '987123654', 'support@officefurniture.com', '+9871236540', NOW(), TRUE);

INSERT INTO purchases.supplier_addresses (id, supplier_id, street, city, state_province, country, postal_code, address_number, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM purchases.suppliers WHERE name = 'Hardware Supplies Inc.'), '999 Industry Blvd', 'Tech City', 'Sector', 'Country', '99999', '999', NOW()),
    (uuid_generate_v4(), (SELECT id FROM purchases.suppliers WHERE name = 'Office Furniture Co.'), '888 Commerce St', 'Market Town', 'Region', 'Country', '88888', '888', NOW());

INSERT INTO purchases.categories (id, name, description, created_at, is_active)
VALUES
    (uuid_generate_v4(), 'Office Supplies', 'Supplies for office use', NOW(), TRUE);

INSERT INTO purchases.units (id, name, description, created_at, is_active)
VALUES
    (uuid_generate_v4(), 'Pack', 'Pack containing multiple units', NOW(), TRUE);

INSERT INTO purchases.products (id, sku, name, description, product_type, category_id, unit_id, selling_price, created_at, is_active)
VALUES
    (uuid_generate_v4(), 'PROD001', 'Office Chair', 'Ergonomic office chair', 1, (SELECT id FROM purchases.categories WHERE name = 'Office Supplies'), (SELECT id FROM purchases.units WHERE name = 'Pack'), 120.00, NOW(), TRUE);

INSERT INTO purchases.orders (id, supplier_id, order_number, order_date, order_status, subtotal, discout, tax, other_costs, total, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM purchases.suppliers WHERE name = 'Hardware Supplies Inc.'), 'PO001', NOW(), 1, 500.00, 0.00, 50.00, 10.00, 560.00, NOW());

INSERT INTO purchases.order_items (id, order_id, product_id, quantity, price, total, discount, tax, other_costs, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM purchases.orders WHERE order_number = 'PO001'), (SELECT id FROM purchases.products WHERE name = 'Office Chair'), 20, 25.00, 500.00, 0.00, 50.00, 10.00, NOW());

INSERT INTO purchases.purchases (id, supplier_id, purchase_number, purchase_date, purchase_status, subtotal, discout, tax, other_costs, total, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM purchases.suppliers WHERE name = 'Office Furniture Co.'), 'PUR001', NOW(), 1, 1200.00, 100.00, 50.00, 30.00, 1180.00, NOW());

INSERT INTO purchases.purchase_items (id, purchase_id, product_id, quantity, price, total, discount, tax, other_costs, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM purchases.purchases WHERE purchase_number = 'PUR001'), (SELECT id FROM purchases.products WHERE name = 'Office Chair'), 10, 120.00, 1200.00, 100.00, 50.00, 30.00, NOW());

INSERT INTO purchases.payments (id, purchase_id, due_date, payment_date, payment_method, payment_status, amount, created_at)
VALUES
    (uuid_generate_v4(), (SELECT id FROM purchases.purchases WHERE purchase_number = 'PUR001'), NOW() + INTERVAL '30 DAYS', NULL, 1, 1, 1180.00, NOW());