Public Class GrupoBLL
    Private ReadOnly datos As New GrupoDAL()
    Private ReadOnly aulas As New AulaDAL()

    Public Function Listar(filtro As String) As List(Of Grupo)
        Return datos.Listar(filtro)
    End Function

    Public Sub Guardar(g As Grupo)
        Validador.Exigir(g.IdCurso > 0 AndAlso g.IdDocente > 0 AndAlso g.IdAula > 0, "Elige el curso, el docente y el aula.")
        Validador.Exigir(g.CupoMaximo > 0, "El cupo debe ser mayor que 0.")
        Validador.Exigir(g.CupoMaximo <= aulas.ObtenerPorId(g.IdAula).Capacidad, "El cupo supera la capacidad del aula.")
        Validador.Exigir(g.FechaFin > g.FechaInicio, "La fecha de fin debe ser posterior a la de inicio.")
        Validador.Exigir({"abierto", "en curso", "cerrado"}.Contains(g.Estado), "El estado no es válido.")
        Validador.Exigir(Not String.IsNullOrWhiteSpace(g.Dias), "Elige al menos un día de clase.")
        Dim inicio As TimeSpan
        Dim fin As TimeSpan
        Validador.Exigir(TimeSpan.TryParse(g.HoraInicio, inicio) AndAlso TimeSpan.TryParse(g.HoraFin, fin), "Las horas no son válidas (formato HH:mm).")
        Validador.Exigir(fin > inicio, "La hora de fin debe ser posterior a la de inicio.")
        If g.IdGrupo > 0 Then
            Dim actual As Grupo = datos.ObtenerPorId(g.IdGrupo)
            Validador.Exigir(g.CupoMaximo >= actual.Inscritos, "El cupo no puede ser menor que los estudiantes ya inscritos.")
        End If
        If g.Estado <> "cerrado" Then
            For Each dia As String In g.Dias.Split(","c)
                Validador.Exigir(Not datos.DiaConChoque(dia.Trim(), inicio, fin, g.IdDocente, g.IdAula, g.IdGrupo),
                                 "Choque de horario el " & dia.Trim() & ": el docente o el aula ya están ocupados.")
            Next
        End If
        If g.IdGrupo = 0 Then
            datos.Insertar(g)
        Else
            datos.Actualizar(g)
        End If
    End Sub

    Public Sub Eliminar(id As Integer)
        Validador.Exigir(Not datos.TieneInscripciones(id), "El grupo tiene inscripciones.")
        Validador.EjecutarProtegido(Sub() datos.Eliminar(id), "No se puede eliminar: tiene datos relacionados.")
    End Sub
End Class
