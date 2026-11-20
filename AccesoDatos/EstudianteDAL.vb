Imports System.Data
Imports System.Linq

Public Class EstudianteDAL
    Private Const ConsultaBase As String =
        "SELECT e.id_estudiante, e.id_persona, p.cedula, p.nombres, p.apellidos, p.fecha_nacimiento, " &
        "p.telefono, p.correo, p.direccion, e.fecha_registro, e.estado " &
        "FROM estudiante e JOIN persona p ON p.id_persona = e.id_persona"

    Private Shared Function Mapear(fila As DataRow) As Estudiante
        Return New Estudiante With {
            .IdEstudiante = Lector.Entero(fila, "id_estudiante"),
            .IdPersona = Lector.Entero(fila, "id_persona"),
            .Cedula = Lector.Texto(fila, "cedula"),
            .Nombres = Lector.Texto(fila, "nombres"),
            .Apellidos = Lector.Texto(fila, "apellidos"),
            .FechaNacimiento = Lector.Fecha(fila, "fecha_nacimiento"),
            .Telefono = Lector.Texto(fila, "telefono"),
            .Correo = Lector.Texto(fila, "correo"),
            .Direccion = Lector.Texto(fila, "direccion"),
            .FechaRegistro = Lector.Fecha(fila, "fecha_registro"),
            .Estado = Lector.Texto(fila, "estado")
        }
    End Function

    Public Function Listar(filtro As String) As List(Of Estudiante)
        Dim tabla As DataTable = Conexion.Consultar(
            ConsultaBase & " WHERE p.nombres + ' ' + p.apellidos LIKE @f OR ISNULL(p.cedula, '') LIKE @f ORDER BY p.nombres, p.apellidos",
            Conexion.Parametro("@f", "%" & filtro & "%"))
        Return tabla.Rows.Cast(Of DataRow)().Select(Function(f) Mapear(f)).ToList()
    End Function

    Public Function ObtenerPorId(id As Integer) As Estudiante
        Dim tabla As DataTable = Conexion.Consultar(ConsultaBase & " WHERE e.id_estudiante = @id", Conexion.Parametro("@id", id))
        Return If(tabla.Rows.Count = 0, Nothing, Mapear(tabla.Rows(0)))
    End Function

    Public Function ExisteCedula(cedula As String, idPersonaExcluir As Integer) As Boolean
        Dim cantidad As Integer = Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM persona WHERE cedula = @c AND id_persona <> @id",
            Conexion.Parametro("@c", cedula), Conexion.Parametro("@id", idPersonaExcluir)))
        Return cantidad > 0
    End Function

    Public Function ExisteCorreo(correo As String, idPersonaExcluir As Integer) As Boolean
        Dim cantidad As Integer = Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM persona WHERE correo = @c AND id_persona <> @id",
            Conexion.Parametro("@c", correo), Conexion.Parametro("@id", idPersonaExcluir)))
        Return cantidad > 0
    End Function

    Public Function TieneInscripciones(idEstudiante As Integer) As Boolean
        Dim cantidad As Integer = Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM inscripcion WHERE id_estudiante = @id", Conexion.Parametro("@id", idEstudiante)))
        Return cantidad > 0
    End Function

    Public Function Insertar(e As Estudiante) As Integer
        Dim idEstudiante As Integer
        Conexion.EnTransaccion(
            Sub(cn, tx)
                Dim idPersona As Integer = Convert.ToInt32(Conexion.EscalarEn(cn, tx,
                    "INSERT INTO persona(cedula, nombres, apellidos, fecha_nacimiento, telefono, correo, direccion) " &
                    "VALUES(@ced, @nom, @ape, @nac, @tel, @cor, @dir); SELECT CAST(SCOPE_IDENTITY() AS INT)",
                    Conexion.ParametroOpcional("@ced", e.Cedula), Conexion.Parametro("@nom", e.Nombres),
                    Conexion.Parametro("@ape", e.Apellidos), Conexion.Parametro("@nac", e.FechaNacimiento),
                    Conexion.ParametroOpcional("@tel", e.Telefono), Conexion.ParametroOpcional("@cor", e.Correo),
                    Conexion.ParametroOpcional("@dir", e.Direccion)))
                idEstudiante = Convert.ToInt32(Conexion.EscalarEn(cn, tx,
                    "INSERT INTO estudiante(id_persona, fecha_registro, estado) VALUES(@per, @reg, @est); SELECT CAST(SCOPE_IDENTITY() AS INT)",
                    Conexion.Parametro("@per", idPersona), Conexion.Parametro("@reg", e.FechaRegistro),
                    Conexion.Parametro("@est", e.Estado)))
            End Sub)
        Return idEstudiante
    End Function

    Public Sub Actualizar(e As Estudiante)
        Conexion.EnTransaccion(
            Sub(cn, tx)
                Conexion.EjecutarEn(cn, tx,
                    "UPDATE persona SET cedula = @ced, nombres = @nom, apellidos = @ape, fecha_nacimiento = @nac, " &
                    "telefono = @tel, correo = @cor, direccion = @dir WHERE id_persona = @id",
                    Conexion.ParametroOpcional("@ced", e.Cedula), Conexion.Parametro("@nom", e.Nombres),
                    Conexion.Parametro("@ape", e.Apellidos), Conexion.Parametro("@nac", e.FechaNacimiento),
                    Conexion.ParametroOpcional("@tel", e.Telefono), Conexion.ParametroOpcional("@cor", e.Correo),
                    Conexion.ParametroOpcional("@dir", e.Direccion), Conexion.Parametro("@id", e.IdPersona))
                Conexion.EjecutarEn(cn, tx,
                    "UPDATE estudiante SET fecha_registro = @reg, estado = @est WHERE id_estudiante = @id",
                    Conexion.Parametro("@reg", e.FechaRegistro), Conexion.Parametro("@est", e.Estado),
                    Conexion.Parametro("@id", e.IdEstudiante))
            End Sub)
    End Sub

    Public Sub Eliminar(idEstudiante As Integer, idPersona As Integer)
        Conexion.EnTransaccion(
            Sub(cn, tx)
                Conexion.EjecutarEn(cn, tx, "DELETE FROM estudiante WHERE id_estudiante = @id", Conexion.Parametro("@id", idEstudiante))
                Conexion.EjecutarEn(cn, tx, "DELETE FROM persona WHERE id_persona = @id", Conexion.Parametro("@id", idPersona))
            End Sub)
    End Sub
End Class
