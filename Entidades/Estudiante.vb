Public Class Estudiante
    Public Property IdEstudiante As Integer
    Public Property IdPersona As Integer
    Public Property Cedula As String = String.Empty
    Public Property Nombres As String = String.Empty
    Public Property Apellidos As String = String.Empty
    Public Property FechaNacimiento As Date
    Public Property Telefono As String = String.Empty
    Public Property Correo As String = String.Empty
    Public Property Direccion As String = String.Empty
    Public Property FechaRegistro As Date
    Public Property Estado As String = "activo"

    Public ReadOnly Property NombreCompleto As String
        Get
            Return Nombres & " " & Apellidos
        End Get
    End Property
End Class
