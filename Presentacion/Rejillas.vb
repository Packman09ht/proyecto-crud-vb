Imports System.Text.RegularExpressions

Friend NotInheritable Class Rejillas
    Private Sub New()
    End Sub

    Public Shared Sub Preparar(rejilla As DataGridView)
        rejilla.Dock = DockStyle.Fill
        rejilla.ReadOnly = True
        rejilla.AllowUserToAddRows = False
        rejilla.AllowUserToDeleteRows = False
        rejilla.MultiSelect = False
        rejilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        rejilla.RowHeadersVisible = False
        rejilla.BackgroundColor = Color.White
        rejilla.BorderStyle = BorderStyle.None
        rejilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
        rejilla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 78, 121)
        rejilla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        rejilla.EnableHeadersVisualStyles = False
    End Sub

    Public Shared Sub AjustarColumnas(rejilla As DataGridView)
        For Each columna As DataGridViewColumn In rejilla.Columns
            columna.HeaderText = Regex.Replace(columna.Name, "(?<=[a-z])(?=[A-Z])", " ")
            If Regex.IsMatch(columna.Name, "^Id[A-Z]") OrElse columna.Name.StartsWith("Contrasena") Then
                columna.Visible = False
            End If
            If columna.ValueType Is GetType(Date) Then
                columna.DefaultCellStyle.Format = "dd/MM/yyyy"
            ElseIf columna.ValueType Is GetType(Decimal) Then
                columna.DefaultCellStyle.Format = "N2"
                columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
        Next
    End Sub
End Class
