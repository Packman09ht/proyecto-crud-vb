# Academia de Idiomas - Proyecto integrador (VB.NET + SQL Server)

Aplicación de escritorio en Visual Studio con VB.NET y SQL Server para gestionar una academia de idiomas.

## Requisitos

- Windows con Visual Studio 2019 o 2022 y la carga de trabajo "Desarrollo de escritorio con .NET".
- SQL Server LocalDB (viene con Visual Studio) o SQL Server Express.

## Puesta en marcha

1. Crear la base de datos. Elige una opción:
   - Ejecutar `BaseDatos\instalar.bat` (usa LocalDB), o
   - En Visual Studio abrir Ver > Explorador de objetos de SQL Server, conectarse a `(localdb)\MSSQLLocalDB`, abrir una consulta nueva, pegar el contenido de `BaseDatos\AcademiaIdiomas.sql` y ejecutarlo.
2. Abrir `AcademiaIdiomas.sln`, dejar `Presentacion` como proyecto de inicio y pulsar F5.
3. Si usas SQL Server Express, cambiar `Server=(localdb)\MSSQLLocalDB` por `Server=.\SQLEXPRESS` en `Presentacion\App.config`.

Usuarios de prueba (clave 1234): `admin`, `recepcion`, `docente`, `estudiante`.

## Arquitectura en capas

| Capa | Proyecto | Contenido |
| --- | --- | --- |
| Entidades | `Entidades` | Clases que representan las tablas |
| Acceso a datos | `AccesoDatos` | Operaciones CRUD con SQL Server (`System.Data.SqlClient`) |
| Lógica de negocio | `LogicaNegocio` | Validaciones y reglas antes de guardar o consultar |
| Interfaz gráfica | `Presentacion` | Formularios de Windows Forms |

## Entidades (8, con 7 o más atributos)

| Entidad | Atributos |
| --- | --- |
| Usuario | 8 |
| Estudiante | 11 |
| Docente | 11 |
| Curso | 8 |
| Aula | 7 |
| Grupo | 15 |
| Inscripcion | 8 |
| Pago | 9 |

## Formularios (11)

- `FrmLogin`: autenticación con contraseña cifrada (SHA-256).
- `FrmMenuPrincipal`: navegación, con opciones según el rol del usuario.
- `FrmUsuarios`, `FrmEstudiantes`, `FrmDocentes`, `FrmCursos`, `FrmAulas`, `FrmGrupos`, `FrmInscripciones` y `FrmPagos`: mantenimiento (crear, leer, actualizar, eliminar) de cada entidad, con búsqueda. Se abren como ventanas modales.
- `FrmDashboard`: estadísticas, morosos, ingresos por método de pago, ocupación de grupos y búsqueda de estudiantes.

## Reglas de negocio principales

- Cédula de 11 dígitos y única, y correo válido y único.
- Un grupo no puede superar la capacidad del aula ni chocar de horario con otro del mismo docente o aula.
- Una inscripción valida cupos, duplicados, estado del estudiante y choques con su horario.
- Un pago no puede superar el saldo que debe el estudiante y genera su número de recibo.

## Base de datos

Dieciocho tablas normalizadas hasta la tercera forma normal. A la tabla `aula` se le agregaron las columnas `ubicacion`, `equipamiento`, `activa` y `observaciones` para cumplir el mínimo de 7 atributos por entidad.
