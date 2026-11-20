Public Class Grupo
    Public Property IdGrupo As Integer
    Public Property IdCurso As Integer
    Public Property IdDocente As Integer
    Public Property IdAula As Integer
    Public Property Curso As String = String.Empty
    Public Property Docente As String = String.Empty
    Public Property Aula As String = String.Empty
    Public Property CupoMaximo As Integer
    Public Property Inscritos As Integer
    Public Property FechaInicio As Date
    Public Property FechaFin As Date
    Public Property Estado As String = "abierto"
    Public Property Dias As String = String.Empty
    Public Property HoraInicio As String = "18:00"
    Public Property HoraFin As String = "20:00"
End Class
