Imports System.Data
Imports System.Linq

Public Class GrupoDAL
    Private Const ConsultaBase As String =
        "SELECT g.id_grupo, g.id_curso, g.id_docente, g.id_aula, c.nombre AS curso, " &
        "p.nombres + ' ' + p.apellidos AS docente, a.nombre AS aula, g.cupo_maximo, g.fecha_inicio, g.fecha_fin, g.estado, " &
        "(SELECT TOP 1 CONVERT(VARCHAR(5), h.hora_inicio, 108) FROM horario h WHERE h.id_grupo = g.id_grupo) AS hora_inicio, " &
        "(SELECT TOP 1 CONVERT(VARCHAR(5), h.hora_fin, 108) FROM horario h WHERE h.id_grupo = g.id_grupo) AS hora_fin, " &
        "STUFF((SELECT ',' + h.dia_semana FROM horario h WHERE h.id_grupo = g.id_grupo " &
        "ORDER BY CASE h.dia_semana WHEN N'Lunes' THEN 1 WHEN N'Martes' THEN 2 WHEN N'Miércoles' THEN 3 " &
        "WHEN N'Jueves' THEN 4 WHEN N'Viernes' THEN 5 ELSE 6 END FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(100)'), 1, 1, '') AS dias, " &
        "(SELECT COUNT(*) FROM inscripcion i WHERE i.id_grupo = g.id_grupo AND i.estado = 'activa') AS inscritos " &
        "FROM grupo g JOIN curso c ON c.id_curso = g.id_curso JOIN docente d ON d.id_docente = g.id_docente " &
        "JOIN persona p ON p.id_persona = d.id_persona JOIN aula a ON a.id_aula = g.id_aula"

    Private Shared Function Mapear(fila As DataRow) As Grupo
        Return New Grupo With {
            .IdGrupo = Lector.Entero(fila, "id_grupo"),
            .IdCurso = Lector.Entero(fila, "id_curso"),
            .IdDocente = Lector.Entero(fila, "id_docente"),
            .IdAula = Lector.Entero(fila, "id_aula"),
            .Curso = Lector.Texto(fila, "curso"),
            .Docente = Lector.Texto(fila, "docente"),
            .Aula = Lector.Texto(fila, "aula"),
            .CupoMaximo = Lector.Entero(fila, "cupo_maximo"),
            .Inscritos = Lector.Entero(fila, "inscritos"),
            .FechaInicio = Lector.Fecha(fila, "fecha_inicio"),
            .FechaFin = Lector.Fecha(fila, "fecha_fin"),
            .Estado = Lector.Texto(fila, "estado"),
            .Dias = Lector.Texto(fila, "dias"),
            .HoraInicio = Lector.Texto(fila, "hora_inicio"),
            .HoraFin = Lector.Texto(fila, "hora_fin")
        }
    End Function

    Public Function Listar(filtro As String) As List(Of Grupo)
        Dim tabla As DataTable = Conexion.Consultar(
            ConsultaBase & " WHERE c.nombre LIKE @f OR p.nombres + ' ' + p.apellidos LIKE @f ORDER BY g.id_grupo DESC",
            Conexion.Parametro("@f", "%" & filtro & "%"))
        Return tabla.Rows.Cast(Of DataRow)().Select(Function(f) Mapear(f)).ToList()
    End Function

    Public Function ObtenerPorId(id As Integer) As Grupo
        Dim tabla As DataTable = Conexion.Consultar(ConsultaBase & " WHERE g.id_grupo = @id", Conexion.Parametro("@id", id))
        Return If(tabla.Rows.Count = 0, Nothing, Mapear(tabla.Rows(0)))
    End Function

    Public Function DiaConChoque(dia As String, inicio As TimeSpan, fin As TimeSpan, idDocente As Integer, idAula As Integer, idGrupoExcluir As Integer) As Boolean
        Dim cantidad As Integer = Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM horario h JOIN grupo g ON g.id_grupo = h.id_grupo " &
            "WHERE h.dia_semana = @dia AND g.id_grupo <> @ex AND g.estado <> 'cerrado' " &
            "AND @ini < h.hora_fin AND h.hora_inicio < @fin AND (g.id_docente = @doc OR g.id_aula = @aula)",
            Conexion.Parametro("@dia", dia), Conexion.Parametro("@ex", idGrupoExcluir),
            Conexion.Parametro("@ini", inicio), Conexion.Parametro("@fin", fin),
            Conexion.Parametro("@doc", idDocente), Conexion.Parametro("@aula", idAula)))
        Return cantidad > 0
    End Function

    Public Function TieneInscripciones(idGrupo As Integer) As Boolean
        Dim cantidad As Integer = Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM inscripcion WHERE id_grupo = @id", Conexion.Parametro("@id", idGrupo)))
        Return cantidad > 0
    End Function

    Private Shared Sub GuardarHorarios(cn As SqlClient.SqlConnection, tx As SqlClient.SqlTransaction, g As Grupo, idGrupo As Integer)
        Conexion.EjecutarEn(cn, tx, "DELETE FROM horario WHERE id_grupo = @id", Conexion.Parametro("@id", idGrupo))
        For Each dia As String In g.Dias.Split(","c)
            Conexion.EjecutarEn(cn, tx,
                "INSERT INTO horario(id_grupo, dia_semana, hora_inicio, hora_fin) VALUES(@g, @d, @i, @f)",
                Conexion.Parametro("@g", idGrupo), Conexion.Parametro("@d", dia.Trim()),
                Conexion.Parametro("@i", TimeSpan.Parse(g.HoraInicio)), Conexion.Parametro("@f", TimeSpan.Parse(g.HoraFin)))
        Next
    End Sub

    Public Function Insertar(g As Grupo) As Integer
        Dim idGrupo As Integer
        Conexion.EnTransaccion(
            Sub(cn, tx)
                idGrupo = Convert.ToInt32(Conexion.EscalarEn(cn, tx,
                    "INSERT INTO grupo(id_curso, id_docente, id_aula, cupo_maximo, fecha_inicio, fecha_fin, estado) " &
                    "VALUES(@c, @d, @a, @cupo, @ini, @fin, @est); SELECT CAST(SCOPE_IDENTITY() AS INT)",
                    Conexion.Parametro("@c", g.IdCurso), Conexion.Parametro("@d", g.IdDocente), Conexion.Parametro("@a", g.IdAula),
                    Conexion.Parametro("@cupo", g.CupoMaximo), Conexion.Parametro("@ini", g.FechaInicio),
                    Conexion.Parametro("@fin", g.FechaFin), Conexion.Parametro("@est", g.Estado)))
                GuardarHorarios(cn, tx, g, idGrupo)
                Conexion.EjecutarEn(cn, tx,
                    "INSERT INTO evaluacion(id_grupo, nombre, ponderacion) VALUES(@g, N'Parcial', 30), (@g, N'Oral', 30), (@g, N'Final', 40)",
                    Conexion.Parametro("@g", idGrupo))
            End Sub)
        Return idGrupo
    End Function

    Public Sub Actualizar(g As Grupo)
        Conexion.EnTransaccion(
            Sub(cn, tx)
                Conexion.EjecutarEn(cn, tx,
                    "UPDATE grupo SET id_curso = @c, id_docente = @d, id_aula = @a, cupo_maximo = @cupo, " &
                    "fecha_inicio = @ini, fecha_fin = @fin, estado = @est WHERE id_grupo = @id",
                    Conexion.Parametro("@c", g.IdCurso), Conexion.Parametro("@d", g.IdDocente), Conexion.Parametro("@a", g.IdAula),
                    Conexion.Parametro("@cupo", g.CupoMaximo), Conexion.Parametro("@ini", g.FechaInicio),
                    Conexion.Parametro("@fin", g.FechaFin), Conexion.Parametro("@est", g.Estado), Conexion.Parametro("@id", g.IdGrupo))
                GuardarHorarios(cn, tx, g, g.IdGrupo)
            End Sub)
    End Sub

    Public Sub Eliminar(idGrupo As Integer)
        Conexion.EnTransaccion(
            Sub(cn, tx)
                Conexion.EjecutarEn(cn, tx, "DELETE FROM horario WHERE id_grupo = @id", Conexion.Parametro("@id", idGrupo))
                Conexion.EjecutarEn(cn, tx, "DELETE FROM evaluacion WHERE id_grupo = @id", Conexion.Parametro("@id", idGrupo))
                Conexion.EjecutarEn(cn, tx, "DELETE FROM sesion WHERE id_grupo = @id", Conexion.Parametro("@id", idGrupo))
                Conexion.EjecutarEn(cn, tx, "DELETE FROM grupo WHERE id_grupo = @id", Conexion.Parametro("@id", idGrupo))
            End Sub)
    End Sub
End Class
