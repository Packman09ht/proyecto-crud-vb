Public Class Usuario
    Public Property IdUsuario As Integer
    Public Property Username As String = String.Empty
    Public Property Contrasena As String = String.Empty
    Public Property ContrasenaHash As String = String.Empty
    Public Property IdRol As Integer
    Public Property Rol As String = String.Empty
    Public Property Activo As Boolean = True
    Public Property NombrePersona As String = String.Empty
End Class
