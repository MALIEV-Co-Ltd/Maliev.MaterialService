CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;
CREATE TABLE colors (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    hex_code character varying(7),
    created_at timestamp with time zone NOT NULL,
    created_by text NOT NULL,
    updated_at timestamp with time zone,
    updated_by text,
    version integer NOT NULL,
    active boolean NOT NULL,
    CONSTRAINT pk_colors PRIMARY KEY (id)
);

CREATE TABLE manufacturing_processes (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    created_by text NOT NULL,
    updated_at timestamp with time zone,
    updated_by text,
    version integer NOT NULL,
    active boolean NOT NULL,
    CONSTRAINT pk_manufacturing_processes PRIMARY KEY (id)
);

CREATE TABLE mechanical_properties (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    unit character varying(50) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    created_by text NOT NULL,
    updated_at timestamp with time zone,
    updated_by text,
    version integer NOT NULL,
    active boolean NOT NULL,
    CONSTRAINT pk_mechanical_properties PRIMARY KEY (id)
);

CREATE TABLE post_processing_methods (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    created_by text NOT NULL,
    updated_at timestamp with time zone,
    updated_by text,
    version integer NOT NULL,
    active boolean NOT NULL,
    CONSTRAINT pk_post_processing_methods PRIMARY KEY (id)
);

CREATE TABLE suppliers (
    id uuid NOT NULL,
    name character varying(200) NOT NULL,
    contact_info character varying(500),
    created_at timestamp with time zone NOT NULL,
    created_by text NOT NULL,
    updated_at timestamp with time zone,
    updated_by text,
    version integer NOT NULL,
    active boolean NOT NULL,
    CONSTRAINT pk_suppliers PRIMARY KEY (id)
);

CREATE TABLE materials (
    id uuid NOT NULL,
    name character varying(200) NOT NULL,
    code character varying(100) NOT NULL,
    description character varying(1000),
    price_per_unit numeric(18,2) NOT NULL,
    stock_level integer NOT NULL,
    supplier_id uuid,
    created_at timestamp with time zone NOT NULL,
    created_by text NOT NULL,
    updated_at timestamp with time zone,
    updated_by text,
    version integer NOT NULL,
    active boolean NOT NULL,
    CONSTRAINT pk_materials PRIMARY KEY (id),
    CONSTRAINT fk_materials_suppliers_supplier_id FOREIGN KEY (supplier_id) REFERENCES suppliers (id) ON DELETE SET NULL
);

CREATE TABLE material_colors (
    available_colors_id uuid NOT NULL,
    materials_id uuid NOT NULL,
    CONSTRAINT pk_material_colors PRIMARY KEY (available_colors_id, materials_id),
    CONSTRAINT fk_material_colors_colors_available_colors_id FOREIGN KEY (available_colors_id) REFERENCES colors (id) ON DELETE CASCADE,
    CONSTRAINT fk_material_colors_materials_materials_id FOREIGN KEY (materials_id) REFERENCES materials (id) ON DELETE CASCADE
);

CREATE TABLE material_manufacturing_processes (
    manufacturing_processes_id uuid NOT NULL,
    materials_id uuid NOT NULL,
    CONSTRAINT pk_material_manufacturing_processes PRIMARY KEY (manufacturing_processes_id, materials_id),
    CONSTRAINT fk_material_manufacturing_processes_manufacturing_processes_ma FOREIGN KEY (manufacturing_processes_id) REFERENCES manufacturing_processes (id) ON DELETE CASCADE,
    CONSTRAINT fk_material_manufacturing_processes_materials_materials_id FOREIGN KEY (materials_id) REFERENCES materials (id) ON DELETE CASCADE
);

CREATE TABLE material_mechanical_properties (
    material_id uuid NOT NULL,
    mechanical_property_id uuid NOT NULL,
    value numeric(18,4) NOT NULL,
    CONSTRAINT pk_material_mechanical_properties PRIMARY KEY (material_id, mechanical_property_id),
    CONSTRAINT fk_material_mechanical_properties_materials_material_id FOREIGN KEY (material_id) REFERENCES materials (id) ON DELETE CASCADE,
    CONSTRAINT fk_material_mechanical_properties_mechanical_properties_mechan FOREIGN KEY (mechanical_property_id) REFERENCES mechanical_properties (id) ON DELETE CASCADE
);

CREATE TABLE material_post_processing_methods (
    materials_id uuid NOT NULL,
    post_processing_methods_id uuid NOT NULL,
    CONSTRAINT pk_material_post_processing_methods PRIMARY KEY (materials_id, post_processing_methods_id),
    CONSTRAINT fk_material_post_processing_methods_materials_materials_id FOREIGN KEY (materials_id) REFERENCES materials (id) ON DELETE CASCADE,
    CONSTRAINT fk_material_post_processing_methods_post_processing_methods_po FOREIGN KEY (post_processing_methods_id) REFERENCES post_processing_methods (id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX ix_colors_hex_code ON colors (hex_code);

CREATE UNIQUE INDEX ix_colors_name ON colors (name);

CREATE UNIQUE INDEX ix_manufacturing_processes_name ON manufacturing_processes (name);

CREATE INDEX ix_material_colors_materials_id ON material_colors (materials_id);

CREATE INDEX ix_material_manufacturing_processes_materials_id ON material_manufacturing_processes (materials_id);

CREATE INDEX ix_material_mechanical_properties_mechanical_property_id ON material_mechanical_properties (mechanical_property_id);

CREATE INDEX ix_material_post_processing_methods_post_processing_methods_id ON material_post_processing_methods (post_processing_methods_id);

CREATE UNIQUE INDEX ix_materials_code ON materials (code);

CREATE UNIQUE INDEX ix_materials_name ON materials (name);

CREATE INDEX ix_materials_supplier_id ON materials (supplier_id);

CREATE UNIQUE INDEX ix_mechanical_properties_name ON mechanical_properties (name);

CREATE UNIQUE INDEX ix_post_processing_methods_name ON post_processing_methods (name);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20251119055602_InitialCreate', '9.0.1');

CREATE INDEX ix_materials_active ON materials (active);

CREATE INDEX ix_materials_created_at ON materials (created_at);

CREATE INDEX ix_materials_price_per_unit ON materials (price_per_unit);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20251119065808_AddPerformanceIndexes', '9.0.1');

COMMIT;

