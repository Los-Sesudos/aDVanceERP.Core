-- ============================================================================
-- aDVance · Migración V001 · Compra: recepción, estados, historial de costo, bitácora
-- BORRADOR PARA REVISIÓN (2026-11-20). NO APLICAR sin: (1) copia de seguridad (mysqldump),
-- (2) revisar las comprobaciones previas, (3) probar primero en una copia de la BD.
-- Compatible con MySQL 8 y MariaDB (no usa "ADD COLUMN IF NOT EXISTS"). Ejecutar UNA sola vez.
-- Retrocompatible: las columnas nuevas tienen DEFAULT, un exe viejo sigue funcionando.
-- Todo InnoDB (nada de MyISAM).
-- ============================================================================

-- ---------- COMPROBACIONES PREVIAS (solo lectura; ejecutar antes y revisar) ----------
-- SELECT VERSION();
-- SELECT COUNT(*) FROM adv__compra;                         -- se espera 0 (no hay compras registradas)
-- SELECT * FROM adv__tipo_movimiento ORDER BY id_tipo_movimiento;  -- ids existentes (el código usa 1,2,4,11)
-- SHOW TABLES LIKE 'adv__migracion';                        -- no debería existir aún

-- ---------- 0. Registro de migraciones (empieza a resolver C-02 de forma manual) ----------
CREATE TABLE IF NOT EXISTS adv__migracion (
  id_migracion   int          NOT NULL AUTO_INCREMENT,
  version        varchar(20)  NOT NULL,
  descripcion    varchar(200) NOT NULL,
  checksum       varchar(64)  DEFAULT NULL,
  fecha_aplicada datetime     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id_migracion),
  UNIQUE KEY uk_migracion_version (version)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------- 1. Compra: ejes de recepción y pago (separados del estado del documento) ----------
ALTER TABLE adv__compra
  ADD COLUMN estado_recepcion enum('SinRecibir','Parcial','Completa') NOT NULL DEFAULT 'SinRecibir',
  ADD COLUMN estado_pago      enum('SinPago','Parcial','Pagada')      NOT NULL DEFAULT 'SinPago',
  ADD KEY idx_compra_proveedor (id_proveedor),
  ADD KEY idx_compra_fecha (fecha);

-- ---------- 2. Detalle de compra: acumulados de recepción y devolución ----------
-- Los acumulados se actualizan en la misma transacción que la recepción/devolución; la fuente de verdad
-- son adv__detalle_recepcion_compra y los movimientos de devolución.
ALTER TABLE adv__detalle_compra_producto
  ADD COLUMN cantidad_recibida decimal(10,2) NOT NULL DEFAULT 0.00,   -- en unidades de la presentación de la línea
  ADD COLUMN cantidad_devuelta decimal(10,2) NOT NULL DEFAULT 0.00,
  -- Moneda y presentación por línea (decisión 2026-11-21). Totales y deuda en moneda base.
  ADD COLUMN id_moneda            int           DEFAULT NULL,                 -- moneda en que se pactó el costo de la línea
  ADD COLUMN tasa_cambio_aplicada decimal(18,6) NOT NULL DEFAULT 1.000000,    -- tasa moneda de la línea -> base, del día de la compra (de Configuración)
  ADD COLUMN costo_unitario_base  decimal(14,4) NOT NULL DEFAULT 0.0000,      -- costo_unitario × tasa: por presentación, en moneda base
  ADD COLUMN factor_presentacion  decimal(14,4) NOT NULL DEFAULT 1.0000,      -- unidades base por presentación, congelado al comprar
  ADD COLUMN cantidad_base        decimal(14,4) NOT NULL DEFAULT 0.0000;      -- cantidad × factor: lo que entra al inventario

-- ---------- 3. Detalle de recepción: costo real aplicado (para revertir devoluciones al mismo costo) ----------
ALTER TABLE adv__detalle_recepcion_compra
  ADD COLUMN costo_unitario_base decimal(14,4) NOT NULL DEFAULT 0.0000,  -- costo real en moneda base (desc., imp. y tasa)
  ADD COLUMN tasa_cambio_aplicada decimal(18,6) NOT NULL DEFAULT 1.000000;

-- ---------- 4. Historial de costo por producto (solo inserciones) ----------
CREATE TABLE IF NOT EXISTS adv__historial_costo_producto (
  id_historial_costo        bigint        NOT NULL AUTO_INCREMENT,
  id_producto               int           NOT NULL,
  id_almacen                int           NOT NULL,
  fecha                     datetime      NOT NULL DEFAULT CURRENT_TIMESTAMP,
  origen                    enum('Recepcion','Devolucion','Ajuste') NOT NULL,
  id_compra                 int           DEFAULT NULL,
  id_recepcion_compra       int           DEFAULT NULL,
  id_proveedor              int           DEFAULT NULL,
  cantidad                  decimal(10,2) NOT NULL,
  id_moneda                 int           DEFAULT NULL,
  costo_unitario_original   decimal(14,4) DEFAULT NULL,   -- en la moneda pactada
  tasa_cambio_aplicada      decimal(18,6) DEFAULT NULL,
  costo_unitario_base       decimal(14,4) NOT NULL,       -- costo real en moneda base
  costo_promedio_resultante decimal(12,4) NOT NULL,
  existencia_resultante     decimal(10,2) NOT NULL,
  id_cuenta_usuario         int           DEFAULT NULL,
  PRIMARY KEY (id_historial_costo),
  KEY idx_hist_producto_fecha (id_producto, fecha),
  KEY idx_hist_compra (id_compra)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------- 5. Bitácora de eventos de la compra (aseguramiento) ----------
CREATE TABLE IF NOT EXISTS adv__compra_evento (
  id_compra_evento  bigint       NOT NULL AUTO_INCREMENT,
  id_compra         int          NOT NULL,
  fecha             datetime     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  tipo              enum('Registro','Edicion','Recepcion','Pago','PagoAnulado','Devolucion','Anulacion') NOT NULL,
  id_cuenta_usuario int          DEFAULT NULL,
  motivo            varchar(255) DEFAULT NULL,
  detalle           text,                                  -- antes/después o resumen (texto/JSON)
  PRIMARY KEY (id_compra_evento),
  KEY idx_evento_compra (id_compra, fecha)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------- 6. Tipo de movimiento para devolución a proveedor (si no existe) ----------
INSERT INTO adv__tipo_movimiento (nombre, efecto)
SELECT 'Devolución a proveedor', 'Descarga' FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM adv__tipo_movimiento WHERE nombre = 'Devolución a proveedor');

-- ---------- 7. Índices de pagos (hoy se filtra por id_compra/id_venta sin índice) ----------
ALTER TABLE adv__pago
  ADD KEY idx_pago_compra (id_compra),
  ADD KEY idx_pago_venta (id_venta);

-- ---------- 8. Registrar la migración ----------
INSERT INTO adv__migracion (version, descripcion) VALUES ('001', 'compra_recepcion_costos_bitacora');

-- ---------- COMPROBACIONES POSTERIORES ----------
-- SHOW COLUMNS FROM adv__compra;  SHOW TABLES LIKE 'adv__%historial%';  SELECT * FROM adv__migracion;
