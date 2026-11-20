Imports System.Collections
Imports System.ComponentModel

<DesignerCategory("Code")>
Public Class FrmGrupos
    Inherits FormMantenimiento

    Private ReadOnly logica As New GrupoBLL()
    Private ReadOnly catalogo As New CatalogoBLL()

    Protected Overrides ReadOnly Property Titulo As String
        Get
            Return "Mantenimiento de grupos"
        End Get
    End Property

    Protected Overrides ReadOnly Property TipoEntidad As Type
        Get
            Return GetType(Grupo)
        End Get
    End Property

    Protected Overrides Function DefinirCampos() As List(Of CampoFormulario)
        Return New List(Of CampoFormulario) From {
            New CampoFormulario("IdCurso", "Curso", TipoCampo.Combo) With {.Opciones = catalogo.Cursos()},
            New CampoFormulario("IdDocente", "Docente", TipoCampo.Combo) With {.Opciones = catalogo.Docentes()},
            New CampoFormulario("IdAula", "Aula", TipoCampo.Combo) With {.Opciones = catalogo.Aulas()},
            New CampoFormulario("CupoMaximo", "Cupo máximo", TipoCampo.Entero) With {.Minimo = 1D, .Maximo = 200D},
            New CampoFormulario("Dias", "Días de clase", TipoCampo.Dias),
            New CampoFormulario("HoraInicio", "Hora de inicio (HH:mm)", TipoCampo.Texto),
            New CampoFormulario("HoraFin", "Hora de fin (HH:mm)", TipoCampo.Texto),
            New CampoFormulario("FechaInicio", "Fecha de inicio", TipoCampo.Fecha),
            New CampoFormulario("FechaFin", "Fecha de fin", TipoCampo.Fecha),
            New CampoFormulario("Estado", "Estado", TipoCampo.ComboTexto) With {.Textos = New String() {"abierto", "en curso", "cerrado"}}
        }
    End Function

    Protected Overrides Function ListarEntidades(filtro As String) As IList
        Return logica.Listar(filtro)
    End Function

    Protected Overrides Sub GuardarEntidad(entidad As Object)
        logica.Guardar(DirectCast(entidad, Grupo))
    End Sub

    Protected Overrides Sub EliminarEntidad(entidad As Object)
        logica.Eliminar(DirectCast(entidad, Grupo).IdGrupo)
    End Sub
End Class
