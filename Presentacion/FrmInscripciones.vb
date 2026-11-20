Imports System.Collections
Imports System.ComponentModel

<DesignerCategory("Code")>
Public Class FrmInscripciones
    Inherits FormMantenimiento

    Private ReadOnly logica As New InscripcionBLL()
    Private ReadOnly catalogo As New CatalogoBLL()

    Protected Overrides ReadOnly Property Titulo As String
        Get
            Return "Mantenimiento de inscripciones"
        End Get
    End Property

    Protected Overrides ReadOnly Property TipoEntidad As Type
        Get
            Return GetType(Inscripcion)
        End Get
    End Property

    Protected Overrides Function DefinirCampos() As List(Of CampoFormulario)
        Return New List(Of CampoFormulario) From {
            New CampoFormulario("IdEstudiante", "Estudiante", TipoCampo.Combo) With {.Opciones = catalogo.Estudiantes()},
            New CampoFormulario("IdGrupo", "Grupo", TipoCampo.Combo) With {.Opciones = catalogo.Grupos()},
            New CampoFormulario("Estado", "Estado", TipoCampo.ComboTexto) With {.Textos = New String() {"activa", "retirada", "finalizada"}}
        }
    End Function

    Protected Overrides Function ListarEntidades(filtro As String) As IList
        Return logica.Listar(filtro)
    End Function

    Protected Overrides Sub GuardarEntidad(entidad As Object)
        logica.Guardar(DirectCast(entidad, Inscripcion))
    End Sub

    Protected Overrides Sub EliminarEntidad(entidad As Object)
        logica.Eliminar(DirectCast(entidad, Inscripcion).IdInscripcion)
    End Sub
End Class
