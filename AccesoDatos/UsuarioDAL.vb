Imports System.Data
Imports System.Linq

Public Class UsuarioDAL
    Private Const ConsultaBase As String =
        "SELECT u.id_usuario, u.username, u.contrasena_hash, u.id_rol, r.nombre AS rol, u.activo, " &
        "ISNULL(p.nombres + ' ' + p.apellidos, '') AS nombre_persona " &
        "FROM usuario u JOIN rol r ON r.id_rol = u.id_rol LEFT JOIN persona p ON p.id_usuario = u.id_usuario"

    Private Shared Function Mapear(fila As DataRow) As Usuario
        Return New Usuario With {
            .IdUsuario = Lector.Entero(fila, "id_usuario"),
            .Username = Lector.Texto(fila, "username"),
            .ContrasenaHash = Lector.Texto(fila, "contrasena_hash"),
            .IdRol = Lector.Entero(fila, "id_rol"),
            .Rol = Lector.Texto(fila, "rol"),
            .Activo = Lector.Logico(fila, "activo"),
            .NombrePersona = Lector.Texto(fila, "nombre_persona")
        }
    End Function

    Public Function Listar(filtro As String) As List(Of Usuario)
        Dim tabla As DataTable = Conexion.Consultar(
            ConsultaBase & " WHERE u.username LIKE @f OR ISNULL(p.nombres, '') LIKE @f ORDER BY u.username",
            Conexion.Parametro("@f", "%" & filtro & "%"))
        Return tabla.Rows.Cast(Of DataRow)().Select(Function(f) Mapear(f)).ToList()
    End Function

    Public Function ObtenerPorUsername(username As String) As Usuario
        Dim tabla As DataTable = Conexion.Consultar(ConsultaBase & " WHERE u.username = @u", Conexion.Parametro("@u", username))
        Return If(tabla.Rows.Count = 0, Nothing, Mapear(tabla.Rows(0)))
    End Function

    Public Function ObtenerPorId(id As Integer) As Usuario
        Dim tabla As DataTable = Conexion.Consultar(ConsultaBase & " WHERE u.id_usuario = @id", Conexion.Parametro("@id", id))
        Return If(tabla.Rows.Count = 0, Nothing, Mapear(tabla.Rows(0)))
    End Function

    Public Function ExisteUsername(username As String, idExcluir As Integer) As Boolean
        Dim cantidad As Integer = Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM usuario WHERE username = @u AND id_usuario <> @id",
            Conexion.Parametro("@u", username), Conexion.Parametro("@id", idExcluir)))
        Return cantidad > 0
    End Function

    Public Function Insertar(u As Usuario) As Integer
        Return Convert.ToInt32(Conexion.Escalar(
            "INSERT INTO usuario(username, contrasena_hash, id_rol, activo) VALUES(@u, @h, @r, @a); SELECT CAST(SCOPE_IDENTITY() AS INT)",
            Conexion.Parametro("@u", u.Username), Conexion.Parametro("@h", u.ContrasenaHash),
            Conexion.Parametro("@r", u.IdRol), Conexion.Parametro("@a", u.Activo)))
    End Function

    Public Sub Actualizar(u As Usuario)
        If String.IsNullOrEmpty(u.ContrasenaHash) Then
            Conexion.Ejecutar("UPDATE usuario SET username = @u, id_rol = @r, activo = @a WHERE id_usuario = @id",
                Conexion.Parametro("@u", u.Username), Conexion.Parametro("@r", u.IdRol),
                Conexion.Parametro("@a", u.Activo), Conexion.Parametro("@id", u.IdUsuario))
        Else
            Conexion.Ejecutar("UPDATE usuario SET username = @u, contrasena_hash = @h, id_rol = @r, activo = @a WHERE id_usuario = @id",
                Conexion.Parametro("@u", u.Username), Conexion.Parametro("@h", u.ContrasenaHash),
                Conexion.Parametro("@r", u.IdRol), Conexion.Parametro("@a", u.Activo), Conexion.Parametro("@id", u.IdUsuario))
        End If
    End Sub

    Public Sub Eliminar(id As Integer)
        Conexion.Ejecutar("DELETE FROM usuario WHERE id_usuario = @id", Conexion.Parametro("@id", id))
    End Sub
End Class
