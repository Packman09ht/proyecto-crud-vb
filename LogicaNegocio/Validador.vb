Imports System.Data.SqlClient
Imports System.Text.RegularExpressions

Friend NotInheritable Class Validador
    Private Sub New()
    End Sub

    Public Shared Sub Exigir(condicion As Boolean, mensaje As String)
        If Not condicion Then
            Throw New ReglaNegocioException(mensaje)
        End If
    End Sub

    Public Shared Function SoloDigitos(texto As String) As String
        Return Regex.Replace(If(texto, String.Empty), "\D", String.Empty)
    End Function

    Public Shared Function CorreoValido(correo As String) As Boolean
        Return Regex.IsMatch(correo, "^\S+@\S+\.\S+$")
    End Function

    Public Shared Sub EjecutarProtegido(accion As Action, mensajeRelacion As String)
        Try
            accion()
        Catch ex As SqlException When ex.Number = 547
            Throw New ReglaNegocioException(mensajeRelacion)
        Catch ex As SqlException When ex.Number = 2601 OrElse ex.Number = 2627
            Throw New ReglaNegocioException("Ya existe un registro con esos datos.")
        End Try
    End Sub
End Class
