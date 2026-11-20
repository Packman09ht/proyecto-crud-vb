Imports System.Collections
Imports System.ComponentModel

<DesignerCategory("Code")>
Public Class FrmDocentes
    Inherits FormMantenimiento

    Private ReadOnly logica As New DocenteBLL()

    Protected Overrides ReadOnly Property Titulo As String
        Get
            Return "Mantenimiento de docentes"
        End Get
    End Property

    Protected Overrides ReadOnly Property TipoEntidad As Type
        Get
            Return GetType(Docente)
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
            New CampoFormulario("Especialidad", "Especialidad", TipoCampo.Texto),
            New CampoFormulario("FechaContratacion", "Fecha de contratación", TipoCampo.Fecha)
        }
    End Function

    Protected Overrides Function ListarEntidades(filtro As String) As IList
        Return logica.Listar(filtro)
    End Function

    Protected Overrides Sub GuardarEntidad(entidad As Object)
        logica.Guardar(DirectCast(entidad, Docente))
    End Sub

    Protected Overrides Sub EliminarEntidad(entidad As Object)
        logica.Eliminar(DirectCast(entidad, Docente))
    End Sub
End Class
