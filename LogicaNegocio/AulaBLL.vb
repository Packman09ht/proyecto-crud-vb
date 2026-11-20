Public Class AulaBLL
    Private ReadOnly datos As New AulaDAL()

    Public Function Listar(filtro As String) As List(Of Aula)
        Return datos.Listar(filtro)
    End Function

    Public Sub Guardar(a As Aula)
        Validador.Exigir(Not String.IsNullOrWhiteSpace(a.Nombre), "El nombre del aula es obligatorio.")
        Validador.Exigir(a.Capacidad > 0, "La capacidad debe ser mayor que 0.")
        Validador.Exigir(Not datos.ExisteNombre(a.Nombre.Trim(), a.IdAula), "Ya existe un aula con ese nombre.")
        a.Nombre = a.Nombre.Trim()
        If a.IdAula = 0 Then
            datos.Insertar(a)
        Else
            datos.Actualizar(a)
        End If
    End Sub

    Public Sub Eliminar(id As Integer)
        Validador.Exigir(Not datos.TieneGrupos(id), "El aula tiene grupos asignados. Márcala como inactiva en su lugar.")
        Validador.EjecutarProtegido(Sub() datos.Eliminar(id), "No se puede eliminar: tiene datos relacionados.")
    End Sub
End Class
