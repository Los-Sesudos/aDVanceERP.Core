-- phpMyAdmin SQL Dump
-- version 5.2.3
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1:3306
-- Tiempo de generación: 07-06-2026 a las 20:23:12
-- Versión del servidor: 8.4.7
-- Versión de PHP: 8.3.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `advanceerp`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__almacen`
--

DROP TABLE IF EXISTS `adv__almacen`;
CREATE TABLE IF NOT EXISTS `adv__almacen` (
  `id_almacen` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `descripcion` text COMMENT 'Descripción opcional del almacén',
  `direccion` varchar(255) NOT NULL,
  `tipo` enum('Primario','Secundario','Transito','Especial') NOT NULL DEFAULT 'Secundario',
  `estado` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_almacen`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__auditoria_inventario`
--

DROP TABLE IF EXISTS `adv__auditoria_inventario`;
CREATE TABLE IF NOT EXISTS `adv__auditoria_inventario` (
  `id_auditoria` bigint NOT NULL AUTO_INCREMENT,
  `id_inventario` int NOT NULL,
  `id_producto` int NOT NULL,
  `id_almacen` int NOT NULL,
  `cantidad_anterior` decimal(10,2) NOT NULL DEFAULT '0.00',
  `costo_prom_anterior` decimal(12,4) NOT NULL DEFAULT '0.0000',
  `valor_total_anterior` decimal(12,4) NOT NULL DEFAULT '0.0000',
  `cantidad_nueva` decimal(10,2) NOT NULL DEFAULT '0.00',
  `costo_prom_nuevo` decimal(12,4) NOT NULL DEFAULT '0.0000',
  `valor_total_nuevo` decimal(12,4) NOT NULL DEFAULT '0.0000',
  `diferencia_cantidad` decimal(10,2) GENERATED ALWAYS AS ((`cantidad_nueva` - `cantidad_anterior`)) STORED,
  `diferencia_valor` decimal(12,4) GENERATED ALWAYS AS ((`valor_total_nuevo` - `valor_total_anterior`)) STORED,
  `id_movimiento_ref` int DEFAULT NULL,
  `id_cuenta_usuario` int DEFAULT NULL,
  `origen` enum('Movimiento','AjusteManual','Carga_Inicial','Sistema','Trigger') NOT NULL DEFAULT 'Trigger',
  `observacion` text,
  `fecha_registro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_auditoria`),
  KEY `idx_inventario` (`id_inventario`),
  KEY `idx_producto` (`id_producto`),
  KEY `idx_almacen` (`id_almacen`),
  KEY `idx_movimiento` (`id_movimiento_ref`),
  KEY `idx_usuario` (`id_cuenta_usuario`),
  KEY `idx_fecha` (`fecha_registro`),
  KEY `idx_origen` (`origen`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Historial completo de cambios en el inventario';

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__caja_arqueo`
--

DROP TABLE IF EXISTS `adv__caja_arqueo`;
CREATE TABLE IF NOT EXISTS `adv__caja_arqueo` (
  `id_arqueo` int NOT NULL AUTO_INCREMENT,
  `id_turno` int NOT NULL,
  `denominacion` decimal(10,2) NOT NULL,
  `cantidad` int NOT NULL DEFAULT '0',
  `subtotal` decimal(12,2) GENERATED ALWAYS AS ((`denominacion` * `cantidad`)) STORED,
  `id_moneda` int NOT NULL DEFAULT '1',
  `tasa_cambio_aplicada` decimal(18,6) NOT NULL DEFAULT '1.000000',
  PRIMARY KEY (`id_arqueo`),
  UNIQUE KEY `uq_arqueo_turno_den` (`id_turno`,`denominacion`),
  KEY `idx_arqueo_turno` (`id_turno`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Migración: soporte multimoneda para adv__caja_arqueo
-- La columna id_moneda ya existe (default 1 = CUP), pero la llave única
-- actual (id_turno, denominacion) no distingue entre monedas.
-- Sin este cambio, dos denominaciones iguales en monedas distintas
-- (ej. un billete "5" en USD y otro "5" en EUR) colisionarían en el
-- INSERT ... ON DUPLICATE KEY UPDATE de RepoCajaArqueo.

ALTER TABLE adv__caja_arqueo
    DROP INDEX uq_arqueo_turno_den,
    ADD UNIQUE KEY uq_arqueo_turno_moneda_den (id_turno, id_moneda, denominacion);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__caja_movimiento`
--

DROP TABLE IF EXISTS `adv__caja_movimiento`;
CREATE TABLE IF NOT EXISTS `adv__caja_movimiento` (
  `id_movimiento_caja` int NOT NULL AUTO_INCREMENT,
  `id_turno` int NOT NULL,
  `tipo` enum('Venta','DevolucionVenta','EntradaManual','SalidaManual','AjusteArqueo') NOT NULL,
  `canal_pago` enum('Efectivo','TransferenciaBancaria') CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Efectivo',
  `id_venta` int DEFAULT NULL,
  `monto` decimal(12,2) NOT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  `id_cuenta_usuario` int NOT NULL,
  `fecha_movimiento` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `id_moneda` int NOT NULL DEFAULT '1',
  `monto_moneda_base` decimal(12,2) NOT NULL DEFAULT '0.00',
  `tasa_cambio_aplicada` decimal(18,6) NOT NULL DEFAULT '1.000000',
  PRIMARY KEY (`id_movimiento_caja`),
  KEY `idx_movim_turno` (`id_turno`),
  KEY `idx_movim_tipo` (`tipo`),
  KEY `idx_movim_canal` (`canal_pago`),
  KEY `idx_movim_venta` (`id_venta`),
  KEY `idx_movim_fecha` (`fecha_movimiento`),
  KEY `idx_movim_turno_canal` (`id_turno`,`canal_pago`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__caja_turno`
--

DROP TABLE IF EXISTS `adv__caja_turno`;
CREATE TABLE IF NOT EXISTS `adv__caja_turno` (
  `id_turno` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(30) NOT NULL,
  `id_almacen` int NOT NULL,
  `id_punto_venta` int DEFAULT NULL,
  `id_cuenta_apertura` int NOT NULL,
  `id_cuenta_cierre` int DEFAULT NULL,
  `fecha_apertura` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_cierre` datetime DEFAULT NULL,
  `monto_apertura` decimal(12,2) NOT NULL DEFAULT '0.00',
  `monto_efectivo_calculado` decimal(12,2) DEFAULT NULL,
  `monto_efectivo_declarado` decimal(12,2) DEFAULT NULL,
  `diferencia_efectivo` decimal(12,2) GENERATED ALWAYS AS ((`monto_efectivo_declarado` - `monto_efectivo_calculado`)) STORED,
  `monto_transferencias_calculado` decimal(12,2) DEFAULT NULL,
  `monto_transferencias_declarado` decimal(12,2) DEFAULT NULL,
  `diferencia_transferencias` decimal(12,2) GENERATED ALWAYS AS ((`monto_transferencias_declarado` - `monto_transferencias_calculado`)) STORED,
  `estado` enum('Abierto','Cerrado','Anulado') NOT NULL DEFAULT 'Abierto',
  `observaciones_apertura` text,
  `observaciones_cierre` text,
  PRIMARY KEY (`id_turno`),
  UNIQUE KEY `uq_turno_codigo` (`codigo`),
  KEY `idx_turno_almacen` (`id_almacen`),
  KEY `idx_turno_estado` (`estado`),
  KEY `idx_turno_almacen_estado` (`id_almacen`,`estado`),
  KEY `idx_turno_cuenta_apertura` (`id_cuenta_apertura`),
  KEY `idx_turno_fecha_apertura` (`fecha_apertura`),
  KEY `idx_turno_punto_venta` (`id_punto_venta`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__clasificacion_producto`
--

DROP TABLE IF EXISTS `adv__clasificacion_producto`;
CREATE TABLE IF NOT EXISTS `adv__clasificacion_producto` (
  `id_clasificacion_producto` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `descripcion` text,
  PRIMARY KEY (`id_clasificacion_producto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__cliente`
--

DROP TABLE IF EXISTS `adv__cliente`;
CREATE TABLE IF NOT EXISTS `adv__cliente` (
  `id_cliente` int NOT NULL AUTO_INCREMENT,
  `id_persona` int NOT NULL,
  `codigo_cliente` varchar(50) NOT NULL,
  `limite_credito` decimal(10,2) DEFAULT '0.00',
  `fecha_registro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_cliente`),
  UNIQUE KEY `uk_codigo_cliente` (`codigo_cliente`),
  KEY `fk_cliente_persona_idx` (`id_persona`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__compra`
--

DROP TABLE IF EXISTS `adv__compra`;
CREATE TABLE IF NOT EXISTS `adv__compra` (
  `id_compra` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(50) NOT NULL,
  `id_proveedor` int NOT NULL,
  `id_cuenta_usuario` int DEFAULT NULL,
  `id_almacen_destino` int NOT NULL,
  `fecha` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `subtotal` decimal(12,2) NOT NULL DEFAULT '0.00',
  `descuento_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `impuesto_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `importe_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `estado_compra` enum('Pendiente','Completada','Anulada') CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Pendiente',
  `observaciones` text,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  `id_moneda` int NOT NULL DEFAULT '1',
  `tasa_cambio_aplicada` decimal(18,6) NOT NULL DEFAULT '1.000000',
  PRIMARY KEY (`id_compra`),
  UNIQUE KEY `uk_codigo_compra` (`codigo`),
  KEY `idx_estado_compra` (`estado_compra`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__correo_contacto`
--

DROP TABLE IF EXISTS `adv__correo_contacto`;
CREATE TABLE IF NOT EXISTS `adv__correo_contacto` (
  `id_correo_contacto` int NOT NULL AUTO_INCREMENT,
  `direccion_correo` varchar(255) NOT NULL,
  `categoria` enum('Personal','Trabajo','Otro') NOT NULL DEFAULT 'Personal',
  `id_persona` int NOT NULL,
  PRIMARY KEY (`id_correo_contacto`),
  KEY `fk_correo_persona_idx` (`id_persona`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__cuenta_usuario`
--

DROP TABLE IF EXISTS `adv__cuenta_usuario`;
CREATE TABLE IF NOT EXISTS `adv__cuenta_usuario` (
  `id_cuenta_usuario` int NOT NULL AUTO_INCREMENT,
  `id_persona` int NOT NULL DEFAULT '0',
  `nombre` varchar(100) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `password_salt` varchar(255) NOT NULL,
  `email` varchar(255) DEFAULT NULL,
  `id_rol` int NOT NULL DEFAULT '0',
  `administrador` tinyint(1) NOT NULL DEFAULT '0',
  `aprobado` tinyint(1) NOT NULL DEFAULT '0',
  `estado` tinyint(1) NOT NULL DEFAULT '1',
  `ultimo_acceso` datetime DEFAULT NULL,
  PRIMARY KEY (`id_cuenta_usuario`),
  UNIQUE KEY `uk_email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__detalle_auditoria`
--

DROP TABLE IF EXISTS `adv__detalle_auditoria`;
CREATE TABLE IF NOT EXISTS `adv__detalle_auditoria` (
  `id_detalle_auditoria` bigint NOT NULL AUTO_INCREMENT,
  `id_sesion_auditoria` int NOT NULL,
  `id_producto` int NOT NULL,
  `id_almacen` int NOT NULL,
  `cantidad_sistema` decimal(10,2) NOT NULL DEFAULT '0.00',
  `costo_prom_sistema` decimal(12,4) NOT NULL DEFAULT '0.0000',
  `cantidad_contada` decimal(10,2) DEFAULT NULL,
  `contado_por` int DEFAULT NULL,
  `fecha_conteo` datetime DEFAULT NULL,
  `diferencia` decimal(10,2) GENERATED ALWAYS AS ((case when (`cantidad_contada` is not null) then (`cantidad_contada` - `cantidad_sistema`) else NULL end)) STORED,
  `valor_diferencia` decimal(12,4) GENERATED ALWAYS AS ((case when (`cantidad_contada` is not null) then ((`cantidad_contada` - `cantidad_sistema`) * `costo_prom_sistema`) else NULL end)) STORED,
  `ajuste_aplicado` tinyint(1) NOT NULL DEFAULT '0',
  `id_movimiento_ajuste` int DEFAULT NULL,
  `observacion` text,
  PRIMARY KEY (`id_detalle_auditoria`),
  UNIQUE KEY `uk_sesion_producto` (`id_sesion_auditoria`,`id_producto`),
  KEY `idx_sesion` (`id_sesion_auditoria`),
  KEY `idx_producto` (`id_producto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__detalle_compra_producto`
--

DROP TABLE IF EXISTS `adv__detalle_compra_producto`;
CREATE TABLE IF NOT EXISTS `adv__detalle_compra_producto` (
  `id_detalle_compra_producto` bigint NOT NULL AUTO_INCREMENT,
  `id_compra` int NOT NULL,
  `id_producto` int NOT NULL,
  `id_presentacion` int DEFAULT NULL,
  `cantidad` decimal(10,2) NOT NULL,
  `costo_unitario` decimal(10,2) NOT NULL,
  `descuento_item` decimal(12,2) NOT NULL DEFAULT '0.00',
  `impuesto_adicional_item` decimal(12,2) NOT NULL DEFAULT '0.00',
  `subtotal` decimal(12,4) GENERATED ALWAYS AS ((`cantidad` * `costo_unitario`)) STORED,
  PRIMARY KEY (`id_detalle_compra_producto`),
  KEY `idx_compra` (`id_compra`),
  KEY `idx_producto` (`id_producto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__detalle_pago_transferencia`
--

DROP TABLE IF EXISTS `adv__detalle_pago_transferencia`;
CREATE TABLE IF NOT EXISTS `adv__detalle_pago_transferencia` (
  `id_detalle_pago_transferencia` int NOT NULL AUTO_INCREMENT,
  `id_pago` int NOT NULL,
  `numero_telefono_remitente` varchar(100) NOT NULL,
  `numero_transaccion` varchar(100) NOT NULL,
  `monto_transferencia` decimal(10,2) NOT NULL,
  `id_moneda` int NOT NULL DEFAULT '1',
  `monto_moneda_base` decimal(12,2) NOT NULL DEFAULT '0.00',
  `tasa_cambio_aplicada` decimal(18,6) NOT NULL DEFAULT '1.000000',
  PRIMARY KEY (`id_detalle_pago_transferencia`),
  UNIQUE KEY `uk_numero_transaccion` (`numero_transaccion`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__detalle_pedido_producto`
--

DROP TABLE IF EXISTS `adv__detalle_pedido_producto`;
CREATE TABLE IF NOT EXISTS `adv__detalle_pedido_producto` (
  `id_detalle_pedido_producto` bigint NOT NULL AUTO_INCREMENT,
  `id_pedido` int NOT NULL,
  `id_producto` int NOT NULL,
  `id_presentacion` int DEFAULT NULL,
  `cantidad_solicitada` decimal(10,2) NOT NULL,
  `precio_venta_referencia` decimal(10,2) NOT NULL,
  `subtotal` decimal(12,4) GENERATED ALWAYS AS ((`cantidad_solicitada` * `precio_venta_referencia`)) STORED,
  PRIMARY KEY (`id_detalle_pedido_producto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__detalle_recepcion_compra`
--

DROP TABLE IF EXISTS `adv__detalle_recepcion_compra`;
CREATE TABLE IF NOT EXISTS `adv__detalle_recepcion_compra` (
  `id_detalle_recepcion_compra` bigint NOT NULL AUTO_INCREMENT,
  `id_recepcion_compra` int NOT NULL,
  `id_detalle_compra_producto` bigint NOT NULL,
  `cantidad_recibida` decimal(10,2) NOT NULL,
  PRIMARY KEY (`id_detalle_recepcion_compra`),
  KEY `idx_recepcion` (`id_recepcion_compra`),
  KEY `idx_detalle_compra` (`id_detalle_compra_producto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__detalle_solicitud_compra`
--

DROP TABLE IF EXISTS `adv__detalle_solicitud_compra`;
CREATE TABLE IF NOT EXISTS `adv__detalle_solicitud_compra` (
  `id_detalle_solicitud_compra` bigint NOT NULL AUTO_INCREMENT,
  `id_solicitud_compra` int NOT NULL,
  `id_producto` int NOT NULL,
  `cantidad_solicitada` decimal(10,2) NOT NULL,
  `precio_adquisicion_referencia` decimal(10,2) NOT NULL,
  `subtotal` decimal(12,4) GENERATED ALWAYS AS ((`cantidad_solicitada` * `precio_adquisicion_referencia`)) STORED,
  PRIMARY KEY (`id_detalle_solicitud_compra`),
  KEY `idx_solicitud_compra` (`id_solicitud_compra`),
  KEY `idx_producto` (`id_producto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__detalle_venta_producto`
--

DROP TABLE IF EXISTS `adv__detalle_venta_producto`;
CREATE TABLE IF NOT EXISTS `adv__detalle_venta_producto` (
  `id_detalle_venta_producto` bigint NOT NULL AUTO_INCREMENT,
  `id_venta` int NOT NULL,
  `id_producto` int NOT NULL,
  `id_presentacion` int DEFAULT NULL,
  `cantidad` decimal(10,2) NOT NULL,
  `precio_compra_vigente` decimal(10,2) NOT NULL,
  `precio_venta_unitario` decimal(10,2) NOT NULL,
  `descuento_item` decimal(12,2) NOT NULL DEFAULT '0.00',
  `impuesto_adicional_item` decimal(12,2) NOT NULL DEFAULT '0.00',
  `subtotal` decimal(12,4) GENERATED ALWAYS AS (((`precio_venta_unitario` - `descuento_item`) * `cantidad`)) STORED,
  PRIMARY KEY (`id_detalle_venta_producto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__empleado`
--

DROP TABLE IF EXISTS `adv__empleado`;
CREATE TABLE IF NOT EXISTS `adv__empleado` (
  `id_empleado` int NOT NULL AUTO_INCREMENT,
  `id_persona` int NOT NULL,
  `codigo_empleado` varchar(50) NOT NULL,
  `fecha_contratacion` date NOT NULL,
  `fecha_nacimiento` date DEFAULT NULL,
  `cargo` varchar(255) DEFAULT NULL,
  `departamento` varchar(255) DEFAULT NULL,
  `salario` decimal(10,2) DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_empleado`),
  UNIQUE KEY `uk_codigo_empleado` (`codigo_empleado`),
  KEY `fk_empleado_persona_idx` (`id_persona`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__empresa`
--

DROP TABLE IF EXISTS `adv__empresa`;
CREATE TABLE IF NOT EXISTS `adv__empresa` (
  `id_empresa` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(255) NOT NULL,
  `razon_social` varchar(255) DEFAULT NULL,
  `rif` varchar(50) DEFAULT NULL,
  `direccion` varchar(500) DEFAULT NULL,
  `telefono` varchar(50) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `web` varchar(255) DEFAULT NULL,
  `ruta_logo` varchar(500) DEFAULT NULL,
  `fecha_registro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_empresa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__inventario`
--

DROP TABLE IF EXISTS `adv__inventario`;
CREATE TABLE IF NOT EXISTS `adv__inventario` (
  `id_inventario` int NOT NULL AUTO_INCREMENT,
  `id_producto` int NOT NULL,
  `id_almacen` int NOT NULL,
  `cantidad` decimal(10,2) NOT NULL,
  `cantidad_minima` decimal(10,2) NOT NULL DEFAULT '0.00',
  `costo_promedio` decimal(12,4) NOT NULL DEFAULT '0.0000',
  `valor_total` decimal(12,4) NOT NULL DEFAULT '0.0000',
  `ultima_actualizacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_inventario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Disparadores `adv__inventario`
--
DROP TRIGGER IF EXISTS `trg_auditoria_inventario_after_insert`;
DELIMITER $$
CREATE TRIGGER `trg_auditoria_inventario_after_insert` AFTER INSERT ON `adv__inventario` FOR EACH ROW BEGIN
    DECLARE v_id_movimiento       INT     DEFAULT NULL;
    DECLARE v_id_tipo_movimiento  INT     DEFAULT NULL;
    DECLARE v_id_cuenta_usuario   INT     DEFAULT NULL;
    DECLARE v_origen              VARCHAR(20) DEFAULT 'Trigger';
    DECLARE v_observacion         TEXT    DEFAULT 'Inserción inicial detectada automáticamente';

    IF NEW.cantidad > 0 THEN
        SELECT
            id_movimiento,
            id_tipo_movimiento,
            id_cuenta_usuario
        INTO
            v_id_movimiento,
            v_id_tipo_movimiento,
            v_id_cuenta_usuario
        FROM adv__movimiento
        WHERE id_producto = NEW.id_producto
          AND id_almacen_destino = NEW.id_almacen
          AND estado IN ('Pendiente', 'Completado')
          AND fecha_creacion >= NOW() - INTERVAL 10 SECOND
        ORDER BY fecha_creacion DESC
        LIMIT 1;

        SET v_origen = CASE v_id_tipo_movimiento
            WHEN 1  THEN 'Movimiento'
            WHEN 11 THEN 'Carga_Inicial'
            WHEN 7  THEN 'Movimiento'
            ELSE         'Carga_Inicial'
        END;

        SET v_observacion = CASE v_id_tipo_movimiento
            WHEN 1  THEN 'Primera entrada por compra en este almacén'
            WHEN 11 THEN 'Carga inicial de inventario'
            WHEN 7  THEN 'Primera entrada de producción en este almacén'
            ELSE        'Carga inicial detectada (sin movimiento correlacionado)'
        END;

        INSERT INTO `adv__auditoria_inventario` (
            id_inventario, id_producto, id_almacen,
            cantidad_anterior, costo_prom_anterior, valor_total_anterior,
            cantidad_nueva, costo_prom_nuevo, valor_total_nuevo,
            id_movimiento_ref, id_cuenta_usuario, origen, observacion
        ) VALUES (
            NEW.id_inventario, NEW.id_producto, NEW.id_almacen,
            0, 0, 0,
            NEW.cantidad, NEW.costo_promedio, NEW.valor_total,
            v_id_movimiento, v_id_cuenta_usuario, v_origen, v_observacion
        );
    END IF;
END
$$
DELIMITER ;
DROP TRIGGER IF EXISTS `trg_auditoria_inventario_after_update`;
DELIMITER $$
CREATE TRIGGER `trg_auditoria_inventario_after_update` AFTER UPDATE ON `adv__inventario` FOR EACH ROW BEGIN
    DECLARE v_id_movimiento       INT     DEFAULT NULL;
    DECLARE v_id_tipo_movimiento  INT     DEFAULT NULL;
    DECLARE v_id_cuenta_usuario   INT     DEFAULT NULL;
    DECLARE v_origen              VARCHAR(20) DEFAULT 'Trigger';
    DECLARE v_observacion         TEXT    DEFAULT 'Cambio detectado automáticamente por trigger';

    IF (OLD.cantidad <> NEW.cantidad OR OLD.costo_promedio <> NEW.costo_promedio) THEN
        SELECT
            id_movimiento,
            id_tipo_movimiento,
            id_cuenta_usuario
        INTO
            v_id_movimiento,
            v_id_tipo_movimiento,
            v_id_cuenta_usuario
        FROM adv__movimiento
        WHERE id_producto = NEW.id_producto
          AND (id_almacen_destino = NEW.id_almacen OR id_almacen_origen = NEW.id_almacen)
          AND estado IN ('Pendiente', 'Completado')
          AND fecha_creacion >= NOW() - INTERVAL 10 SECOND
        ORDER BY fecha_creacion DESC
        LIMIT 1;

        SET v_origen = CASE v_id_tipo_movimiento
            WHEN 1  THEN 'Movimiento'
            WHEN 2  THEN 'Movimiento'
            WHEN 3  THEN 'Movimiento'
            WHEN 4  THEN 'Movimiento'
            WHEN 5  THEN 'AjusteManual'
            WHEN 6  THEN 'AjusteManual'
            WHEN 7  THEN 'Movimiento'
            WHEN 8  THEN 'Movimiento'
            WHEN 9  THEN 'Movimiento'
            WHEN 10 THEN 'Movimiento'
            WHEN 11 THEN 'Carga_Inicial'
            WHEN 12 THEN 'Movimiento'
            ELSE         'Trigger'
        END;

        SET v_observacion = CASE v_id_tipo_movimiento
            WHEN 1  THEN 'Entrada por compra'
            WHEN 2  THEN 'Salida por venta'
            WHEN 3  THEN 'Entrada por devolución de venta'
            WHEN 4  THEN 'Salida por devolución a proveedor'
            WHEN 5  THEN 'Ajuste manual positivo'
            WHEN 6  THEN 'Ajuste manual negativo'
            WHEN 7  THEN 'Entrada de producción'
            WHEN 8  THEN 'Salida a producción'
            WHEN 9  THEN 'Consumo interno'
            WHEN 10 THEN 'Merma o pérdida registrada'
            WHEN 11 THEN 'Carga inicial de inventario (actualización)'
            WHEN 12 THEN 'Traslado entre almacenes'
            ELSE        'Cambio detectado por trigger sin movimiento correlacionado'
        END;

        INSERT INTO `adv__auditoria_inventario` (
            id_inventario, id_producto, id_almacen,
            cantidad_anterior, costo_prom_anterior, valor_total_anterior,
            cantidad_nueva, costo_prom_nuevo, valor_total_nuevo,
            id_movimiento_ref, id_cuenta_usuario, origen, observacion
        ) VALUES (
            OLD.id_inventario, OLD.id_producto, OLD.id_almacen,
            OLD.cantidad, OLD.costo_promedio, OLD.valor_total,
            NEW.cantidad, NEW.costo_promedio, NEW.valor_total,
            v_id_movimiento, v_id_cuenta_usuario, v_origen, v_observacion
        );
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__mensajero`
--

DROP TABLE IF EXISTS `adv__mensajero`;
CREATE TABLE IF NOT EXISTS `adv__mensajero` (
  `id_mensajero` int NOT NULL AUTO_INCREMENT,
  `id_persona` int NOT NULL,
  `codigo_mensajero` varchar(50) NOT NULL,
  `matricula_vehiculo` varchar(20) DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_mensajero`),
  UNIQUE KEY `uk_codigo_mensajero` (`codigo_mensajero`),
  KEY `fk_mensajero_persona_idx` (`id_persona`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__moneda`
--

DROP TABLE IF EXISTS `adv__moneda`;
CREATE TABLE IF NOT EXISTS `adv__moneda` (
  `id_moneda` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(3) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `simbolo` varchar(5) NOT NULL,
  `precision_decimal` tinyint NOT NULL DEFAULT '2',
  `es_base` tinyint(1) NOT NULL DEFAULT '0',
  `activa` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_moneda`),
  UNIQUE KEY `uk_moneda_codigo` (`codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__movimiento`
--

DROP TABLE IF EXISTS `adv__movimiento`;
CREATE TABLE IF NOT EXISTS `adv__movimiento` (
  `id_movimiento` int NOT NULL AUTO_INCREMENT,
  `id_producto` int NOT NULL,
  `costo_unitario` decimal(10,2) NOT NULL,
  `costo_total` decimal(12,4) GENERATED ALWAYS AS ((`cantidad_movida` * `costo_unitario`)) STORED,
  `id_almacen_origen` int NOT NULL,
  `id_almacen_destino` int NOT NULL,
  `fecha_creacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `estado` enum('Pendiente','Completado','Cancelado','') NOT NULL,
  `saldo_inicial` decimal(10,2) NOT NULL,
  `fecha_termino` datetime NOT NULL,
  `cantidad_movida` decimal(10,2) NOT NULL,
  `saldo_final` decimal(10,2) NOT NULL,
  `id_tipo_movimiento` int NOT NULL DEFAULT '0',
  `id_cuenta_usuario` int NOT NULL,
  `notas` text,
  PRIMARY KEY (`id_movimiento`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__pago`
--

DROP TABLE IF EXISTS `adv__pago`;
CREATE TABLE IF NOT EXISTS `adv__pago` (
  `id_pago` int NOT NULL AUTO_INCREMENT,
  `id_compra` int DEFAULT '0',
  `id_venta` int DEFAULT '0',
  `metodo_pago` enum('Efectivo','TransferenciaBancaria') NOT NULL,
  `monto_pagado` decimal(10,2) NOT NULL,
  `fecha_pago` datetime DEFAULT NULL,
  `fecha_confirmacion_pago` datetime DEFAULT NULL,
  `estado_pago` enum('Pendiente','Confirmado','Fallido','Anulado') NOT NULL DEFAULT 'Pendiente',
  `id_moneda` int DEFAULT NULL,
  `monto_moneda_base` decimal(12,2) DEFAULT NULL,
  `tasa_cambio_aplicada` decimal(18,6) DEFAULT NULL,
  PRIMARY KEY (`id_pago`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__pago_detalle_moneda`
--

DROP TABLE IF EXISTS `adv__pago_detalle_moneda`;
CREATE TABLE IF NOT EXISTS `adv__pago_detalle_moneda` (
  `id_pago_detalle` int NOT NULL AUTO_INCREMENT,
  `id_pago` int NOT NULL,
  `id_moneda` int NOT NULL,
  `monto_moneda` decimal(12,2) NOT NULL,
  `monto_moneda_base` decimal(12,2) NOT NULL,
  `tasa_cambio_aplicada` decimal(18,6) NOT NULL,
  PRIMARY KEY (`id_pago_detalle`),
  KEY `idx_pago` (`id_pago`),
  KEY `idx_moneda` (`id_moneda`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__pedido`
--

DROP TABLE IF EXISTS `adv__pedido`;
CREATE TABLE IF NOT EXISTS `adv__pedido` (
  `id_pedido` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(50) NOT NULL,
  `id_cliente` int NOT NULL,
  `id_empleado_vendedor` int DEFAULT NULL,
  `fecha_pedido` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_entrega_solicitada` datetime DEFAULT NULL,
  `direccion_entrega` varchar(500) DEFAULT NULL,
  `total_pedido` decimal(12,2) NOT NULL DEFAULT '0.00',
  `estado_pedido` enum('Pendiente','Confirmado','Preparando','ListoParaRetirar','Retirado','Cancelado') NOT NULL DEFAULT 'Pendiente',
  `observaciones_pedido` text,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_pedido`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__permiso_rol`
--

DROP TABLE IF EXISTS `adv__permiso_rol`;
CREATE TABLE IF NOT EXISTS `adv__permiso_rol` (
  `id_permiso_rol` int NOT NULL AUTO_INCREMENT,
  `id_rol` int NOT NULL,
  `modulo` varchar(50) NOT NULL,
  `puede_ver` tinyint(1) NOT NULL DEFAULT '1',
  `puede_crear` tinyint(1) NOT NULL DEFAULT '0',
  `puede_editar` tinyint(1) NOT NULL DEFAULT '0',
  `puede_eliminar` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id_permiso_rol`),
  UNIQUE KEY `uk_rol_modulo` (`id_rol`,`modulo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__persona`
--

DROP TABLE IF EXISTS `adv__persona`;
CREATE TABLE IF NOT EXISTS `adv__persona` (
  `id_persona` int NOT NULL AUTO_INCREMENT,
  `nombre_completo` varchar(255) NOT NULL,
  `tipo_documento` varchar(50) DEFAULT NULL,
  `numero_documento` varchar(50) DEFAULT NULL,
  `direccion_principal` varchar(500) DEFAULT NULL,
  `fecha_registro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_persona`),
  UNIQUE KEY `uk_numero_documento` (`tipo_documento`,`numero_documento`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__precio_presentacion`
--

DROP TABLE IF EXISTS `adv__precio_presentacion`;
CREATE TABLE IF NOT EXISTS `adv__precio_presentacion` (
  `id_presentacion` int NOT NULL AUTO_INCREMENT,
  `id_producto` int NOT NULL,
  `id_unidad_medida` int NOT NULL,
  `cantidad` decimal(10,2) NOT NULL,
  `precio_venta` decimal(10,2) NOT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_presentacion`),
  KEY `idx_producto` (`id_producto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__producto`
--

DROP TABLE IF EXISTS `adv__producto`;
CREATE TABLE IF NOT EXISTS `adv__producto` (
  `id_producto` int NOT NULL AUTO_INCREMENT,
  `ruta_imagen` varchar(255) DEFAULT NULL,
  `categoria` enum('Mercancia','ProductoTerminado','MateriaPrima') NOT NULL DEFAULT 'Mercancia',
  `nombre` varchar(255) NOT NULL,
  `codigo` varchar(100) NOT NULL,
  `id_proveedor` int DEFAULT '0',
  `descripcion` text NOT NULL,
  `id_unidad_medida` int DEFAULT '0',
  `id_clasificacion_producto` int NOT NULL DEFAULT '1',
  `es_vendible` tinyint(1) NOT NULL DEFAULT '1',
  `costo_adquisicion_unitario` decimal(10,2) NOT NULL,
  `costo_produccion_unitario` decimal(10,2) NOT NULL,
  `impuesto_venta_porcentaje` decimal(5,2) NOT NULL DEFAULT '10.00',
  `margen_ganancia_deseado` decimal(5,2) NOT NULL DEFAULT '0.00',
  `precio_venta_base` decimal(10,2) NOT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_producto`),
  UNIQUE KEY `nombre_unico` (`nombre`),
  KEY `idx_categoria` (`categoria`),
  KEY `idx_codigo` (`codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__proveedor`
--

DROP TABLE IF EXISTS `adv__proveedor`;
CREATE TABLE IF NOT EXISTS `adv__proveedor` (
  `id_proveedor` int NOT NULL AUTO_INCREMENT,
  `id_persona` int NOT NULL,
  `codigo_proveedor` varchar(50) NOT NULL,
  `razon_social` varchar(255) NOT NULL,
  `nit` varchar(50) NOT NULL,
  `condiciones_pago` varchar(255) DEFAULT NULL,
  `fecha_registro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_proveedor`),
  UNIQUE KEY `uk_codigo_proveedor` (`codigo_proveedor`),
  UNIQUE KEY `uk_nit` (`nit`),
  KEY `fk_proveedor_persona_idx` (`id_persona`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__punto_venta`
--

DROP TABLE IF EXISTS `adv__punto_venta`;
CREATE TABLE IF NOT EXISTS `adv__punto_venta` (
  `id_punto_venta` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(50) NOT NULL,
  `nombre` varchar(150) NOT NULL,
  `id_almacen` int NOT NULL,
  `estado` enum('Activo','Inactivo','Mantenimiento') NOT NULL DEFAULT 'Activo',
  `observaciones` text,
  `fecha_registro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_punto_venta`),
  UNIQUE KEY `uk_codigo` (`codigo`),
  KEY `idx_almacen` (`id_almacen`),
  KEY `idx_estado` (`estado`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__recepcion_compra`
--

DROP TABLE IF EXISTS `adv__recepcion_compra`;
CREATE TABLE IF NOT EXISTS `adv__recepcion_compra` (
  `id_recepcion_compra` int NOT NULL AUTO_INCREMENT,
  `id_compra` int NOT NULL,
  `fecha_recepcion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `recibido_por` int NOT NULL,
  `observaciones` text,
  `id_movimiento_generado` int DEFAULT NULL,
  PRIMARY KEY (`id_recepcion_compra`),
  KEY `idx_compra` (`id_compra`),
  KEY `idx_movimiento` (`id_movimiento_generado`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__rol`
--

DROP TABLE IF EXISTS `adv__rol`;
CREATE TABLE IF NOT EXISTS `adv__rol` (
  `id_rol` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_rol`),
  UNIQUE KEY `uk_nombre` (`nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__seguimiento_entrega`
--

DROP TABLE IF EXISTS `adv__seguimiento_entrega`;
CREATE TABLE IF NOT EXISTS `adv__seguimiento_entrega` (
  `id_seguimiento_entrega` int NOT NULL AUTO_INCREMENT,
  `id_venta` int NOT NULL,
  `id_cliente` int DEFAULT NULL,
  `id_mensajero` int DEFAULT NULL,
  `tipo_envio` enum('RetiroEnLocal','MensajeriaConFondo','MensajeriaSinFondo') NOT NULL DEFAULT 'RetiroEnLocal',
  `fecha_asignacion` datetime DEFAULT NULL,
  `fecha_entrega_realizada` datetime DEFAULT NULL,
  `fecha_pago_negocio` datetime DEFAULT NULL,
  `estado_entrega` enum('PendienteAsignación','Asignado','EnRuta','Entregado','PagoRecibido','Completado','Cancelado','Fallido','EnEspera') NOT NULL DEFAULT 'PendienteAsignación',
  `monto_cobrado_al_cliente` decimal(10,2) DEFAULT '0.00',
  `observaciones_entrega` text,
  PRIMARY KEY (`id_seguimiento_entrega`),
  UNIQUE KEY `uk_id_venta` (`id_venta`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__servicio_actividad`
--

DROP TABLE IF EXISTS `adv__servicio_actividad`;
CREATE TABLE IF NOT EXISTS `adv__servicio_actividad` (
  `id_servicio_actividad` int NOT NULL AUTO_INCREMENT,
  `id_servicio_tipo` int DEFAULT NULL,
  `codigo` varchar(50) NOT NULL,
  `nombre` varchar(150) NOT NULL,
  `descripcion` text,
  `precio_base` decimal(10,2) NOT NULL DEFAULT '0.00',
  `unidad_medida` varchar(50) DEFAULT 'Servicio',
  `tiempo_estimado_minutos` int DEFAULT NULL,
  `requiere_materiales` tinyint(1) NOT NULL DEFAULT '0',
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_servicio_actividad`),
  UNIQUE KEY `uk_codigo_actividad` (`codigo`),
  KEY `idx_servicio_tipo` (`id_servicio_tipo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__servicio_bitacora`
--

DROP TABLE IF EXISTS `adv__servicio_bitacora`;
CREATE TABLE IF NOT EXISTS `adv__servicio_bitacora` (
  `id_servicio_bitacora` bigint NOT NULL AUTO_INCREMENT,
  `id_servicio_orden` int NOT NULL,
  `id_cuenta_usuario` int DEFAULT NULL,
  `fecha_registro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `tipo_evento` enum('Creacion','Programacion','Inicio','Pausa','Reanudacion','Completado','Nota','CambioEstado') NOT NULL,
  `descripcion` text NOT NULL,
  `foto_url` varchar(500) DEFAULT NULL COMMENT 'URL de foto evidencia si aplica',
  `ubicacion_latitud` decimal(10,8) DEFAULT NULL,
  `ubicacion_longitud` decimal(11,8) DEFAULT NULL,
  PRIMARY KEY (`id_servicio_bitacora`),
  KEY `idx_orden` (`id_servicio_orden`),
  KEY `idx_usuario` (`id_cuenta_usuario`),
  KEY `idx_fecha` (`fecha_registro`),
  KEY `idx_tipo_evento` (`tipo_evento`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__servicio_garantia`
--

DROP TABLE IF EXISTS `adv__servicio_garantia`;
CREATE TABLE IF NOT EXISTS `adv__servicio_garantia` (
  `id_servicio_garantia` int NOT NULL AUTO_INCREMENT,
  `id_servicio_orden` int NOT NULL,
  `codigo` varchar(50) NOT NULL,
  `fecha_inicio` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_fin` datetime NOT NULL,
  `dias_garantia` int NOT NULL,
  `estado` enum('Activa','Vencida','Utilizada','Anulada') NOT NULL DEFAULT 'Activa',
  `descripcion_cobertura` text,
  `observaciones` text,
  `id_servicio_orden_reclamo` int DEFAULT NULL COMMENT 'Orden creada por reclamo de garantía',
  PRIMARY KEY (`id_servicio_garantia`),
  UNIQUE KEY `uk_codigo_garantia` (`codigo`),
  KEY `idx_orden` (`id_servicio_orden`),
  KEY `idx_estado` (`estado`),
  KEY `idx_fecha_fin` (`fecha_fin`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__servicio_orden`
--

DROP TABLE IF EXISTS `adv__servicio_orden`;
CREATE TABLE IF NOT EXISTS `adv__servicio_orden` (
  `id_servicio_orden` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(50) NOT NULL,
  `id_cliente` int NOT NULL,
  `id_cuenta_usuario` int DEFAULT NULL,
  `id_servicio_tipo` int DEFAULT NULL,
  `id_servicio_presupuesto` int DEFAULT NULL COMMENT 'Referencia al presupuesto si existe',
  `id_venta` int DEFAULT NULL COMMENT 'Referencia a factura si ya se facturó',
  `fecha_creacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_inicio_programada` datetime DEFAULT NULL,
  `fecha_fin_programada` datetime DEFAULT NULL,
  `fecha_inicio_real` datetime DEFAULT NULL,
  `fecha_fin_real` datetime DEFAULT NULL,
  `estado` enum('Pendiente','Programada','EnProgreso','EnPausa','Completada','Facturada','Anulada') NOT NULL DEFAULT 'Pendiente',
  `prioridad` enum('Baja','Media','Alta','Urgente') NOT NULL DEFAULT 'Media',
  `ubicacion_servicio` varchar(255) DEFAULT NULL,
  `contacto_cliente` varchar(100) DEFAULT NULL,
  `telefono_contacto` varchar(20) DEFAULT NULL,
  `subtotal` decimal(12,2) NOT NULL DEFAULT '0.00',
  `descuento_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `impuesto_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `importe_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `observaciones` text,
  `notas_internas` text,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  `id_movimiento_ingreso` int DEFAULT NULL COMMENT 'Movimiento de ingreso por cobro',
  PRIMARY KEY (`id_servicio_orden`),
  UNIQUE KEY `uk_codigo_orden` (`codigo`),
  KEY `idx_cliente` (`id_cliente`),
  KEY `idx_usuario` (`id_cuenta_usuario`),
  KEY `idx_servicio_tipo` (`id_servicio_tipo`),
  KEY `idx_presupuesto` (`id_servicio_presupuesto`),
  KEY `idx_venta` (`id_venta`),
  KEY `idx_estado` (`estado`),
  KEY `idx_prioridad` (`prioridad`),
  KEY `idx_fecha_creacion` (`fecha_creacion`),
  KEY `idx_movimiento_ingreso` (`id_movimiento_ingreso`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__servicio_orden_detalle`
--

DROP TABLE IF EXISTS `adv__servicio_orden_detalle`;
CREATE TABLE IF NOT EXISTS `adv__servicio_orden_detalle` (
  `id_servicio_orden_detalle` bigint NOT NULL AUTO_INCREMENT,
  `id_servicio_orden` int NOT NULL,
  `tipo_item` enum('Actividad','Material') NOT NULL,
  `id_servicio_actividad` int DEFAULT NULL,
  `id_producto` int DEFAULT NULL COMMENT 'Para materiales/insumos',
  `id_almacen_origen` int DEFAULT NULL COMMENT 'Almacén de donde salen los materiales',
  `descripcion_personalizada` varchar(255) DEFAULT NULL,
  `cantidad_planificada` decimal(10,2) NOT NULL DEFAULT '1.00',
  `cantidad_ejecutada` decimal(10,2) DEFAULT NULL,
  `precio_unitario` decimal(10,2) NOT NULL DEFAULT '0.00',
  `descuento_item` decimal(12,2) NOT NULL DEFAULT '0.00',
  `impuesto_item` decimal(12,2) NOT NULL DEFAULT '0.00',
  `subtotal` decimal(12,4) GENERATED ALWAYS AS ((`cantidad_planificada` * `precio_unitario`)) STORED,
  `total_item` decimal(12,4) GENERATED ALWAYS AS ((((`cantidad_planificada` * `precio_unitario`) - `descuento_item`) + `impuesto_item`)) STORED,
  `orden` int NOT NULL DEFAULT '0',
  `observaciones` text,
  `ejecutado_por` int DEFAULT NULL COMMENT 'ID del técnico/empleado que ejecutó',
  `fecha_ejecucion` datetime DEFAULT NULL,
  `id_movimiento_salida` int DEFAULT NULL COMMENT 'Movimiento de inventario generado',
  PRIMARY KEY (`id_servicio_orden_detalle`),
  KEY `idx_orden` (`id_servicio_orden`),
  KEY `idx_actividad` (`id_servicio_actividad`),
  KEY `idx_producto` (`id_producto`),
  KEY `idx_almacen` (`id_almacen_origen`),
  KEY `idx_movimiento_salida` (`id_movimiento_salida`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Disparadores `adv__servicio_orden_detalle`
--
DROP TRIGGER IF EXISTS `trg_servicio_orden_detalle_after_delete`;
DELIMITER $$
CREATE TRIGGER `trg_servicio_orden_detalle_after_delete` AFTER DELETE ON `adv__servicio_orden_detalle` FOR EACH ROW BEGIN
    UPDATE adv__servicio_orden so
    SET 
        so.subtotal = (
            SELECT COALESCE(SUM(subtotal), 0)
            FROM adv__servicio_orden_detalle
            WHERE id_servicio_orden = OLD.id_servicio_orden
        ),
        so.descuento_total = (
            SELECT COALESCE(SUM(descuento_item), 0)
            FROM adv__servicio_orden_detalle
            WHERE id_servicio_orden = OLD.id_servicio_orden
        ),
        so.impuesto_total = (
            SELECT COALESCE(SUM(impuesto_item), 0)
            FROM adv__servicio_orden_detalle
            WHERE id_servicio_orden = OLD.id_servicio_orden
        ),
        so.importe_total = (
            SELECT COALESCE(SUM(total_item), 0)
            FROM adv__servicio_orden_detalle
            WHERE id_servicio_orden = OLD.id_servicio_orden
        )
    WHERE so.id_servicio_orden = OLD.id_servicio_orden;
END
$$
DELIMITER ;
DROP TRIGGER IF EXISTS `trg_servicio_orden_detalle_after_insert`;
DELIMITER $$
CREATE TRIGGER `trg_servicio_orden_detalle_after_insert` AFTER INSERT ON `adv__servicio_orden_detalle` FOR EACH ROW BEGIN
    UPDATE adv__servicio_orden so
    SET 
        so.subtotal = (
            SELECT COALESCE(SUM(subtotal), 0)
            FROM adv__servicio_orden_detalle
            WHERE id_servicio_orden = NEW.id_servicio_orden
        ),
        so.descuento_total = (
            SELECT COALESCE(SUM(descuento_item), 0)
            FROM adv__servicio_orden_detalle
            WHERE id_servicio_orden = NEW.id_servicio_orden
        ),
        so.impuesto_total = (
            SELECT COALESCE(SUM(impuesto_item), 0)
            FROM adv__servicio_orden_detalle
            WHERE id_servicio_orden = NEW.id_servicio_orden
        ),
        so.importe_total = (
            SELECT COALESCE(SUM(total_item), 0)
            FROM adv__servicio_orden_detalle
            WHERE id_servicio_orden = NEW.id_servicio_orden
        )
    WHERE so.id_servicio_orden = NEW.id_servicio_orden;
END
$$
DELIMITER ;
DROP TRIGGER IF EXISTS `trg_servicio_orden_detalle_after_update`;
DELIMITER $$
CREATE TRIGGER `trg_servicio_orden_detalle_after_update` AFTER UPDATE ON `adv__servicio_orden_detalle` FOR EACH ROW BEGIN
    UPDATE adv__servicio_orden so
    SET 
        so.subtotal = (
            SELECT COALESCE(SUM(subtotal), 0)
            FROM adv__servicio_orden_detalle
            WHERE id_servicio_orden = NEW.id_servicio_orden
        ),
        so.descuento_total = (
            SELECT COALESCE(SUM(descuento_item), 0)
            FROM adv__servicio_orden_detalle
            WHERE id_servicio_orden = NEW.id_servicio_orden
        ),
        so.impuesto_total = (
            SELECT COALESCE(SUM(impuesto_item), 0)
            FROM adv__servicio_orden_detalle
            WHERE id_servicio_orden = NEW.id_servicio_orden
        ),
        so.importe_total = (
            SELECT COALESCE(SUM(total_item), 0)
            FROM adv__servicio_orden_detalle
            WHERE id_servicio_orden = NEW.id_servicio_orden
        )
    WHERE so.id_servicio_orden = NEW.id_servicio_orden;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__servicio_presupuesto`
--

DROP TABLE IF EXISTS `adv__servicio_presupuesto`;
CREATE TABLE IF NOT EXISTS `adv__servicio_presupuesto` (
  `id_servicio_presupuesto` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(50) NOT NULL,
  `id_cliente` int NOT NULL,
  `id_cuenta_usuario` int DEFAULT NULL,
  `id_servicio_tipo` int DEFAULT NULL,
  `fecha_emision` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_validez` datetime DEFAULT NULL,
  `fecha_aprobacion` datetime DEFAULT NULL,
  `fecha_rechazo` datetime DEFAULT NULL,
  `estado` enum('Borrador','Enviado','Aprobado','Rechazado','Expirado','Convertido') NOT NULL DEFAULT 'Borrador',
  `subtotal` decimal(12,2) NOT NULL DEFAULT '0.00',
  `descuento_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `impuesto_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `importe_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `observaciones` text,
  `terminos_condiciones` text,
  `id_venta_origen` int DEFAULT NULL COMMENT 'ID de venta si se convirtió desde pedido',
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_servicio_presupuesto`),
  UNIQUE KEY `uk_codigo_presupuesto` (`codigo`),
  KEY `idx_cliente` (`id_cliente`),
  KEY `idx_usuario` (`id_cuenta_usuario`),
  KEY `idx_servicio_tipo` (`id_servicio_tipo`),
  KEY `idx_estado` (`estado`),
  KEY `idx_fecha_emision` (`fecha_emision`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__servicio_presupuesto_detalle`
--

DROP TABLE IF EXISTS `adv__servicio_presupuesto_detalle`;
CREATE TABLE IF NOT EXISTS `adv__servicio_presupuesto_detalle` (
  `id_servicio_presupuesto_detalle` bigint NOT NULL AUTO_INCREMENT,
  `id_servicio_presupuesto` int NOT NULL,
  `tipo_item` enum('Actividad','Material') NOT NULL,
  `id_servicio_actividad` int DEFAULT NULL,
  `id_producto` int DEFAULT NULL COMMENT 'Para materiales/insumos',
  `descripcion_personalizada` varchar(255) DEFAULT NULL,
  `cantidad` decimal(10,2) NOT NULL DEFAULT '1.00',
  `precio_unitario` decimal(10,2) NOT NULL DEFAULT '0.00',
  `descuento_item` decimal(12,2) NOT NULL DEFAULT '0.00',
  `impuesto_item` decimal(12,2) NOT NULL DEFAULT '0.00',
  `subtotal` decimal(12,4) GENERATED ALWAYS AS ((`cantidad` * `precio_unitario`)) STORED,
  `total_item` decimal(12,4) GENERATED ALWAYS AS ((((`cantidad` * `precio_unitario`) - `descuento_item`) + `impuesto_item`)) STORED,
  `orden` int NOT NULL DEFAULT '0',
  `observaciones` text,
  PRIMARY KEY (`id_servicio_presupuesto_detalle`),
  KEY `idx_presupuesto` (`id_servicio_presupuesto`),
  KEY `idx_actividad` (`id_servicio_actividad`),
  KEY `idx_producto` (`id_producto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Disparadores `adv__servicio_presupuesto_detalle`
--
DROP TRIGGER IF EXISTS `trg_servicio_presupuesto_detalle_after_delete`;
DELIMITER $$
CREATE TRIGGER `trg_servicio_presupuesto_detalle_after_delete` AFTER DELETE ON `adv__servicio_presupuesto_detalle` FOR EACH ROW BEGIN
    UPDATE adv__servicio_presupuesto sp
    SET 
        sp.subtotal = (
            SELECT COALESCE(SUM(subtotal), 0)
            FROM adv__servicio_presupuesto_detalle
            WHERE id_servicio_presupuesto = OLD.id_servicio_presupuesto
        ),
        sp.descuento_total = (
            SELECT COALESCE(SUM(descuento_item), 0)
            FROM adv__servicio_presupuesto_detalle
            WHERE id_servicio_presupuesto = OLD.id_servicio_presupuesto
        ),
        sp.impuesto_total = (
            SELECT COALESCE(SUM(impuesto_item), 0)
            FROM adv__servicio_presupuesto_detalle
            WHERE id_servicio_presupuesto = OLD.id_servicio_presupuesto
        ),
        sp.importe_total = (
            SELECT COALESCE(SUM(total_item), 0)
            FROM adv__servicio_presupuesto_detalle
            WHERE id_servicio_presupuesto = OLD.id_servicio_presupuesto
        )
    WHERE sp.id_servicio_presupuesto = OLD.id_servicio_presupuesto;
END
$$
DELIMITER ;
DROP TRIGGER IF EXISTS `trg_servicio_presupuesto_detalle_after_insert`;
DELIMITER $$
CREATE TRIGGER `trg_servicio_presupuesto_detalle_after_insert` AFTER INSERT ON `adv__servicio_presupuesto_detalle` FOR EACH ROW BEGIN
    UPDATE adv__servicio_presupuesto sp
    SET 
        sp.subtotal = (
            SELECT COALESCE(SUM(subtotal), 0)
            FROM adv__servicio_presupuesto_detalle
            WHERE id_servicio_presupuesto = NEW.id_servicio_presupuesto
        ),
        sp.descuento_total = (
            SELECT COALESCE(SUM(descuento_item), 0)
            FROM adv__servicio_presupuesto_detalle
            WHERE id_servicio_presupuesto = NEW.id_servicio_presupuesto
        ),
        sp.impuesto_total = (
            SELECT COALESCE(SUM(impuesto_item), 0)
            FROM adv__servicio_presupuesto_detalle
            WHERE id_servicio_presupuesto = NEW.id_servicio_presupuesto
        ),
        sp.importe_total = (
            SELECT COALESCE(SUM(total_item), 0)
            FROM adv__servicio_presupuesto_detalle
            WHERE id_servicio_presupuesto = NEW.id_servicio_presupuesto
        )
    WHERE sp.id_servicio_presupuesto = NEW.id_servicio_presupuesto;
END
$$
DELIMITER ;
DROP TRIGGER IF EXISTS `trg_servicio_presupuesto_detalle_after_update`;
DELIMITER $$
CREATE TRIGGER `trg_servicio_presupuesto_detalle_after_update` AFTER UPDATE ON `adv__servicio_presupuesto_detalle` FOR EACH ROW BEGIN
    UPDATE adv__servicio_presupuesto sp
    SET 
        sp.subtotal = (
            SELECT COALESCE(SUM(subtotal), 0)
            FROM adv__servicio_presupuesto_detalle
            WHERE id_servicio_presupuesto = NEW.id_servicio_presupuesto
        ),
        sp.descuento_total = (
            SELECT COALESCE(SUM(descuento_item), 0)
            FROM adv__servicio_presupuesto_detalle
            WHERE id_servicio_presupuesto = NEW.id_servicio_presupuesto
        ),
        sp.impuesto_total = (
            SELECT COALESCE(SUM(impuesto_item), 0)
            FROM adv__servicio_presupuesto_detalle
            WHERE id_servicio_presupuesto = NEW.id_servicio_presupuesto
        ),
        sp.importe_total = (
            SELECT COALESCE(SUM(total_item), 0)
            FROM adv__servicio_presupuesto_detalle
            WHERE id_servicio_presupuesto = NEW.id_servicio_presupuesto
        )
    WHERE sp.id_servicio_presupuesto = NEW.id_servicio_presupuesto;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__servicio_tecnico_asignado`
--

DROP TABLE IF EXISTS `adv__servicio_tecnico_asignado`;
CREATE TABLE IF NOT EXISTS `adv__servicio_tecnico_asignado` (
  `id_asignacion` int NOT NULL AUTO_INCREMENT,
  `id_servicio_orden` int NOT NULL,
  `id_empleado` int NOT NULL,
  `id_cuenta_usuario_asigno` int NOT NULL,
  `rol` enum('Principal','Secundario','Supervisor') COLLATE utf8mb4_unicode_ci DEFAULT 'Secundario',
  `horas_trabajadas` decimal(6,2) DEFAULT '0.00',
  `fecha_asignacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_desasignacion` datetime DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_asignacion`),
  KEY `idx_orden` (`id_servicio_orden`),
  KEY `idx_empleado` (`id_empleado`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__servicio_tipo`
--

DROP TABLE IF EXISTS `adv__servicio_tipo`;
CREATE TABLE IF NOT EXISTS `adv__servicio_tipo` (
  `id_servicio_tipo` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `descripcion` text,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_servicio_tipo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__sesion_auditoria`
--

DROP TABLE IF EXISTS `adv__sesion_auditoria`;
CREATE TABLE IF NOT EXISTS `adv__sesion_auditoria` (
  `id_sesion_auditoria` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(150) NOT NULL,
  `id_almacen` int NOT NULL,
  `id_cuenta_usuario` int NOT NULL,
  `fecha_inicio` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_cierre` datetime DEFAULT NULL,
  `estado` enum('Abierta','En_Proceso','Cerrada','Anulada') NOT NULL DEFAULT 'Abierta',
  `observaciones` text,
  PRIMARY KEY (`id_sesion_auditoria`),
  KEY `idx_almacen` (`id_almacen`),
  KEY `idx_usuario` (`id_cuenta_usuario`),
  KEY `idx_estado` (`estado`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__solicitud_compra`
--

DROP TABLE IF EXISTS `adv__solicitud_compra`;
CREATE TABLE IF NOT EXISTS `adv__solicitud_compra` (
  `id_solicitud_compra` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(50) NOT NULL,
  `id_solicitante` int NOT NULL,
  `fecha_solicitud` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_requerida` date DEFAULT NULL,
  `observaciones` text,
  `estado` enum('Borrador','Pendiente_Aprobacion','Aprobada','Rechazada','Convertida','Cancelada') NOT NULL DEFAULT 'Borrador',
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_solicitud_compra`),
  UNIQUE KEY `uk_codigo_solicitud` (`codigo`),
  KEY `idx_solicitante` (`id_solicitante`),
  KEY `idx_estado` (`estado`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__tasa_cambio`
--

DROP TABLE IF EXISTS `adv__tasa_cambio`;
CREATE TABLE IF NOT EXISTS `adv__tasa_cambio` (
  `id_tasa_cambio` int NOT NULL AUTO_INCREMENT,
  `id_moneda_origen` int NOT NULL,
  `id_moneda_destino` int NOT NULL,
  `fecha` date NOT NULL,
  `tasa` decimal(18,6) NOT NULL,
  `fuente` varchar(100) DEFAULT NULL,
  `id_cuenta_usuario` int DEFAULT NULL,
  `fecha_registro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `aplica_efectivo` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id_tasa_cambio`),
  UNIQUE KEY `uk_tasa_fecha_monedas` (`id_moneda_origen`,`id_moneda_destino`,`fecha`),
  KEY `idx_tasa_fecha` (`fecha`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__tecnico_zona`
--

DROP TABLE IF EXISTS `adv__tecnico_zona`;
CREATE TABLE IF NOT EXISTS `adv__tecnico_zona` (
  `id_tecnico_zona` int NOT NULL AUTO_INCREMENT,
  `id_cuenta_usuario` int NOT NULL COMMENT 'Técnico responsable',
  `id_servicio_tipo` int DEFAULT NULL COMMENT 'Especialidad del técnico',
  `zona_nombre` varchar(100) NOT NULL,
  `zona_descripcion` text,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_tecnico_zona`),
  KEY `idx_tecnico` (`id_cuenta_usuario`),
  KEY `idx_servicio_tipo` (`id_servicio_tipo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__telefono_contacto`
--

DROP TABLE IF EXISTS `adv__telefono_contacto`;
CREATE TABLE IF NOT EXISTS `adv__telefono_contacto` (
  `id_telefono_contacto` int NOT NULL AUTO_INCREMENT,
  `prefijo` varchar(8) NOT NULL DEFAULT '',
  `numero` varchar(16) NOT NULL,
  `categoria` enum('Movil','Fijo','Fax','Trabajo','Otro') NOT NULL DEFAULT 'Otro',
  `id_persona` int NOT NULL,
  PRIMARY KEY (`id_telefono_contacto`),
  KEY `fk_telefono_persona_idx` (`id_persona`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__tipo_compra`
--

DROP TABLE IF EXISTS `adv__tipo_compra`;
CREATE TABLE IF NOT EXISTS `adv__tipo_compra` (
  `id_tipo_compra` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `descripcion` text,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_tipo_compra`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__tipo_movimiento`
--

DROP TABLE IF EXISTS `adv__tipo_movimiento`;
CREATE TABLE IF NOT EXISTS `adv__tipo_movimiento` (
  `id_tipo_movimiento` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  `efecto` enum('Carga','Descarga','Transferencia') NOT NULL,
  PRIMARY KEY (`id_tipo_movimiento`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__unidad_medida`
--

DROP TABLE IF EXISTS `adv__unidad_medida`;
CREATE TABLE IF NOT EXISTS `adv__unidad_medida` (
  `id_unidad_medida` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  `abreviatura` varchar(10) NOT NULL,
  `descripcion` text,
  PRIMARY KEY (`id_unidad_medida`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__venta`
--

DROP TABLE IF EXISTS `adv__venta`;
CREATE TABLE IF NOT EXISTS `adv__venta` (
  `id_venta` int NOT NULL AUTO_INCREMENT,
  `id_pedido` int DEFAULT 0,
  `id_empleado` int NOT NULL DEFAULT 0,
  `id_cliente` int DEFAULT 0,
  `id_cuenta_usuario` int DEFAULT NULL,
  `id_almacen_origen` int DEFAULT NULL,
  `numero_factura_ticket` varchar(50) DEFAULT NULL,
  `fecha` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `subtotal` decimal(12,2) NOT NULL DEFAULT '0.00',
  `descuento_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `impuesto_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `importe_total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `metodo_pago_principal` varchar(50) DEFAULT NULL,
  `estado_venta` enum('Pendiente','Completada','Anulada','Entregada') NOT NULL DEFAULT 'Pendiente',
  `observaciones` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  `id_moneda` int NOT NULL DEFAULT '1',
  `tasa_cambio_aplicada` decimal(18,6) NOT NULL DEFAULT '1.000000',
  PRIMARY KEY (`id_venta`),
  UNIQUE KEY `numero_factura_ticket` (`numero_factura_ticket`),
  UNIQUE KEY `uk_numero_factura_ticket` (`numero_factura_ticket`),
  KEY `idx_punto_venta` (`id_almacen_origen`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `adv__version_esquema`
--

DROP TABLE IF EXISTS `adv__version_esquema`;
CREATE TABLE IF NOT EXISTS `adv__version_esquema` (
  `id_version` int NOT NULL AUTO_INCREMENT,
  `version` varchar(20) NOT NULL,
  `descripcion` text,
  `fecha_aplicada` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_version`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_auditoria_inventario`
-- (Véase abajo para la vista actual)
--
DROP VIEW IF EXISTS `v_auditoria_inventario`;
CREATE TABLE IF NOT EXISTS `v_auditoria_inventario` (
`cantidad_anterior` decimal(10,2)
,`cantidad_nueva` decimal(10,2)
,`codigo_producto` varchar(100)
,`costo_prom_anterior` decimal(12,4)
,`costo_prom_nuevo` decimal(12,4)
,`diferencia_cantidad` decimal(10,2)
,`diferencia_valor` decimal(12,4)
,`fecha_registro` datetime
,`id_almacen` int
,`id_auditoria` bigint
,`id_movimiento_ref` int
,`id_producto` int
,`observacion` text
,`origen` enum('Movimiento','AjusteManual','Carga_Inicial','Sistema','Trigger')
,`tipo_movimiento` varchar(50)
,`usuario` varchar(100)
,`valor_total_anterior` decimal(12,4)
,`valor_total_nuevo` decimal(12,4)
);

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_diferencias_pendientes`
-- (Véase abajo para la vista actual)
--
DROP VIEW IF EXISTS `v_diferencias_pendientes`;
CREATE TABLE IF NOT EXISTS `v_diferencias_pendientes` (
`almacen` varchar(100)
,`cantidad_contada` decimal(10,2)
,`cantidad_sistema` decimal(10,2)
,`codigo_producto` varchar(100)
,`contado_por` varchar(100)
,`diferencia` decimal(10,2)
,`fecha_conteo` datetime
,`id_detalle_auditoria` bigint
,`observacion` text
,`producto` varchar(255)
,`sesion` varchar(150)
,`valor_diferencia` decimal(12,4)
);

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_kardex_valorizado`
-- (Véase abajo para la vista actual)
--
DROP VIEW IF EXISTS `v_kardex_valorizado`;
CREATE TABLE IF NOT EXISTS `v_kardex_valorizado` (
`almacen` varchar(100)
,`cantidad_anterior` decimal(10,2)
,`cantidad_nueva` decimal(10,2)
,`costo_promedio` decimal(12,4)
,`fecha_registro` datetime
,`movimiento` decimal(10,2)
,`producto` varchar(255)
,`saldo_valorizado` decimal(12,4)
,`tipo` varchar(7)
);

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_resumen_sesion_auditoria`
-- (Véase abajo para la vista actual)
--
DROP VIEW IF EXISTS `v_resumen_sesion_auditoria`;
CREATE TABLE IF NOT EXISTS `v_resumen_sesion_auditoria` (
`almacen` varchar(100)
,`estado` enum('Abierta','En_Proceso','Cerrada','Anulada')
,`fecha_cierre` datetime
,`fecha_inicio` datetime
,`id_sesion_auditoria` int
,`impacto_economico_total` decimal(34,4)
,`productos_exactos` decimal(23,0)
,`productos_faltante` decimal(23,0)
,`productos_sin_contar` decimal(23,0)
,`productos_sobrante` decimal(23,0)
,`responsable` varchar(100)
,`sesion` varchar(150)
,`total_productos` bigint
);

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_servicio_actividades_completas`
-- (Véase abajo para la vista actual)
--
DROP VIEW IF EXISTS `v_servicio_actividades_completas`;
CREATE TABLE IF NOT EXISTS `v_servicio_actividades_completas` (
`activo` tinyint(1)
,`codigo` varchar(50)
,`descripcion` text
,`id_servicio_actividad` int
,`id_servicio_tipo` int
,`nombre` varchar(150)
,`precio_base` decimal(10,2)
,`requiere_materiales` tinyint(1)
,`tiempo_estimado_minutos` int
,`tipo_servicio_nombre` varchar(100)
,`unidad_medida` varchar(50)
);

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_servicio_orden_resumen`
-- (Véase abajo para la vista actual)
--
DROP VIEW IF EXISTS `v_servicio_orden_resumen`;
CREATE TABLE IF NOT EXISTS `v_servicio_orden_resumen` (
`cantidad_actividades` decimal(23,0)
,`cantidad_items` bigint
,`cantidad_materiales` decimal(23,0)
,`cliente_nombre` varchar(255)
,`codigo` varchar(50)
,`descuento_total` decimal(12,2)
,`estado` enum('Pendiente','Programada','EnProgreso','EnPausa','Completada','Facturada','Anulada')
,`fecha_creacion` datetime
,`fecha_fin_programada` datetime
,`fecha_inicio_programada` datetime
,`id_cliente` int
,`id_servicio_orden` int
,`id_servicio_tipo` int
,`importe_total` decimal(12,2)
,`impuesto_total` decimal(12,2)
,`prioridad` enum('Baja','Media','Alta','Urgente')
,`servicio_tipo_nombre` varchar(100)
,`subtotal` decimal(12,2)
);

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_servicio_presupuesto_resumen`
-- (Véase abajo para la vista actual)
--
DROP VIEW IF EXISTS `v_servicio_presupuesto_resumen`;
CREATE TABLE IF NOT EXISTS `v_servicio_presupuesto_resumen` (
`cantidad_items` bigint
,`cliente_nombre` varchar(255)
,`codigo` varchar(50)
,`descuento_total` decimal(12,2)
,`dias_restantes` int
,`estado` enum('Borrador','Enviado','Aprobado','Rechazado','Expirado','Convertido')
,`estado_actualizado` varchar(10)
,`fecha_emision` datetime
,`fecha_validez` datetime
,`id_cliente` int
,`id_servicio_presupuesto` int
,`id_servicio_tipo` int
,`importe_total` decimal(12,2)
,`impuesto_total` decimal(12,2)
,`servicio_tipo_nombre` varchar(100)
,`subtotal` decimal(12,2)
);

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_usuario_login`
-- (Véase abajo para la vista actual)
--
DROP VIEW IF EXISTS `v_usuario_login`;
CREATE TABLE IF NOT EXISTS `v_usuario_login` (
`administrador` tinyint(1)
,`email` varchar(255)
,`estado` tinyint(1)
,`id_cuenta_usuario` int
,`id_rol` int
,`nombre` varchar(100)
,`password_hash` varchar(255)
,`password_salt` varchar(255)
,`rol_nombre` varchar(50)
);

-- --------------------------------------------------------

--
-- Estructura para la vista `v_auditoria_inventario`
--
DROP TABLE IF EXISTS `v_auditoria_inventario`;

DROP VIEW IF EXISTS `v_auditoria_inventario`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_auditoria_inventario`  AS SELECT `a`.`id_auditoria` AS `id_auditoria`, `a`.`fecha_registro` AS `fecha_registro`, `p`.`id_producto` AS `id_producto`, `p`.`codigo` AS `codigo_producto`, `al`.`id_almacen` AS `id_almacen`, `a`.`cantidad_anterior` AS `cantidad_anterior`, `a`.`cantidad_nueva` AS `cantidad_nueva`, `a`.`diferencia_cantidad` AS `diferencia_cantidad`, `a`.`costo_prom_anterior` AS `costo_prom_anterior`, `a`.`costo_prom_nuevo` AS `costo_prom_nuevo`, `a`.`valor_total_anterior` AS `valor_total_anterior`, `a`.`valor_total_nuevo` AS `valor_total_nuevo`, `a`.`diferencia_valor` AS `diferencia_valor`, `a`.`origen` AS `origen`, `cu`.`nombre` AS `usuario`, `tm`.`nombre` AS `tipo_movimiento`, `a`.`id_movimiento_ref` AS `id_movimiento_ref`, `a`.`observacion` AS `observacion` FROM ((((((`adv__auditoria_inventario` `a` left join `adv__inventario` `inv` on((`a`.`id_inventario` = `inv`.`id_inventario`))) left join `adv__producto` `p` on((`a`.`id_producto` = `p`.`id_producto`))) left join `adv__almacen` `al` on((`a`.`id_almacen` = `al`.`id_almacen`))) left join `adv__cuenta_usuario` `cu` on((`a`.`id_cuenta_usuario` = `cu`.`id_cuenta_usuario`))) left join `adv__movimiento` `m` on((`a`.`id_movimiento_ref` = `m`.`id_movimiento`))) left join `adv__tipo_movimiento` `tm` on((`m`.`id_tipo_movimiento` = `tm`.`id_tipo_movimiento`))) ;

-- --------------------------------------------------------

--
-- Estructura para la vista `v_diferencias_pendientes`
--
DROP TABLE IF EXISTS `v_diferencias_pendientes`;

DROP VIEW IF EXISTS `v_diferencias_pendientes`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_diferencias_pendientes`  AS SELECT `d`.`id_detalle_auditoria` AS `id_detalle_auditoria`, `s`.`nombre` AS `sesion`, `p`.`nombre` AS `producto`, `p`.`codigo` AS `codigo_producto`, `al`.`nombre` AS `almacen`, `d`.`cantidad_sistema` AS `cantidad_sistema`, `d`.`cantidad_contada` AS `cantidad_contada`, `d`.`diferencia` AS `diferencia`, `d`.`valor_diferencia` AS `valor_diferencia`, `cu`.`nombre` AS `contado_por`, `d`.`fecha_conteo` AS `fecha_conteo`, `d`.`observacion` AS `observacion` FROM ((((`adv__detalle_auditoria` `d` join `adv__sesion_auditoria` `s` on((`d`.`id_sesion_auditoria` = `s`.`id_sesion_auditoria`))) join `adv__producto` `p` on((`d`.`id_producto` = `p`.`id_producto`))) join `adv__almacen` `al` on((`d`.`id_almacen` = `al`.`id_almacen`))) left join `adv__cuenta_usuario` `cu` on((`d`.`contado_por` = `cu`.`id_cuenta_usuario`))) WHERE ((`d`.`diferencia` <> 0) AND (`d`.`diferencia` is not null) AND (`d`.`ajuste_aplicado` = 0)) ;

-- --------------------------------------------------------

--
-- Estructura para la vista `v_kardex_valorizado`
--
DROP TABLE IF EXISTS `v_kardex_valorizado`;

DROP VIEW IF EXISTS `v_kardex_valorizado`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_kardex_valorizado`  AS SELECT `p`.`nombre` AS `producto`, `a`.`nombre` AS `almacen`, `aui`.`fecha_registro` AS `fecha_registro`, `aui`.`cantidad_anterior` AS `cantidad_anterior`, `aui`.`cantidad_nueva` AS `cantidad_nueva`, `aui`.`diferencia_cantidad` AS `movimiento`, (case when (`aui`.`diferencia_cantidad` > 0) then 'Entrada' else 'Salida' end) AS `tipo`, `aui`.`costo_prom_nuevo` AS `costo_promedio`, `aui`.`valor_total_nuevo` AS `saldo_valorizado` FROM ((`v_auditoria_inventario` `aui` join `adv__producto` `p` on((`aui`.`id_producto` = `p`.`id_producto`))) join `adv__almacen` `a` on((`aui`.`id_almacen` = `a`.`id_almacen`))) ORDER BY `p`.`id_producto` ASC, `aui`.`fecha_registro` ASC ;

-- --------------------------------------------------------

--
-- Estructura para la vista `v_resumen_sesion_auditoria`
--
DROP TABLE IF EXISTS `v_resumen_sesion_auditoria`;

DROP VIEW IF EXISTS `v_resumen_sesion_auditoria`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_resumen_sesion_auditoria`  AS SELECT `s`.`id_sesion_auditoria` AS `id_sesion_auditoria`, `s`.`nombre` AS `sesion`, `al`.`nombre` AS `almacen`, `cu`.`nombre` AS `responsable`, `s`.`fecha_inicio` AS `fecha_inicio`, `s`.`fecha_cierre` AS `fecha_cierre`, `s`.`estado` AS `estado`, count(`d`.`id_detalle_auditoria`) AS `total_productos`, sum((case when (`d`.`diferencia` > 0) then 1 else 0 end)) AS `productos_sobrante`, sum((case when (`d`.`diferencia` < 0) then 1 else 0 end)) AS `productos_faltante`, sum((case when (`d`.`diferencia` = 0) then 1 else 0 end)) AS `productos_exactos`, sum((case when (`d`.`diferencia` is null) then 1 else 0 end)) AS `productos_sin_contar`, coalesce(sum(`d`.`valor_diferencia`),0) AS `impacto_economico_total` FROM (((`adv__sesion_auditoria` `s` left join `adv__almacen` `al` on((`s`.`id_almacen` = `al`.`id_almacen`))) left join `adv__cuenta_usuario` `cu` on((`s`.`id_cuenta_usuario` = `cu`.`id_cuenta_usuario`))) left join `adv__detalle_auditoria` `d` on((`s`.`id_sesion_auditoria` = `d`.`id_sesion_auditoria`))) GROUP BY `s`.`id_sesion_auditoria`, `s`.`nombre`, `al`.`nombre`, `cu`.`nombre`, `s`.`fecha_inicio`, `s`.`fecha_cierre`, `s`.`estado` ;

-- --------------------------------------------------------

--
-- Estructura para la vista `v_servicio_actividades_completas`
--
DROP TABLE IF EXISTS `v_servicio_actividades_completas`;

DROP VIEW IF EXISTS `v_servicio_actividades_completas`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_servicio_actividades_completas`  AS SELECT `sa`.`id_servicio_actividad` AS `id_servicio_actividad`, `sa`.`codigo` AS `codigo`, `sa`.`nombre` AS `nombre`, `sa`.`descripcion` AS `descripcion`, `sa`.`precio_base` AS `precio_base`, `sa`.`unidad_medida` AS `unidad_medida`, `sa`.`tiempo_estimado_minutos` AS `tiempo_estimado_minutos`, `sa`.`requiere_materiales` AS `requiere_materiales`, `sa`.`activo` AS `activo`, `st`.`id_servicio_tipo` AS `id_servicio_tipo`, `st`.`nombre` AS `tipo_servicio_nombre` FROM (`adv__servicio_actividad` `sa` left join `adv__servicio_tipo` `st` on((`sa`.`id_servicio_tipo` = `st`.`id_servicio_tipo`))) ;

-- --------------------------------------------------------

--
-- Estructura para la vista `v_servicio_orden_resumen`
--
DROP TABLE IF EXISTS `v_servicio_orden_resumen`;

DROP VIEW IF EXISTS `v_servicio_orden_resumen`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_servicio_orden_resumen`  AS SELECT `so`.`id_servicio_orden` AS `id_servicio_orden`, `so`.`codigo` AS `codigo`, `so`.`id_cliente` AS `id_cliente`, `p`.`nombre_completo` AS `cliente_nombre`, `so`.`id_servicio_tipo` AS `id_servicio_tipo`, `st`.`nombre` AS `servicio_tipo_nombre`, `so`.`estado` AS `estado`, `so`.`prioridad` AS `prioridad`, `so`.`fecha_creacion` AS `fecha_creacion`, `so`.`fecha_inicio_programada` AS `fecha_inicio_programada`, `so`.`fecha_fin_programada` AS `fecha_fin_programada`, `so`.`subtotal` AS `subtotal`, `so`.`descuento_total` AS `descuento_total`, `so`.`impuesto_total` AS `impuesto_total`, `so`.`importe_total` AS `importe_total`, count(`sod`.`id_servicio_orden_detalle`) AS `cantidad_items`, sum((case when (`sod`.`tipo_item` = 'Actividad') then 1 else 0 end)) AS `cantidad_actividades`, sum((case when (`sod`.`tipo_item` = 'Material') then 1 else 0 end)) AS `cantidad_materiales` FROM ((((`adv__servicio_orden` `so` join `adv__cliente` `c` on((`so`.`id_cliente` = `c`.`id_cliente`))) join `adv__persona` `p` on((`c`.`id_persona` = `p`.`id_persona`))) left join `adv__servicio_tipo` `st` on((`so`.`id_servicio_tipo` = `st`.`id_servicio_tipo`))) left join `adv__servicio_orden_detalle` `sod` on((`so`.`id_servicio_orden` = `sod`.`id_servicio_orden`))) WHERE (`so`.`activo` = 1) GROUP BY `so`.`id_servicio_orden` ;

-- --------------------------------------------------------

--
-- Estructura para la vista `v_servicio_presupuesto_resumen`
--
DROP TABLE IF EXISTS `v_servicio_presupuesto_resumen`;

DROP VIEW IF EXISTS `v_servicio_presupuesto_resumen`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_servicio_presupuesto_resumen`  AS SELECT `sp`.`id_servicio_presupuesto` AS `id_servicio_presupuesto`, `sp`.`codigo` AS `codigo`, `sp`.`id_cliente` AS `id_cliente`, `p`.`nombre_completo` AS `cliente_nombre`, `sp`.`id_servicio_tipo` AS `id_servicio_tipo`, `st`.`nombre` AS `servicio_tipo_nombre`, `sp`.`estado` AS `estado`, `sp`.`fecha_emision` AS `fecha_emision`, `sp`.`fecha_validez` AS `fecha_validez`, (case when ((`sp`.`fecha_validez` < now()) and (`sp`.`estado` = 'Enviado')) then 'Expirado' else `sp`.`estado` end) AS `estado_actualizado`, `sp`.`subtotal` AS `subtotal`, `sp`.`descuento_total` AS `descuento_total`, `sp`.`impuesto_total` AS `impuesto_total`, `sp`.`importe_total` AS `importe_total`, count(`spd`.`id_servicio_presupuesto_detalle`) AS `cantidad_items`, greatest(0,(to_days(coalesce(`sp`.`fecha_validez`,(`sp`.`fecha_emision` + interval 7 day))) - to_days(curdate()))) AS `dias_restantes` FROM ((((`adv__servicio_presupuesto` `sp` join `adv__cliente` `c` on((`sp`.`id_cliente` = `c`.`id_cliente`))) join `adv__persona` `p` on((`c`.`id_persona` = `p`.`id_persona`))) left join `adv__servicio_tipo` `st` on((`sp`.`id_servicio_tipo` = `st`.`id_servicio_tipo`))) left join `adv__servicio_presupuesto_detalle` `spd` on((`sp`.`id_servicio_presupuesto` = `spd`.`id_servicio_presupuesto`))) WHERE (`sp`.`activo` = 1) GROUP BY `sp`.`id_servicio_presupuesto` ;

-- --------------------------------------------------------

--
-- Estructura para la vista `v_usuario_login`
--
DROP TABLE IF EXISTS `v_usuario_login`;

DROP VIEW IF EXISTS `v_usuario_login`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_usuario_login`  AS SELECT `u`.`id_cuenta_usuario` AS `id_cuenta_usuario`, `u`.`nombre` AS `nombre`, `u`.`password_hash` AS `password_hash`, `u`.`password_salt` AS `password_salt`, `u`.`email` AS `email`, `u`.`administrador` AS `administrador`, `u`.`estado` AS `estado`, `r`.`id_rol` AS `id_rol`, `r`.`nombre` AS `rol_nombre` FROM (`adv__cuenta_usuario` `u` join `adv__rol` `r` on((`u`.`id_rol` = `r`.`id_rol`))) WHERE ((`u`.`estado` = 1) AND (`r`.`activo` = 1)) ;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
