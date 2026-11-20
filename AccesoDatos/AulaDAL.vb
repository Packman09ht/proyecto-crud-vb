Imports System.Data
Imports System.Linq

Public Class AulaDAL
    Private Shared Function Mapear(fila As DataRow) As Aula
        Return New Aula With {
            .IdAula = Lector.Entero(fila, "id_aula"),
            .Nombre = Lector.Texto(fila, "nombre"),
            .Capacidad = Lector.Entero(fila, "capacidad"),
            .Ubicacion = Lector.Texto(fila, "ubicacion"),
            .Equipamiento = Lector.Texto(fila, "equipamiento"),
            .Activa = Lector.Logico(fila, "activa"),
            .Observaciones = Lector.Texto(fila, "observaciones")
        }
    End Function

    Public Function Listar(filtro As String) As List(Of Aula)
        Dim tabla As DataTable = Conexion.Consultar(
            "SELECT id_aula, nombre, capacidad, ubicacion, equipamiento, activa, observaciones FROM aula " &
            "WHERE nombre LIKE @f OR ISNULL(ubicacion, '') LIKE @f ORDER BY nombre",
            Conexion.Parametro("@f", "%" & filtro & "%"))
        Return tabla.Rows.Cast(Of DataRow)().Select(Function(f) Mapear(f)).ToList()
    End Function

    Public Function ObtenerPorId(id As Integer) As Aula
        Dim tabla As DataTable = Conexion.Consultar(
            "SELECT id_aula, nombre, capacidad, ubicacion, equipamiento, activa, observaciones FROM aula WHERE id_aula = @id",
            Conexion.Parametro("@id", id))
        Return If(tabla.Rows.Count = 0, Nothing, Mapear(tabla.Rows(0)))
    End Function

    Public Function ExisteNombre(nombre As String, idExcluir As Integer) As Boolean
        Dim cantidad As Integer = Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM aula WHERE nombre = @n AND id_aula <> @id",
            Conexion.Parametro("@n", nombre), Conexion.Parametro("@id", idExcluir)))
        Return cantidad > 0
    End Function

    Public Function TieneGrupos(idAula As Integer) As Boolean
        Dim cantidad As Integer = Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM grupo WHERE id_aula = @id", Conexion.Parametro("@id", idAula)))
        Return cantidad > 0
    End Function

    Public Function Insertar(a As Aula) As Integer
        Return Convert.ToInt32(Conexion.Escalar(
            "INSERT INTO aula(nombre, capacidad, ubicacion, equipamiento, activa, observaciones) " &
            "VALUES(@n, @c, @u, @e, @a, @o); SELECT CAST(SCOPE_IDENTITY() AS INT)",
            Conexion.Parametro("@n", a.Nombre), Conexion.Parametro("@c", a.Capacidad),
            Conexion.ParametroOpcional("@u", a.Ubicacion), Conexion.ParametroOpcional("@e", a.Equipamiento),
            Conexion.Parametro("@a", a.Activa), Conexion.ParametroOpcional("@o", a.Observaciones)))
    End Function

    Public Sub Actualizar(a As Aula)
        Conexion.Ejecutar(
            "UPDATE aula SET nombre = @n, capacidad = @c, ubicacion = @u, equipamiento = @e, activa = @a, observaciones = @o WHERE id_aula = @id",
            Conexion.Parametro("@n", a.Nombre), Conexion.Parametro("@c", a.Capacidad),
            Conexion.ParametroOpcional("@u", a.Ubicacion), Conexion.ParametroOpcional("@e", a.Equipamiento),
            Conexion.Parametro("@a", a.Activa), Conexion.ParametroOpcional("@o", a.Observaciones),
            Conexion.Parametro("@id", a.IdAula))
    End Sub

    Public Sub Eliminar(id As Integer)
        Conexion.Ejecutar("DELETE FROM aula WHERE id_aula = @id", Conexion.Parametro("@id", id))
    End Sub
End Class
