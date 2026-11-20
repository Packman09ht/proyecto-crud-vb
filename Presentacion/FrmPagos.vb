Imports System.Collections
Imports System.ComponentModel

<DesignerCategory("Code")>
Public Class FrmPagos
    Inherits FormMantenimiento

    Private ReadOnly logica As New PagoBLL()
    Private ReadOnly catalogo As New CatalogoBLL()

    Protected Overrides ReadOnly Property Titulo As String
        Get
            Return "Mantenimiento de pagos"
        End Get
    End Property

    Protected Overrides ReadOnly Property TipoEntidad As Type
        Get
            Return GetType(Pago)
        End Get
    End Property

    Protected Overrides Function DefinirCampos() As List(Of CampoFormulario)
        Return New List(Of CampoFormulario) From {
            New CampoFormulario("IdInscripcion", "Inscripción", TipoCampo.Combo) With {.Opciones = catalogo.Inscripciones()},
            New CampoFormulario("IdMetodo", "Método de pago", TipoCampo.Combo) With {.Opciones = catalogo.MetodosPago()},
            New CampoFormulario("FechaPago", "Fecha del pago", TipoCampo.Fecha),
            New CampoFormulario("Monto", "Monto (RD$)", TipoCampo.Moneda) With {.Minimo = 0D, .Maximo = 1000000D},
            New CampoFormulario("Concepto", "Concepto", TipoCampo.ComboTexto) With {.Textos = New String() {"Mensualidad", "Matrícula", "Otro"}}
        }
    End Function

    Protected Overrides Function ListarEntidades(filtro As String) As IList
        Return logica.Listar(filtro)
    End Function

    Protected Overrides Sub GuardarEntidad(entidad As Object)
        logica.Guardar(DirectCast(entidad, Pago))
    End Sub

    Protected Overrides Sub EliminarEntidad(entidad As Object)
        logica.Eliminar(DirectCast(entidad, Pago).IdPago)
    End Sub
End Class
