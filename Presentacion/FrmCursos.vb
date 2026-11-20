Imports System.Collections
Imports System.ComponentModel

<DesignerCategory("Code")>
Public Class FrmCursos
    Inherits FormMantenimiento

    Private ReadOnly logica As New CursoBLL()
    Private ReadOnly catalogo As New CatalogoBLL()

    Protected Overrides ReadOnly Property Titulo As String
        Get
            Return "Mantenimiento de cursos"
        End Get
    End Property

    Protected Overrides ReadOnly Property TipoEntidad As Type
        Get
            Return GetType(Curso)
        End Get
    End Property

    Protected Overrides Function DefinirCampos() As List(Of CampoFormulario)
        Return New List(Of CampoFormulario) From {
            New CampoFormulario("IdIdioma", "Idioma", TipoCampo.Combo) With {.Opciones = catalogo.Idiomas()},
            New CampoFormulario("IdNivel", "Nivel", TipoCampo.Combo) With {.Opciones = catalogo.Niveles()},
            New CampoFormulario("Nombre", "Nombre del curso", TipoCampo.Texto),
            New CampoFormulario("DuracionHoras", "Duración (horas)", TipoCampo.Entero) With {.Minimo = 1D, .Maximo = 1000D},
            New CampoFormulario("Precio", "Precio (RD$)", TipoCampo.Moneda) With {.Maximo = 1000000D}
        }
    End Function

    Protected Overrides Function ListarEntidades(filtro As String) As IList
        Return logica.Listar(filtro)
    End Function

    Protected Overrides Sub GuardarEntidad(entidad As Object)
        logica.Guardar(DirectCast(entidad, Curso))
    End Sub

    Protected Overrides Sub EliminarEntidad(entidad As Object)
        logica.Eliminar(DirectCast(entidad, Curso).IdCurso)
    End Sub
End Class
