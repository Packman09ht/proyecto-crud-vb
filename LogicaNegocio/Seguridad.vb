Imports System.Security.Cryptography
Imports System.Text

Public NotInheritable Class Seguridad
    Private Sub New()
    End Sub

    Public Shared Function CalcularHash(texto As String) As String
        Using sha As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha.ComputeHash(Encoding.UTF8.GetBytes(texto))
            Return BitConverter.ToString(bytes).Replace("-", String.Empty)
        End Using
    End Function
End Class
