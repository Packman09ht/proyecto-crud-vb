Imports System.Data
Imports System.Linq

Public Class CatalogoDAL
    Private Shared Function Cargar(sql As String) As List(Of ItemLista)
        Dim tabla As DataTable = Conexion.Consultar(sql)
        Return tabla.Rows.Cast(Of DataRow)().Select(Function(f) New ItemLista(Lector.Entero(f, "valor"), Lector.Texto(f, "texto"))).ToList()
    End Function

    Public Function Roles() As List(Of ItemLista)
        Return Cargar("SELECT id_rol AS valor, nombre AS texto FROM rol ORDER BY nombre")
    End Function

    Public Function Idiomas() As List(Of ItemLista)
        Return Cargar("SELECT id_idioma AS valor, nombre AS texto FROM idioma ORDER BY nombre")
    End Function

    Public Function Niveles() As List(Of ItemLista)
        Return Cargar("SELECT id_nivel AS valor, codigo + ' - ' + ISNULL(descripcion, '') AS texto FROM nivel ORDER BY codigo")
    End Function

    Public Function MetodosPago() As List(Of ItemLista)
        Return Cargar("SELECT id_metodo AS valor, nombre AS texto FROM metodo_pago ORDER BY nombre")
    End Function

    Public Function Cursos() As List(Of ItemLista)
        Return Cargar("SELECT id_curso AS valor, nombre AS texto FROM curso ORDER BY nombre")
    End Function

    Public Function Aulas() As List(Of ItemLista)
        Return Cargar("SELECT id_aula AS valor, nombre AS texto FROM aula WHERE activa = 1 ORDER BY nombre")
    End Function

    Public Function Docentes() As List(Of ItemLista)
        Return Cargar("SELECT d.id_docente AS valor, p.nombres + ' ' + p.apellidos AS texto FROM docente d JOIN persona p ON p.id_persona = d.id_persona ORDER BY p.nombres")
    End Function

    Public Function Estudiantes() As List(Of ItemLista)
        Return Cargar("SELECT e.id_estudiante AS valor, p.nombres + ' ' + p.apellidos AS texto FROM estudiante e JOIN persona p ON p.id_persona = e.id_persona WHERE e.estado = 'activo' ORDER BY p.nombres")
    End Function

    Public Function Grupos() As List(Of ItemLista)
        Return Cargar("SELECT g.id_grupo AS valor, c.nombre + ' (grupo ' + CAST(g.id_grupo AS VARCHAR(10)) + ')' AS texto FROM grupo g JOIN curso c ON c.id_curso = g.id_curso WHERE g.estado <> 'cerrado' ORDER BY c.nombre")
    End Function

    Public Function Inscripciones() As List(Of ItemLista)
        Return Cargar("SELECT i.id_inscripcion AS valor, p.nombres + ' ' + p.apellidos + ' - ' + c.nombre AS texto " &
                      "FROM inscripcion i JOIN estudiante e ON e.id_estudiante = i.id_estudiante JOIN persona p ON p.id_persona = e.id_persona " &
                      "JOIN grupo g ON g.id_grupo = i.id_grupo JOIN curso c ON c.id_curso = g.id_curso WHERE i.estado = 'activa' ORDER BY p.nombres")
    End Function
End Class
