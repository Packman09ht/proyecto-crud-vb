Imports System.Collections
Imports System.ComponentModel

<DesignerCategory("Code")>
Public Class FrmAulas
    Inherits FormMantenimiento

    Private ReadOnly logica As New AulaBLL()

    Protected Overrides ReadOnly Property Titulo As String
        Get
            Return "Mantenimiento de aulas"
        End Get
    End Property

    Protected Overrides ReadOnly Property TipoEntidad As Type
        Get
            Return GetType(Aula)
        End Get
    End Property

    Protected Overrides Function DefinirCampos() As List(Of CampoFormulario)
        Return New List(Of CampoFormulario) From {
            New CampoFormulario("Nombre", "Nombre", TipoCampo.Texto),
            New CampoFormulario("Capacidad", "Capacidad", TipoCampo.Entero) With {.Minimo = 1D, .Maximo = 200D},
            New CampoFormulario("Ubicacion", "Ubicación", TipoCampo.Texto),
            New CampoFormulario("Equipamiento", "Equipamiento", TipoCampo.Texto),
            New CampoFormulario("Activa", "Activa", TipoCampo.Logico),
            New CampoFormulario("Observaciones", "Observaciones", TipoCampo.TextoLargo)
        }
    End Function

    Protected Overrides Function ListarEntidades(filtro As String) As IList
        Return logica.Listar(filtro)
    End Function

    Protected Overrides Sub GuardarEntidad(entidad As Object)
        logica.Guardar(DirectCast(entidad, Aula))
    End Sub

    Protected Overrides Sub EliminarEntidad(entidad As Object)
        logica.Eliminar(DirectCast(entidad, Aula).IdAula)
    End Sub
End Class
