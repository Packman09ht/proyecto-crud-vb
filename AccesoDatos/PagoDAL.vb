Imports System.Data
Imports System.Linq

Public Class PagoDAL
    Private Const ConsultaBase As String =
        "SELECT pg.id_pago, pg.id_inscripcion, pg.id_metodo, p.nombres + ' ' + p.apellidos + ' - ' + c.nombre AS inscripcion, " &
        "m.nombre AS metodo, pg.fecha_pago, pg.monto, pg.concepto, pg.num_recibo " &
        "FROM pago pg JOIN metodo_pago m ON m.id_metodo = pg.id_metodo JOIN inscripcion i ON i.id_inscripcion = pg.id_inscripcion " &
        "JOIN estudiante e ON e.id_estudiante = i.id_estudiante JOIN persona p ON p.id_persona = e.id_persona " &
        "JOIN grupo g ON g.id_grupo = i.id_grupo JOIN curso c ON c.id_curso = g.id_curso"

    Private Shared Function Mapear(fila As DataRow) As Pago
        Return New Pago With {
            .IdPago = Lector.Entero(fila, "id_pago"),
            .IdInscripcion = Lector.Entero(fila, "id_inscripcion"),
            .IdMetodo = Lector.Entero(fila, "id_metodo"),
            .Inscripcion = Lector.Texto(fila, "inscripcion"),
            .Metodo = Lector.Texto(fila, "metodo"),
            .FechaPago = Lector.Fecha(fila, "fecha_pago"),
            .Monto = Lector.Monto(fila, "monto"),
            .Concepto = Lector.Texto(fila, "concepto"),
            .NumRecibo = Lector.Texto(fila, "num_recibo")
        }
    End Function

    Public Function Listar(filtro As String) As List(Of Pago)
        Dim tabla As DataTable = Conexion.Consultar(
            ConsultaBase & " WHERE pg.num_recibo LIKE @f OR p.nombres + ' ' + p.apellidos LIKE @f ORDER BY pg.id_pago DESC",
            Conexion.Parametro("@f", "%" & filtro & "%"))
        Return tabla.Rows.Cast(Of DataRow)().Select(Function(f) Mapear(f)).ToList()
    End Function

    Public Function SaldoDisponible(idInscripcion As Integer, idPagoExcluir As Integer) As Decimal
        Dim valor As Object = Conexion.Escalar(
            "SELECT c.precio - ISNULL((SELECT SUM(pg.monto) FROM pago pg WHERE pg.id_inscripcion = i.id_inscripcion AND pg.id_pago <> @ex), 0) " &
            "FROM inscripcion i JOIN grupo g ON g.id_grupo = i.id_grupo JOIN curso c ON c.id_curso = g.id_curso WHERE i.id_inscripcion = @id",
            Conexion.Parametro("@id", idInscripcion), Conexion.Parametro("@ex", idPagoExcluir))
        Return If(valor Is Nothing, 0D, Convert.ToDecimal(valor))
    End Function

    Public Function Insertar(p As Pago) As Integer
        Dim idPago As Integer
        Conexion.EnTransaccion(
            Sub(cn, tx)
                idPago = Convert.ToInt32(Conexion.EscalarEn(cn, tx,
                    "INSERT INTO pago(id_inscripcion, id_metodo, fecha_pago, monto, concepto, num_recibo) " &
                    "VALUES(@i, @m, @f, @mo, @c, 'T' + LEFT(REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', ''), 15)); SELECT CAST(SCOPE_IDENTITY() AS INT)",
                    Conexion.Parametro("@i", p.IdInscripcion), Conexion.Parametro("@m", p.IdMetodo), Conexion.Parametro("@f", p.FechaPago),
                    Conexion.Parametro("@mo", p.Monto), Conexion.Parametro("@c", p.Concepto)))
                Conexion.EjecutarEn(cn, tx,
                    "UPDATE pago SET num_recibo = 'REC-' + CASE WHEN @id < 10000 THEN RIGHT('0000' + CAST(@id AS VARCHAR(10)), 4) ELSE CAST(@id AS VARCHAR(10)) END WHERE id_pago = @id",
                    Conexion.Parametro("@id", idPago))
            End Sub)
        Return idPago
    End Function

    Public Sub Actualizar(p As Pago)
        Conexion.Ejecutar(
            "UPDATE pago SET id_inscripcion = @i, id_metodo = @m, fecha_pago = @f, monto = @mo, concepto = @c WHERE id_pago = @id",
            Conexion.Parametro("@i", p.IdInscripcion), Conexion.Parametro("@m", p.IdMetodo), Conexion.Parametro("@f", p.FechaPago),
            Conexion.Parametro("@mo", p.Monto), Conexion.Parametro("@c", p.Concepto), Conexion.Parametro("@id", p.IdPago))
    End Sub

    Public Sub Eliminar(id As Integer)
        Conexion.Ejecutar("DELETE FROM pago WHERE id_pago = @id", Conexion.Parametro("@id", id))
    End Sub
End Class
