Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient

Public NotInheritable Class Conexion
    Private Sub New()
    End Sub

    Public Shared Function CadenaConexion() As String
        Dim configurada As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("Academia")
        If configurada Is Nothing Then
            Throw New InvalidOperationException("No se encontró la cadena de conexión 'Academia' en App.config.")
        End If
        Return configurada.ConnectionString
    End Function

    Public Shared Function Parametro(nombre As String, valor As Object) As SqlParameter
        Return New SqlParameter(nombre, If(valor, DBNull.Value))
    End Function

    Public Shared Function ParametroOpcional(nombre As String, texto As String) As SqlParameter
        If String.IsNullOrWhiteSpace(texto) Then
            Return New SqlParameter(nombre, DBNull.Value)
        End If
        Return New SqlParameter(nombre, texto.Trim())
    End Function

    Public Shared Function Consultar(sql As String, ParamArray parametros As SqlParameter()) As DataTable
        Using cn As New SqlConnection(CadenaConexion())
            Using cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddRange(parametros)
                Using adaptador As New SqlDataAdapter(cmd)
                    Dim tabla As New DataTable()
                    adaptador.Fill(tabla)
                    Return tabla
                End Using
            End Using
        End Using
    End Function

    Public Shared Function Ejecutar(sql As String, ParamArray parametros As SqlParameter()) As Integer
        Using cn As New SqlConnection(CadenaConexion())
            Using cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddRange(parametros)
                cn.Open()
                Return cmd.ExecuteNonQuery()
            End Using
        End Using
    End Function

    Public Shared Function Escalar(sql As String, ParamArray parametros As SqlParameter()) As Object
        Using cn As New SqlConnection(CadenaConexion())
            Using cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddRange(parametros)
                cn.Open()
                Return cmd.ExecuteScalar()
            End Using
        End Using
    End Function

    Public Shared Sub EnTransaccion(trabajo As Action(Of SqlConnection, SqlTransaction))
        Using cn As New SqlConnection(CadenaConexion())
            cn.Open()
            Using tx As SqlTransaction = cn.BeginTransaction()
                Try
                    trabajo(cn, tx)
                    tx.Commit()
                Catch
                    tx.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Sub

    Public Shared Function EjecutarEn(cn As SqlConnection, tx As SqlTransaction, sql As String, ParamArray parametros As SqlParameter()) As Integer
        Using cmd As New SqlCommand(sql, cn, tx)
            cmd.Parameters.AddRange(parametros)
            Return cmd.ExecuteNonQuery()
        End Using
    End Function

    Public Shared Function EscalarEn(cn As SqlConnection, tx As SqlTransaction, sql As String, ParamArray parametros As SqlParameter()) As Object
        Using cmd As New SqlCommand(sql, cn, tx)
            cmd.Parameters.AddRange(parametros)
            Return cmd.ExecuteScalar()
        End Using
    End Function
End Class
