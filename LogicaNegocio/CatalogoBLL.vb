Public Class CatalogoBLL
    Private ReadOnly datos As New CatalogoDAL()

    Public Function Roles() As List(Of ItemLista)
        Return datos.Roles()
    End Function

    Public Function Idiomas() As List(Of ItemLista)
        Return datos.Idiomas()
    End Function

    Public Function Niveles() As List(Of ItemLista)
        Return datos.Niveles()
    End Function

    Public Function MetodosPago() As List(Of ItemLista)
        Return datos.MetodosPago()
    End Function

    Public Function Cursos() As List(Of ItemLista)
        Return datos.Cursos()
    End Function

    Public Function Aulas() As List(Of ItemLista)
        Return datos.Aulas()
    End Function

    Public Function Docentes() As List(Of ItemLista)
        Return datos.Docentes()
    End Function

    Public Function Estudiantes() As List(Of ItemLista)
        Return datos.Estudiantes()
    End Function

    Public Function Grupos() As List(Of ItemLista)
        Return datos.Grupos()
    End Function

    Public Function Inscripciones() As List(Of ItemLista)
        Return datos.Inscripciones()
    End Function
End Class
