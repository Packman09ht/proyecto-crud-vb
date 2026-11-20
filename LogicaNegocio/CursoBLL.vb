Public Class CursoBLL
    Private ReadOnly datos As New CursoDAL()

    Public Function Listar(filtro As String) As List(Of Curso)
        Return datos.Listar(filtro)
    End Function

    Public Sub Guardar(c As Curso)
        Validador.Exigir(Not String.IsNullOrWhiteSpace(c.Nombre), "El nombre del curso es obligatorio.")
        Validador.Exigir(c.IdIdioma > 0, "Elige un idioma.")
        Validador.Exigir(c.IdNivel > 0, "Elige un nivel.")
        Validador.Exigir(c.DuracionHoras > 0, "La duración debe ser mayor que 0.")
        Validador.Exigir(c.Precio >= 0, "El precio no puede ser negativo.")
        c.Nombre = c.Nombre.Trim()
        If c.IdCurso = 0 Then
            datos.Insertar(c)
        Else
            datos.Actualizar(c)
        End If
    End Sub

    Public Sub Eliminar(id As Integer)
        Validador.Exigir(Not datos.TieneGrupos(id), "El curso tiene grupos creados.")
        Validador.EjecutarProtegido(Sub() datos.Eliminar(id), "No se puede eliminar: tiene datos relacionados.")
    End Sub
End Class
