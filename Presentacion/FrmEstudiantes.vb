Imports System.Collections
Imports System.ComponentModel

<DesignerCategory("Code")>
Public Class FrmEstudiantes
    Inherits FormMantenimiento

    Private ReadOnly logica As New EstudianteBLL()

    Protected Overrides ReadOnly Property Titulo As String
        Get
            Return "Mantenimiento de estudiantes"
        End Get
    End Property

    Protected Overrides ReadOnly Property TipoEntidad As Type
        Get
            Return GetType(Estudiante)
        End Get
    End Property

    Protected Overrides Function DefinirCampos() As List(Of CampoFormulario)
        Return New List(Of CampoFormulario) From {
            New CampoFormulario("Nombres", "Nombres", TipoCampo.Texto),
            New CampoFormulario("Apellidos", "Apellidos", TipoCampo.Texto),
            New CampoFormulario("Cedula", "Cédula (11 dígitos)", TipoCampo.Texto),
            New CampoFormulario("FechaNacimiento", "Fecha de nacimiento", TipoCampo.Fecha),
            New CampoFormulario("Telefono", "Teléfono", TipoCampo.Texto),
            New CampoFormulario("Correo", "Correo", TipoCampo.Texto),
            New CampoFormulario("Direccion", "Dirección", TipoCampo.Texto),
            New CampoFormulario("FechaRegistro", "Fecha de registro", TipoCampo.Fecha),
            New CampoFormulario("Estado", "Estado", TipoCampo.ComboTexto) With {.Textos = New String() {"activo", "inactivo", "retirado"}}
        }
    End Function

    Protected Overrides Function ListarEntidades(filtro As String) As IList
        Return logica.Listar(filtro)
    End Function

    Protected Overrides Sub GuardarEntidad(entidad As Object)
        logica.Guardar(DirectCast(entidad, Estudiante))
    End Sub

    Protected Overrides Sub EliminarEntidad(entidad As Object)
        logica.Eliminar(DirectCast(entidad, Estudiante))
    End Sub
End Class
