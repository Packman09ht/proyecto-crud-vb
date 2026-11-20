Imports System.ComponentModel

<DesignerCategory("Code")>
Public Class FrmLogin
    Inherits Form

    Private ReadOnly logica As New UsuarioBLL()
    Private ReadOnly txtUsuario As New TextBox()
    Private ReadOnly txtClave As New TextBox()

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        ConstruirInterfaz()
    End Sub

    Private Sub ConstruirInterfaz()
        Text = "Academia de Idiomas - Iniciar sesión"
        Font = New Font("Segoe UI", 10.0F)
        ClientSize = New Size(380, 360)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        StartPosition = FormStartPosition.CenterScreen
        BackColor = Color.White

        Dim titulo As New Label With {
            .Text = "Academia de Idiomas", .Font = New Font("Segoe UI", 17.0F, FontStyle.Bold),
            .ForeColor = Color.FromArgb(31, 78, 121), .AutoSize = True, .Location = New Point(40, 30)
        }
        Dim subtitulo As New Label With {
            .Text = "Inicia sesión para continuar", .ForeColor = Color.Gray, .AutoSize = True, .Location = New Point(42, 68)
        }
        Dim etiquetaUsuario As New Label With {.Text = "Usuario", .AutoSize = True, .Location = New Point(42, 110)}
        txtUsuario.Location = New Point(42, 134)
        txtUsuario.Width = 296
        Dim etiquetaClave As New Label With {.Text = "Contraseña", .AutoSize = True, .Location = New Point(42, 178)}
        txtClave.Location = New Point(42, 202)
        txtClave.Width = 296
        txtClave.UseSystemPasswordChar = True

        Dim entrar As New Button With {
            .Text = "Entrar", .Location = New Point(42, 256), .Size = New Size(296, 40), .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(31, 78, 121), .ForeColor = Color.White
        }
        AddHandler entrar.Click, AddressOf Entrar_Click
        Dim ayuda As New Label With {
            .Text = "Usuarios de prueba: admin, recepcion, docente, estudiante", .ForeColor = Color.Gray,
            .Font = New Font("Segoe UI", 8.5F), .AutoSize = True, .Location = New Point(42, 314)
        }

        Controls.AddRange(New Control() {titulo, subtitulo, etiquetaUsuario, txtUsuario, etiquetaClave, txtClave, entrar, ayuda})
        AcceptButton = entrar
    End Sub

    Private Sub Entrar_Click(remitente As Object, e As EventArgs)
        Try
            logica.Autenticar(txtUsuario.Text, txtClave.Text)
            DialogResult = DialogResult.OK
        Catch ex As ReglaNegocioException
            MessageBox.Show(ex.Message, "No se pudo iniciar sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtClave.Clear()
            txtClave.Focus()
        Catch ex As Exception
            MessageBox.Show("No se pudo conectar con la base de datos: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
