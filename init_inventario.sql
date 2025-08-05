
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TYPE tipo_transaccion AS ENUM ('compra', 'venta');

CREATE TABLE productos (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    nombre VARCHAR(100),
    descripcion TEXT,
    categoria VARCHAR(50),
    imagen TEXT,
    precio NUMERIC(10, 2),
    stock INT
);

CREATE TABLE transacciones (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    tipo_transaccion tipo_transaccion NOT NULL,
    producto_id UUID REFERENCES productos(id),
    cantidad INT,
    precio_unitario NUMERIC(10, 2),
    precio_total NUMERIC(10, 2),
    detalle TEXT
);
