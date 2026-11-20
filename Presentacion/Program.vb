Module Program
    <STAThread>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Do
            Using inicio As New FrmLogin()
                If inicio.ShowDialog() <> DialogResult.OK Then Exit Do
            End Using
            Using principal As New FrmMenuPrincipal()
                principal.ShowDialog()
                If Not principal.CerroSesion Then Exit Do
            End Using
            SesionActual.Cerrar()
        Loop
    End Sub
End Module
