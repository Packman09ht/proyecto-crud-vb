Public Class Pago
    Public Property IdPago As Integer
    Public Property IdInscripcion As Integer
    Public Property IdMetodo As Integer
    Public Property Inscripcion As String = String.Empty
    Public Property Metodo As String = String.Empty
    Public Property FechaPago As Date
    Public Property Monto As Decimal
    Public Property Concepto As String = "Mensualidad"
    Public Property NumRecibo As String = String.Empty
End Class
