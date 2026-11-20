Imports System.Data
Imports System.Linq

Public Class DocenteDAL
    Private Const ConsultaBase As String =
        "SELECT d.id_docente, d.id_persona, p.cedula, p.nombres, p.apellidos, p.fecha_nacimiento, " &
        "p.telefono, p.correo, p.direccion, d.especialidad, d.fecha_contratacion " &
        "FROM docente d JOIN persona p ON p.id_persona = d.id_persona"

    Private Shared Function Mapear(fila As DataRow) As Docente
        Return New Docente With {
            .IdDocente = Lector.Entero(fila, "id_docente"),
            .IdPersona = Lector.Entero(fila, "id_persona"),
            .Cedula = Lector.Texto(fila, "cedula"),
            .Nombres = Lector.Texto(fila, "nombres"),
            .Apellidos = Lector.Texto(fila, "apellidos"),
            .FechaNacimiento = Lector.Fecha(fila, "fecha_nacimiento"),
            .Telefono = Lector.Texto(fila, "telefono"),
            .Correo = Lector.Texto(fila, "correo"),
            .Direccion = Lector.Texto(fila, "direccion"),
            .Especialidad = Lector.Texto(fila, "especialidad"),
            .FechaContratacion = Lector.Fecha(fila, "fecha_contratacion")
        }
    End Function

    Public Function Listar(filtro As String) As List(Of Docente)
        Dim tabla As DataTable = Conexion.Consultar(
            ConsultaBase & " WHERE p.nombres + ' ' + p.apellidos LIKE @f OR ISNULL(d.especialidad, '') LIKE @f ORDER BY p.nombres, p.apellidos",
            Conexion.Parametro("@f", "%" & filtro & "%"))
        Return tabla.Rows.Cast(Of DataRow)().Select(Function(f) Mapear(f)).ToList()
    End Function

    Public Function TieneGrupos(idDocente As Integer) As Boolean
        Dim cantidad As Integer = Convert.ToInt32(Conexion.Escalar(
            "SELECT COUNT(*) FROM grupo WHERE id_docente = @id", Conexion.Parametro("@id", idDocente)))
        Return cantidad > 0
    End Function

    Public Function Insertar(d As Docente) As Integer
        Dim idDocente As Integer
        Conexion.EnTransaccion(
            Sub(cn, tx)
                Dim idPersona As Integer = Convert.ToInt32(Conexion.EscalarEn(cn, tx,
                    "INSERT INTO persona(cedula, nombres, apellidos, fecha_nacimiento, telefono, correo, direccion) " &
                    "VALUES(@ced, @nom, @ape, @nac, @tel, @cor, @dir); SELECT CAST(SCOPE_IDENTITY() AS INT)",
                    Conexion.ParametroOpcional("@ced", d.Cedula), Conexion.Parametro("@nom", d.Nombres),
                    Conexion.Parametro("@ape", d.Apellidos), Conexion.Parametro("@nac", d.FechaNacimiento),
                    Conexion.ParametroOpcional("@tel", d.Telefono), Conexion.ParametroOpcional("@cor", d.Correo),
                    Conexion.ParametroOpcional("@dir", d.Direccion)))
                idDocente = Convert.ToInt32(Conexion.EscalarEn(cn, tx,
                    "INSERT INTO docente(id_persona, especialidad, fecha_contratacion) VALUES(@per, @esp, @con); SELECT CAST(SCOPE_IDENTITY() AS INT)",
                    Conexion.Parametro("@per", idPersona), Conexion.ParametroOpcional("@esp", d.Especialidad),
                    Conexion.Parametro("@con", d.FechaContratacion)))
            End Sub)
        Return idDocente
    End Function

    Public Sub Actualizar(d As Docente)
        Conexion.EnTransaccion(
            Sub(cn, tx)
                Conexion.EjecutarEn(cn, tx,
                    "UPDATE persona SET cedula = @ced, nombres = @nom, apellidos = @ape, fecha_nacimiento = @nac, " &
                    "telefono = @tel, correo = @cor, direccion = @dir WHERE id_persona = @id",
                    Conexion.ParametroOpcional("@ced", d.Cedula), Conexion.Parametro("@nom", d.Nombres),
                    Conexion.Parametro("@ape", d.Apellidos), Conexion.Parametro("@nac", d.FechaNacimiento),
                    Conexion.ParametroOpcional("@tel", d.Telefono), Conexion.ParametroOpcional("@cor", d.Correo),
                    Conexion.ParametroOpcional("@dir", d.Direccion), Conexion.Parametro("@id", d.IdPersona))
                Conexion.EjecutarEn(cn, tx,
                    "UPDATE docente SET especialidad = @esp, fecha_contratacion = @con WHERE id_docente = @id",
                    Conexion.ParametroOpcional("@esp", d.Especialidad), Conexion.Parametro("@con", d.FechaContratacion),
                    Conexion.Parametro("@id", d.IdDocente))
            End Sub)
    End Sub

    Public Sub Eliminar(idDocente As Integer, idPersona As Integer)
        Conexion.EnTransaccion(
            Sub(cn, tx)
                Conexion.EjecutarEn(cn, tx, "DELETE FROM docente WHERE id_docente = @id", Conexion.Parametro("@id", idDocente))
                Conexion.EjecutarEn(cn, tx, "DELETE FROM persona WHERE id_persona = @id", Conexion.Parametro("@id", idPersona))
            End Sub)
    End Sub
End Class
