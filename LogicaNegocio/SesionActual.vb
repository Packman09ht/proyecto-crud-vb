Public NotInheritable Class SesionActual
    Private Sub New()
    End Sub

    Public Shared Property Usuario As Usuario

    Public Shared ReadOnly Property Rol As String
        Get
            Return If(Usuario Is Nothing, String.Empty, Usuario.Rol)
        End Get
    End Property

    Public Shared ReadOnly Property EsAdministrador As Boolean
        Get
            Return Rol = "Administrador"
        End Get
    End Property

    Public Shared ReadOnly Property EsRecepcion As Boolean
        Get
            Return Rol = "Recepción"
        End Get
    End Property

    Public Shared Sub Cerrar()
        Usuario = Nothing
    End Sub
End Class
