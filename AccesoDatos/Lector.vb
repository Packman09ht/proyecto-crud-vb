Imports System.Data

Public NotInheritable Class Lector
    Private Sub New()
    End Sub

    Public Shared Function Texto(fila As DataRow, columna As String) As String
        Dim valor As Object = fila(columna)
        Return If(IsDBNull(valor), String.Empty, Convert.ToString(valor))
    End Function

    Public Shared Function Entero(fila As DataRow, columna As String) As Integer
        Dim valor As Object = fila(columna)
        Return If(IsDBNull(valor), 0, Convert.ToInt32(valor))
    End Function

    Public Shared Function Monto(fila As DataRow, columna As String) As Decimal
        Dim valor As Object = fila(columna)
        Return If(IsDBNull(valor), 0D, Convert.ToDecimal(valor))
    End Function

    Public Shared Function Fecha(fila As DataRow, columna As String) As Date
        Dim valor As Object = fila(columna)
        Return If(IsDBNull(valor), Date.MinValue, Convert.ToDateTime(valor))
    End Function

    Public Shared Function Logico(fila As DataRow, columna As String) As Boolean
        Dim valor As Object = fila(columna)
        Return If(IsDBNull(valor), False, Convert.ToBoolean(valor))
    End Function
End Class
