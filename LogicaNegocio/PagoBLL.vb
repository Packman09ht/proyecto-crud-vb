Public Class PagoBLL
    Private ReadOnly datos As New PagoDAL()

    Public Function Listar(filtro As String) As List(Of Pago)
        Return datos.Listar(filtro)
    End Function

    Public Sub Guardar(p As Pago)
        Validador.Exigir(p.IdInscripcion > 0, "Elige la inscripción.")
        Validador.Exigir(p.IdMetodo > 0, "Elige el método de pago.")
        Validador.Exigir(p.Monto > 0, "El monto debe ser mayor que 0.")
        Validador.Exigir(p.FechaPago <= Date.Today, "La fecha del pago no puede ser futura.")
        Validador.Exigir(Not String.IsNullOrWhiteSpace(p.Concepto), "El concepto es obligatorio.")
        Validador.Exigir(p.Monto <= datos.SaldoDisponible(p.IdInscripcion, p.IdPago), "El monto supera lo que debe el estudiante.")
        If p.IdPago = 0 Then
            datos.Insertar(p)
        Else
            datos.Actualizar(p)
        End If
    End Sub

    Public Sub Eliminar(id As Integer)
        Validador.EjecutarProtegido(Sub() datos.Eliminar(id), "No se puede anular este pago.")
    End Sub
End Class
