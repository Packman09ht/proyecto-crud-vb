Imports System.Data

Public Class ReporteBLL
    Private ReadOnly datos As New ReporteDAL()
    Private ReadOnly estudiantes As New EstudianteDAL()

    Public Function Resumen() As DataRow
        Return datos.Resumen().Rows(0)
    End Function

    Public Function Morosos() As DataTable
        Return datos.Morosos()
    End Function

    Public Function IngresosPorMetodo() As DataTable
        Return datos.IngresosPorMetodo()
    End Function

    Public Function OcupacionPorGrupo() As DataTable
        Return datos.OcupacionPorGrupo()
    End Function

    Public Function BuscarEstudiantes(filtro As String) As List(Of Estudiante)
        Return estudiantes.Listar(filtro)
    End Function
End Class
