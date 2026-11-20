Imports System.Data
Imports System.Linq

Public Class CursoDAL
    Private Const ConsultaBase As String =
        "SELECT c.id_curso, c.id_idioma, c.id_nivel, i.nombre AS idioma, n.codigo AS nivel, c.nombre, c.duracion_horas, c.precio " &
        "FROM curso c JOIN idioma i ON i.id_idioma = c.id_idioma JOIN nivel n ON n.id_nivel = c.id_nivel"

    Private Shared Function Mapear(fila As DataRow) As Curso
        Return New Curso With {
            .IdCurso = Lector.Entero(fila, "id_curso"),
            .IdIdioma = Lector.Entero(fila, "id_idioma"),
            .IdNivel = Lector.Entero(fila, "id_nivel"),
            .Idioma = Lector.Texto(fila, "idioma"),
            .Nivel = Lector.Texto(fila, "nivel"),
            .Nombre = Lector.Texto(fila, "nombre"),
            .DuracionHoras = Lector.Entero(fila, "duracion_horas"),
            .Precio = Lector.Monto(fila, "precio")
        }
    End Function

    Public Function Listar(filtro As String) As List(Of Curso)
        Dim tabla As DataTable = Conexion.Consultar(
            ConsultaBase & " WHERE c.nombre LIKE @f OR i.nombre LIKE @f ORDER BY c.nombre",
            Conexion.Parametro("@f", "%" & filtro & "%"))
        Return tabla.Rows.Cast(Of DataRow)().Select(Function(f) Mapear(f)).ToList()
    End Function

    Public Function TieneGrupos(idCurso As Integer) As Boolean
        Dim cantidad As Integer = Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM grupo WHERE id_curso = @id", Conexion.Parametro("@id", idCurso)))
        Return cantidad > 0
    End Function

    Public Function Insertar(c As Curso) As Integer
        Return Convert.ToInt32(Conexion.Escalar(
            "INSERT INTO curso(id_idioma, id_nivel, nombre, duracion_horas, precio) VALUES(@i, @n, @nom, @h, @p); SELECT CAST(SCOPE_IDENTITY() AS INT)",
            Conexion.Parametro("@i", c.IdIdioma), Conexion.Parametro("@n", c.IdNivel), Conexion.Parametro("@nom", c.Nombre),
            Conexion.Parametro("@h", c.DuracionHoras), Conexion.Parametro("@p", c.Precio)))
    End Function

    Public Sub Actualizar(c As Curso)
        Conexion.Ejecutar(
            "UPDATE curso SET id_idioma = @i, id_nivel = @n, nombre = @nom, duracion_horas = @h, precio = @p WHERE id_curso = @id",
            Conexion.Parametro("@i", c.IdIdioma), Conexion.Parametro("@n", c.IdNivel), Conexion.Parametro("@nom", c.Nombre),
            Conexion.Parametro("@h", c.DuracionHoras), Conexion.Parametro("@p", c.Precio), Conexion.Parametro("@id", c.IdCurso))
    End Sub

    Public Sub Eliminar(id As Integer)
        Conexion.Ejecutar("DELETE FROM curso WHERE id_curso = @id", Conexion.Parametro("@id", id))
    End Sub
End Class
