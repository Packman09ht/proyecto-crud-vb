Public Class InscripcionBLL
    Private ReadOnly datos As New InscripcionDAL()
    Private ReadOnly grupos As New GrupoDAL()

    Public Function Listar(filtro As String) As List(Of Inscripcion)
        Return datos.Listar(filtro)
    End Function

    Public Sub Guardar(i As Inscripcion)
        Validador.Exigir(i.IdEstudiante > 0 AndAlso i.IdGrupo > 0, "Elige el estudiante y el grupo.")
        Validador.Exigir({"activa", "retirada", "finalizada"}.Contains(i.Estado), "El estado no es válido.")
        Dim previa As Inscripcion = datos.ObtenerPrevia(i.IdEstudiante, i.IdGrupo)
        If i.IdInscripcion = 0 Then
            Validador.Exigir(datos.EstadoEstudiante(i.IdEstudiante) = "activo", "El estudiante no está activo.")
            Validador.Exigir(previa Is Nothing OrElse previa.Estado <> "activa", "El estudiante ya está inscrito en ese grupo.")
            ValidarGrupo(i.IdGrupo, i.IdEstudiante, If(previa Is Nothing, 0, previa.IdInscripcion))
            i.FechaInscripcion = Date.Today
            i.Estado = "activa"
            If previa Is Nothing Then
                datos.Insertar(i)
            Else
                previa.Estado = "activa"
                previa.FechaInscripcion = Date.Today
                datos.Actualizar(previa)
            End If
        Else
            Validador.Exigir(previa Is Nothing OrElse previa.IdInscripcion = i.IdInscripcion, "El estudiante ya tiene otra inscripción en ese grupo.")
            If i.Estado = "activa" Then
                ValidarGrupo(i.IdGrupo, i.IdEstudiante, i.IdInscripcion)
            End If
            datos.Actualizar(i)
        End If
    End Sub

    Private Sub ValidarGrupo(idGrupo As Integer, idEstudiante As Integer, idInscripcionExcluir As Integer)
        Dim destino As Grupo = grupos.ObtenerPorId(idGrupo)
        Validador.Exigir(destino IsNot Nothing, "El grupo no existe.")
        Validador.Exigir(destino.Estado <> "cerrado", "El grupo está cerrado.")
        Validador.Exigir(datos.CuposOcupados(idGrupo) < destino.CupoMaximo, "El grupo no tiene cupos.")
        Validador.Exigir(Not datos.ChocaConHorario(idEstudiante, idGrupo, idInscripcionExcluir), "El estudiante ya tiene otra clase a esa hora.")
    End Sub

    Public Sub Eliminar(id As Integer)
        Validador.Exigir(Not datos.TienePagos(id), "La inscripción tiene pagos registrados. Cambia su estado a retirada.")
        Validador.EjecutarProtegido(Sub() datos.Eliminar(id), "No se puede eliminar: tiene datos relacionados.")
    End Sub
End Class
