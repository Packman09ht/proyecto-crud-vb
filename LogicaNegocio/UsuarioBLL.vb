Public Class UsuarioBLL
    Private ReadOnly datos As New UsuarioDAL()

    Public Function Autenticar(username As String, clave As String) As Usuario
        Validador.Exigir(Not String.IsNullOrWhiteSpace(username) AndAlso Not String.IsNullOrEmpty(clave), "Escribe tu usuario y tu contraseña.")
        Dim encontrado As Usuario = datos.ObtenerPorUsername(username.Trim())
        Validador.Exigir(encontrado IsNot Nothing AndAlso encontrado.ContrasenaHash = Seguridad.CalcularHash(clave), "Usuario o contraseña incorrectos.")
        Validador.Exigir(encontrado.Activo, "Este usuario está desactivado.")
        SesionActual.Usuario = encontrado
        Return encontrado
    End Function

    Public Function Listar(filtro As String) As List(Of Usuario)
        Return datos.Listar(filtro)
    End Function

    Public Sub Guardar(u As Usuario)
        Validador.Exigir(Not String.IsNullOrWhiteSpace(u.Username), "El nombre de usuario es obligatorio.")
        Validador.Exigir(u.IdRol > 0, "Elige un rol.")
        Validador.Exigir(Not datos.ExisteUsername(u.Username.Trim(), u.IdUsuario), "Ese nombre de usuario ya existe.")
        u.Username = u.Username.Trim()
        If u.IdUsuario = 0 Then
            Validador.Exigir(Not String.IsNullOrEmpty(u.Contrasena) AndAlso u.Contrasena.Length >= 6, "La contraseña debe tener al menos 6 caracteres.")
        Else
            Validador.Exigir(String.IsNullOrEmpty(u.Contrasena) OrElse u.Contrasena.Length >= 6, "La nueva contraseña debe tener al menos 6 caracteres.")
        End If
        u.ContrasenaHash = If(String.IsNullOrEmpty(u.Contrasena), String.Empty, Seguridad.CalcularHash(u.Contrasena))
        If u.IdUsuario = 0 Then
            datos.Insertar(u)
        Else
            Validador.Exigir(Not (SesionActual.Usuario IsNot Nothing AndAlso SesionActual.Usuario.IdUsuario = u.IdUsuario AndAlso Not u.Activo), "No puedes desactivar tu propio usuario.")
            datos.Actualizar(u)
        End If
    End Sub

    Public Sub Eliminar(id As Integer)
        Validador.Exigir(SesionActual.Usuario Is Nothing OrElse SesionActual.Usuario.IdUsuario <> id, "No puedes eliminar tu propio usuario.")
        Validador.EjecutarProtegido(Sub() datos.Eliminar(id), "No se puede eliminar: el usuario está asociado a una persona. Desactívalo en su lugar.")
    End Sub
End Class
