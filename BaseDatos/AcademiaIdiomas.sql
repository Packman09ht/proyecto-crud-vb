USE master;
GO

IF DB_ID(N'AcademiaIdiomas') IS NOT NULL
BEGIN
    ALTER DATABASE AcademiaIdiomas SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE AcademiaIdiomas;
END
GO

CREATE DATABASE AcademiaIdiomas;
GO

USE AcademiaIdiomas;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE TABLE rol (
    id_rol INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(30) NOT NULL UNIQUE
);

CREATE TABLE usuario (
    id_usuario INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(40) NOT NULL UNIQUE,
    contrasena_hash VARCHAR(64) NOT NULL,
    id_rol INT NOT NULL REFERENCES rol(id_rol),
    activo BIT NOT NULL DEFAULT 1
);

CREATE TABLE persona (
    id_persona INT IDENTITY(1,1) PRIMARY KEY,
    cedula VARCHAR(11) NULL,
    nombres NVARCHAR(60) NOT NULL,
    apellidos NVARCHAR(60) NOT NULL,
    fecha_nacimiento DATE NOT NULL,
    telefono VARCHAR(15) NULL,
    correo NVARCHAR(80) NULL,
    direccion NVARCHAR(150) NULL,
    id_usuario INT NULL REFERENCES usuario(id_usuario)
);
CREATE UNIQUE INDEX ux_persona_cedula ON persona(cedula) WHERE cedula IS NOT NULL;
CREATE UNIQUE INDEX ux_persona_correo ON persona(correo) WHERE correo IS NOT NULL;
CREATE UNIQUE INDEX ux_persona_usuario ON persona(id_usuario) WHERE id_usuario IS NOT NULL;

CREATE TABLE estudiante (
    id_estudiante INT IDENTITY(1,1) PRIMARY KEY,
    id_persona INT NOT NULL UNIQUE REFERENCES persona(id_persona),
    fecha_registro DATE NOT NULL,
    estado VARCHAR(10) NOT NULL DEFAULT 'activo' CHECK (estado IN ('activo', 'inactivo', 'retirado'))
);

CREATE TABLE docente (
    id_docente INT IDENTITY(1,1) PRIMARY KEY,
    id_persona INT NOT NULL UNIQUE REFERENCES persona(id_persona),
    especialidad NVARCHAR(60) NULL,
    fecha_contratacion DATE NOT NULL
);

CREATE TABLE idioma (
    id_idioma INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(40) NOT NULL UNIQUE
);

CREATE TABLE nivel (
    id_nivel INT IDENTITY(1,1) PRIMARY KEY,
    codigo VARCHAR(5) NOT NULL UNIQUE,
    descripcion NVARCHAR(60) NULL
);

CREATE TABLE curso (
    id_curso INT IDENTITY(1,1) PRIMARY KEY,
    id_idioma INT NOT NULL REFERENCES idioma(id_idioma),
    id_nivel INT NOT NULL REFERENCES nivel(id_nivel),
    nombre NVARCHAR(80) NOT NULL,
    duracion_horas INT NOT NULL CHECK (duracion_horas > 0),
    precio DECIMAL(10,2) NOT NULL CHECK (precio >= 0)
);

CREATE TABLE aula (
    id_aula INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(30) NOT NULL UNIQUE,
    capacidad INT NOT NULL CHECK (capacidad > 0),
    ubicacion NVARCHAR(60) NULL,
    equipamiento NVARCHAR(100) NULL,
    activa BIT NOT NULL DEFAULT 1,
    observaciones NVARCHAR(150) NULL
);

CREATE TABLE grupo (
    id_grupo INT IDENTITY(1,1) PRIMARY KEY,
    id_curso INT NOT NULL REFERENCES curso(id_curso),
    id_docente INT NOT NULL REFERENCES docente(id_docente),
    id_aula INT NOT NULL REFERENCES aula(id_aula),
    cupo_maximo INT NOT NULL CHECK (cupo_maximo > 0),
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    estado VARCHAR(10) NOT NULL DEFAULT 'abierto' CHECK (estado IN ('abierto', 'en curso', 'cerrado')),
    CONSTRAINT ck_grupo_fechas CHECK (fecha_fin > fecha_inicio)
);

CREATE TABLE horario (
    id_horario INT IDENTITY(1,1) PRIMARY KEY,
    id_grupo INT NOT NULL REFERENCES grupo(id_grupo),
    dia_semana NVARCHAR(10) NOT NULL CHECK (dia_semana IN (N'Lunes', N'Martes', N'Miércoles', N'Jueves', N'Viernes', N'Sábado')),
    hora_inicio TIME(0) NOT NULL,
    hora_fin TIME(0) NOT NULL,
    CONSTRAINT ck_horario_horas CHECK (hora_fin > hora_inicio)
);

CREATE TABLE inscripcion (
    id_inscripcion INT IDENTITY(1,1) PRIMARY KEY,
    id_estudiante INT NOT NULL REFERENCES estudiante(id_estudiante),
    id_grupo INT NOT NULL REFERENCES grupo(id_grupo),
    fecha_inscripcion DATE NOT NULL,
    estado VARCHAR(10) NOT NULL DEFAULT 'activa' CHECK (estado IN ('activa', 'retirada', 'finalizada')),
    CONSTRAINT uq_inscripcion UNIQUE (id_estudiante, id_grupo)
);

CREATE TABLE sesion (
    id_sesion INT IDENTITY(1,1) PRIMARY KEY,
    id_grupo INT NOT NULL REFERENCES grupo(id_grupo),
    fecha DATE NOT NULL,
    tema NVARCHAR(100) NULL
);

CREATE TABLE asistencia (
    id_asistencia INT IDENTITY(1,1) PRIMARY KEY,
    id_inscripcion INT NOT NULL REFERENCES inscripcion(id_inscripcion),
    id_sesion INT NOT NULL REFERENCES sesion(id_sesion),
    presente BIT NOT NULL,
    CONSTRAINT uq_asistencia UNIQUE (id_inscripcion, id_sesion)
);

CREATE TABLE evaluacion (
    id_evaluacion INT IDENTITY(1,1) PRIMARY KEY,
    id_grupo INT NOT NULL REFERENCES grupo(id_grupo),
    nombre NVARCHAR(50) NOT NULL,
    ponderacion DECIMAL(5,2) NOT NULL CHECK (ponderacion BETWEEN 0 AND 100)
);

CREATE TABLE calificacion (
    id_calificacion INT IDENTITY(1,1) PRIMARY KEY,
    id_inscripcion INT NOT NULL REFERENCES inscripcion(id_inscripcion),
    id_evaluacion INT NOT NULL REFERENCES evaluacion(id_evaluacion),
    nota DECIMAL(5,2) NOT NULL CHECK (nota BETWEEN 0 AND 100),
    CONSTRAINT uq_calificacion UNIQUE (id_inscripcion, id_evaluacion)
);

CREATE TABLE metodo_pago (
    id_metodo INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(30) NOT NULL UNIQUE
);

CREATE TABLE pago (
    id_pago INT IDENTITY(1,1) PRIMARY KEY,
    id_inscripcion INT NOT NULL REFERENCES inscripcion(id_inscripcion),
    id_metodo INT NOT NULL REFERENCES metodo_pago(id_metodo),
    fecha_pago DATE NOT NULL,
    monto DECIMAL(10,2) NOT NULL CHECK (monto > 0),
    concepto NVARCHAR(50) NOT NULL,
    num_recibo VARCHAR(20) NOT NULL UNIQUE
);
GO

INSERT INTO rol(nombre) VALUES (N'Administrador'), (N'Recepción'), (N'Docente'), (N'Estudiante');

INSERT INTO usuario(username, contrasena_hash, id_rol) VALUES
    (N'admin', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '1234'), 2), 1),
    (N'recepcion', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '1234'), 2), 2),
    (N'docente', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '1234'), 2), 3),
    (N'estudiante', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '1234'), 2), 4),
    (N'juan', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '1234'), 2), 4),
    (N'ana', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '1234'), 2), 4),
    (N'marc', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '1234'), 2), 3);

INSERT INTO persona(cedula, nombres, apellidos, fecha_nacimiento, telefono, correo, direccion, id_usuario) VALUES
    ('00100000001', N'Carla', N'Rodríguez', '1985-03-11', '809-555-0101', N'carla@academia.do', N'Calle 1, Barahona', 1),
    ('00100000002', N'Pedro', N'Díaz', '1990-06-21', '809-555-0102', N'pedro@academia.do', N'Calle 2, Barahona', 2),
    ('00100000003', N'Laura', N'Peña', '1988-09-15', '809-555-0103', N'laura@academia.do', N'Calle 3, Barahona', 3),
    ('00100000004', N'María', N'Pérez', '2001-01-30', '809-555-0104', N'maria@academia.do', N'Calle 4, Neyba', 4),
    ('00100000005', N'Juan', N'Rodríguez', '2002-04-12', '809-555-0105', N'juan@academia.do', N'Calle 5, Neyba', 5),
    ('00100000006', N'Ana', N'Martínez', '2000-11-02', '809-555-0106', N'ana@academia.do', N'Calle 6, Barahona', 6),
    ('00100000007', N'Marc', N'Dupont', '1984-07-07', '809-555-0107', N'marc@academia.do', N'Calle 7, Barahona', 7);

INSERT INTO estudiante(id_persona, fecha_registro) VALUES (4, '2026-09-01'), (5, '2026-09-01'), (6, '2026-09-01');
INSERT INTO docente(id_persona, especialidad, fecha_contratacion) VALUES (3, N'Inglés', '2025-02-01'), (7, N'Francés', '2025-02-01');
INSERT INTO idioma(nombre) VALUES (N'Inglés'), (N'Francés');
INSERT INTO nivel(codigo, descripcion) VALUES ('A2', N'Básico'), ('B1', N'Intermedio');
INSERT INTO curso(id_idioma, id_nivel, nombre, duracion_horas, precio) VALUES
    (1, 1, N'Inglés A2', 60, 4000), (1, 2, N'Inglés B1', 60, 4500), (2, 1, N'Francés A2', 60, 4200);
INSERT INTO aula(nombre, capacidad, ubicacion, equipamiento, observaciones) VALUES
    (N'Aula 1', 15, N'Primer piso', N'Pizarra y proyector', NULL),
    (N'Aula 2', 12, N'Primer piso', N'Pizarra y bocinas', NULL);
INSERT INTO grupo(id_curso, id_docente, id_aula, cupo_maximo, fecha_inicio, fecha_fin, estado) VALUES
    (2, 1, 1, 15, '2026-09-01', '2026-12-18', 'en curso'),
    (3, 2, 2, 12, '2026-09-01', '2026-12-18', 'en curso'),
    (1, 1, 2, 10, '2026-09-01', '2026-12-18', 'en curso');
INSERT INTO horario(id_grupo, dia_semana, hora_inicio, hora_fin) VALUES
    (1, N'Lunes', '18:00', '20:00'), (1, N'Miércoles', '18:00', '20:00'),
    (2, N'Martes', '19:30', '21:30'), (2, N'Jueves', '19:30', '21:30'),
    (3, N'Sábado', '08:00', '11:00');
INSERT INTO inscripcion(id_estudiante, id_grupo, fecha_inscripcion) VALUES (1, 1, '2026-09-01'), (2, 1, '2026-09-01'), (3, 1, '2026-09-01'), (1, 2, '2026-09-01');
INSERT INTO sesion(id_grupo, fecha, tema) VALUES (1, '2026-10-26', N'Presentaciones');
INSERT INTO asistencia(id_inscripcion, id_sesion, presente) VALUES (1, 1, 1), (2, 1, 1), (3, 1, 0);
INSERT INTO evaluacion(id_grupo, nombre, ponderacion) VALUES
    (1, N'Parcial', 30), (1, N'Oral', 30), (1, N'Final', 40),
    (2, N'Parcial', 30), (2, N'Oral', 30), (2, N'Final', 40),
    (3, N'Parcial', 30), (3, N'Oral', 30), (3, N'Final', 40);
INSERT INTO calificacion(id_inscripcion, id_evaluacion, nota) VALUES
    (1, 1, 90), (1, 2, 85), (1, 3, 92), (2, 1, 75), (2, 2, 80), (2, 3, 70), (3, 1, 60), (3, 2, 70), (3, 3, 65);
INSERT INTO metodo_pago(nombre) VALUES (N'Efectivo'), (N'Transferencia'), (N'Tarjeta');
INSERT INTO pago(id_inscripcion, id_metodo, fecha_pago, monto, concepto, num_recibo) VALUES
    (1, 1, '2026-09-02', 4500, N'Matrícula', 'REC-0001'),
    (2, 1, '2026-09-15', 2000, N'Matrícula', 'REC-0002'),
    (4, 1, '2026-10-05', 1000, N'Matrícula', 'REC-0003');
GO
