Public Enum TipoCampo
    Texto
    TextoLargo
    Contrasena
    Entero
    Moneda
    Fecha
    Combo
    ComboTexto
    Logico
    Dias
End Enum

Public Class CampoFormulario
    Public Property Propiedad As String
    Public Property Etiqueta As String
    Public Property Tipo As TipoCampo
    Public Property Opciones As List(Of ItemLista) = New List(Of ItemLista)()
    Public Property Textos As String() = New String() {}
    Public Property Minimo As Decimal = 0D
    Public Property Maximo As Decimal = 1000000D

    Public Sub New(propiedad As String, etiqueta As String, tipo As TipoCampo)
        Me.Propiedad = propiedad
        Me.Etiqueta = etiqueta
        Me.Tipo = tipo
    End Sub
End Class
