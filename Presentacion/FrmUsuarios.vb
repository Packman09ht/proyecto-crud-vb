Imports System.Collections
Imports System.ComponentModel

<DesignerCategory("Code")>
Public Class FrmUsuarios
    Inherits FormMantenimiento

    Private ReadOnly logica As New UsuarioBLL()
    Private ReadOnly catalogo As New CatalogoBLL()

    Protected Overrides ReadOnly Property Titulo As String
        Get
            Return "Mantenimiento de usuarios"
        End Get
    End Property

    Protected Overrides ReadOnly Property TipoEntidad As Type
        Get
            Return GetType(Usuario)
        End Get
    End Property

    Protected Overrides Function DefinirCampos() As List(Of CampoFormulario)
        Return New List(Of CampoFormulario) From {
            New CampoFormulario("Username", "Usuario", TipoCampo.Texto),
            New CampoFormulario("Contrasena", "Contraseña", TipoCampo.Contrasena),
            New CampoFormulario("IdRol", "Rol", TipoCampo.Combo) With {.Opciones = catalogo.Roles()},
            New CampoFormulario("Activo", "Activo", TipoCampo.Logico)
        }
    End Function

    Protected Overrides Function ListarEntidades(filtro As String) As IList
        Return logica.Listar(filtro)
    End Function

    Protected Overrides Sub GuardarEntidad(entidad As Object)
        logica.Guardar(DirectCast(entidad, Usuario))
    End Sub

    Protected Overrides Sub EliminarEntidad(entidad As Object)
        logica.Eliminar(DirectCast(entidad, Usuario).IdUsuario)
    End Sub
End Class
