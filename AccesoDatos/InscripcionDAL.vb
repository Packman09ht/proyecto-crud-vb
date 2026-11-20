Imports System.Data
Imports System.Linq

Public Class InscripcionDAL
    Private Const ConsultaBase As String =
        "SELECT i.id_inscripcion, i.id_estudiante, i.id_grupo, p.nombres + ' ' + p.apellidos AS estudiante, c.nombre AS curso, " &
        "i.fecha_inscripcion, i.estado, " &
        "c.precio - ISNULL((SELECT SUM(pg.monto) FROM pago pg WHERE pg.id_inscripcion = i.id_inscripcion), 0) AS saldo " &
        "FROM inscripcion i JOIN estudiante e ON e.id_estudiante = i.id_estudiante JOIN persona p ON p.id_persona = e.id_persona " &
        "JOIN grupo g ON g.id_grupo = i.id_grupo JOIN curso c ON c.id_curso = g.id_curso"

    Private Shared Function Mapear(fila As DataRow) As Inscripcion
        Return New Inscripcion With {
            .IdInscripcion = Lector.Entero(fila, "id_inscripcion"),
            .IdEstudiante = Lector.Entero(fila, "id_estudiante"),
            .IdGrupo = Lector.Entero(fila, "id_grupo"),
            .Estudiante = Lector.Texto(fila, "estudiante"),
            .Curso = Lector.Texto(fila, "curso"),
            .FechaInscripcion = Lector.Fecha(fila, "fecha_inscripcion"),
            .Estado = Lector.Texto(fila, "estado"),
            .Saldo = Lector.Monto(fila, "saldo")
        }
    End Function

    Public Function Listar(filtro As String) As List(Of Inscripcion)
        Dim tabla As DataTable = Conexion.Consultar(
            ConsultaBase & " WHERE p.nombres + ' ' + p.apellidos LIKE @f OR c.nombre LIKE @f ORDER BY i.id_inscripcion DESC",
            Conexion.Parametro("@f", "%" & filtro & "%"))
        Return tabla.Rows.Cast(Of DataRow)().Select(Function(f) Mapear(f)).ToList()
    End Function

    Public Function ObtenerPrevia(idEstudiante As Integer, idGrupo As Integer) As Inscripcion
        Dim tabla As DataTable = Conexion.Consultar(
            ConsultaBase & " WHERE i.id_estudiante = @e AND i.id_grupo = @g",
            Conexion.Parametro("@e", idEstudiante), Conexion.Parametro("@g", idGrupo))
        Return If(tabla.Rows.Count = 0, Nothing, Mapear(tabla.Rows(0)))
    End Function

    Public Function EstadoEstudiante(idEstudiante As Integer) As String
        Dim valor As Object = Conexion.Escalar("SELECT estado FROM estudiante WHERE id_estudiante = @id", Conexion.Parametro("@id", idEstudiante))
        Return If(valor Is Nothing, String.Empty, Convert.ToString(valor))
    End Function

    Public Function CuposOcupados(idGrupo As Integer) As Integer
        Return Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM inscripcion WHERE id_grupo = @id AND estado = 'activa'", Conexion.Parametro("@id", idGrupo)))
    End Function

    Public Function ChocaConHorario(idEstudiante As Integer, idGrupo As Integer, idInscripcionExcluir As Integer) As Boolean
        Dim cantidad As Integer = Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM horario h JOIN horario o ON o.dia_semana = h.dia_semana AND o.hora_inicio < h.hora_fin AND h.hora_inicio < o.hora_fin " &
            "JOIN inscripcion i ON i.id_grupo = o.id_grupo AND i.estado = 'activa' AND i.id_estudiante = @e AND i.id_inscripcion <> @ex " &
            "WHERE h.id_grupo = @g AND o.id_grupo <> @g",
            Conexion.Parametro("@e", idEstudiante), Conexion.Parametro("@g", idGrupo), Conexion.Parametro("@ex", idInscripcionExcluir)))
        Return cantidad > 0
    End Function

    Public Function TienePagos(idInscripcion As Integer) As Boolean
        Dim cantidad As Integer = Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM pago WHERE id_inscripcion = @id", Conexion.Parametro("@id", idInscripcion)))
        Return cantidad > 0
    End Function

    Public Function Insertar(i As Inscripcion) As Integer
        Return Convert.ToInt32(Conexion.Escalar(
            "INSERT INTO inscripcion(id_estudiante, id_grupo, fecha_inscripcion, estado) VALUES(@e, @g, @f, @s); SELECT CAST(SCOPE_IDENTITY() AS INT)",
            Conexion.Parametro("@e", i.IdEstudiante), Conexion.Parametro("@g", i.IdGrupo),
            Conexion.Parametro("@f", i.FechaInscripcion), Conexion.Parametro("@s", i.Estado)))
    End Function

    Public Sub Actualizar(i As Inscripcion)
        Conexion.Ejecutar(
            "UPDATE inscripcion SET id_estudiante = @e, id_grupo = @g, fecha_inscripcion = @f, estado = @s WHERE id_inscripcion = @id",
            Conexion.Parametro("@e", i.IdEstudiante), Conexion.Parametro("@g", i.IdGrupo),
            Conexion.Parametro("@f", i.FechaInscripcion), Conexion.Parametro("@s", i.Estado), Conexion.Parametro("@id", i.IdInscripcion))
    End Sub

    Public Sub Eliminar(id As Integer)
        Conexion.EnTransaccion(
            Sub(cn, tx)
                Conexion.EjecutarEn(cn, tx, "DELETE FROM asistencia WHERE id_inscripcion = @id", Conexion.Parametro("@id", id))
                Conexion.EjecutarEn(cn, tx, "DELETE FROM calificacion WHERE id_inscripcion = @id", Conexion.Parametro("@id", id))
                Conexion.EjecutarEn(cn, tx, "DELETE FROM inscripcion WHERE id_inscripcion = @id", Conexion.Parametro("@id", id))
            End Sub)
    End Sub
End Class
