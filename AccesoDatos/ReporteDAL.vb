Imports System.Data

Public Class ReporteDAL
    Public Function Resumen() As DataTable
        Return Conexion.Consultar(
            "SELECT " &
            "(SELECT COUNT(*) FROM estudiante WHERE estado = 'activo') AS estudiantes_activos, " &
            "(SELECT COUNT(*) FROM docente) AS docentes, " &
            "(SELECT COUNT(*) FROM grupo WHERE estado <> 'cerrado') AS grupos_abiertos, " &
            "(SELECT COUNT(*) FROM inscripcion WHERE estado = 'activa') AS inscripciones_activas, " &
            "(SELECT ISNULL(SUM(monto), 0) FROM pago) AS ingresos_totales, " &
            "(SELECT ISNULL(SUM(c.precio), 0) FROM inscripcion i JOIN grupo g ON g.id_grupo = i.id_grupo JOIN curso c ON c.id_curso = g.id_curso WHERE i.estado = 'activa') " &
            "- (SELECT ISNULL(SUM(pg.monto), 0) FROM pago pg JOIN inscripcion i ON i.id_inscripcion = pg.id_inscripcion WHERE i.estado = 'activa') AS por_cobrar")
    End Function

    Public Function Morosos() As DataTable
        Return Conexion.Consultar(
            "SELECT p.nombres + ' ' + p.apellidos AS Estudiante, c.nombre AS Curso, c.precio AS Precio, " &
            "ISNULL(SUM(pg.monto), 0) AS Pagado, c.precio - ISNULL(SUM(pg.monto), 0) AS Debe " &
            "FROM inscripcion i JOIN estudiante e ON e.id_estudiante = i.id_estudiante JOIN persona p ON p.id_persona = e.id_persona " &
            "JOIN grupo g ON g.id_grupo = i.id_grupo JOIN curso c ON c.id_curso = g.id_curso " &
            "LEFT JOIN pago pg ON pg.id_inscripcion = i.id_inscripcion WHERE i.estado = 'activa' " &
            "GROUP BY p.nombres, p.apellidos, c.nombre, c.precio HAVING c.precio - ISNULL(SUM(pg.monto), 0) > 0 ORDER BY Debe DESC")
    End Function

    Public Function IngresosPorMetodo() As DataTable
        Return Conexion.Consultar(
            "SELECT m.nombre AS Metodo, COUNT(pg.id_pago) AS Pagos, ISNULL(SUM(pg.monto), 0) AS Total " &
            "FROM metodo_pago m LEFT JOIN pago pg ON pg.id_metodo = m.id_metodo GROUP BY m.nombre ORDER BY Total DESC")
    End Function

    Public Function OcupacionPorGrupo() As DataTable
        Return Conexion.Consultar(
            "SELECT c.nombre AS Curso, g.id_grupo AS Grupo, g.cupo_maximo AS Cupo, " &
            "(SELECT COUNT(*) FROM inscripcion i WHERE i.id_grupo = g.id_grupo AND i.estado = 'activa') AS Inscritos " &
            "FROM grupo g JOIN curso c ON c.id_curso = g.id_curso WHERE g.estado <> 'cerrado' ORDER BY c.nombre")
    End Function
End Class
