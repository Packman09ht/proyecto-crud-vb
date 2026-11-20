Imports System.ComponentModel

<DesignerCategory("Code")>
Public Class FrmMenuPrincipal
    Inherits Form

    Private ReadOnly accesos As New FlowLayoutPanel()
    Private ReadOnly barraMenu As New MenuStrip()
    Private ReadOnly mantenimientos As New ToolStripMenuItem("Mantenimientos")

    Public Property CerroSesion As Boolean

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        ConstruirInterfaz()
    End Sub

    Private Sub ConstruirInterfaz()
        Text = "Academia de Idiomas"
        Font = New Font("Segoe UI", 10.0F)
        Size = New Size(960, 640)
        StartPosition = FormStartPosition.CenterScreen
        BackColor = Color.FromArgb(245, 247, 250)

        Dim usuario As Usuario = SesionActual.Usuario
        Dim nombre As String = If(String.IsNullOrEmpty(usuario.NombrePersona), usuario.Username, usuario.NombrePersona)
        Dim bienvenida As New Label With {
            .Text = "Hola, " & nombre, .Font = New Font("Segoe UI", 20.0F, FontStyle.Bold), .AutoSize = True,
            .ForeColor = Color.FromArgb(31, 78, 121), .Location = New Point(30, 50)
        }
        Dim rol As New Label With {.Text = "Rol: " & usuario.Rol & ". Elige qué quieres hacer.", .ForeColor = Color.Gray, .AutoSize = True, .Location = New Point(33, 96)}

        accesos.Location = New Point(30, 140)
        accesos.Size = New Size(880, 440)
        accesos.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
        accesos.AutoScroll = True

        Dim archivo As New ToolStripMenuItem("Archivo")
        Dim cerrarSesion As New ToolStripMenuItem("Cerrar sesión")
        AddHandler cerrarSesion.Click, AddressOf CerrarSesion_Click
        Dim salir As New ToolStripMenuItem("Salir")
        AddHandler salir.Click, Sub(remitente, argumentos) Close()
        archivo.DropDownItems.AddRange(New ToolStripItem() {cerrarSesion, salir})
        barraMenu.Items.Add(archivo)
        barraMenu.Items.Add(mantenimientos)

        AgregarAcceso("Estudiantes", "Registrar y consultar alumnos", Function() New FrmEstudiantes(), "Administrador", "Recepción")
        AgregarAcceso("Docentes", "Profesores de la academia", Function() New FrmDocentes(), "Administrador")
        AgregarAcceso("Cursos", "Idiomas, niveles y precios", Function() New FrmCursos(), "Administrador")
        AgregarAcceso("Aulas", "Espacios y capacidad", Function() New FrmAulas(), "Administrador")
        AgregarAcceso("Grupos", "Horarios, docentes y cupos", Function() New FrmGrupos(), "Administrador")
        AgregarAcceso("Inscripciones", "Matricular estudiantes", Function() New FrmInscripciones(), "Administrador", "Recepción")
        AgregarAcceso("Pagos", "Cobros y recibos", Function() New FrmPagos(), "Administrador", "Recepción")
        AgregarAcceso("Usuarios", "Accesos al sistema", Function() New FrmUsuarios(), "Administrador")
        AgregarAcceso("Dashboard", "Estadísticas y reportes", Function() New FrmDashboard())

        MainMenuStrip = barraMenu
        Controls.AddRange(New Control() {accesos, rol, bienvenida, barraMenu})
    End Sub

    Private Sub AgregarAcceso(texto As String, descripcion As String, fabrica As Func(Of Form), ParamArray roles As String())
        If roles.Length > 0 AndAlso Not roles.Contains(SesionActual.Rol) Then Return
        Dim abrir As Action = Sub()
                                  Using ventana As Form = fabrica()
                                      ventana.ShowDialog(Me)
                                  End Using
                              End Sub
        Dim boton As New Button With {
            .Text = texto & Environment.NewLine & descripcion, .Size = New Size(205, 96), .Margin = New Padding(8),
            .FlatStyle = FlatStyle.Flat, .BackColor = Color.White, .TextAlign = ContentAlignment.MiddleCenter
        }
        AddHandler boton.Click, Sub(remitente, argumentos) abrir()
        accesos.Controls.Add(boton)
        Dim opcion As New ToolStripMenuItem(texto)
        AddHandler opcion.Click, Sub(remitente, argumentos) abrir()
        mantenimientos.DropDownItems.Add(opcion)
    End Sub

    Private Sub CerrarSesion_Click(remitente As Object, e As EventArgs)
        CerroSesion = True
        Close()
    End Sub
End Class
