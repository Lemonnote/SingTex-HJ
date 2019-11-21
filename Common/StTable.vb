Public NotInheritable Class St

    Private Shared phLookupTable_ As System.Data.DataTable
    Public Shared Property PhLookupTable() As System.Data.DataTable
        Get
            Return phLookupTable_
        End Get
        Set(ByVal value As System.Data.DataTable)
            phLookupTable_ = value
        End Set
    End Property

    Public Shared Function GetAmount(ByVal startPh As Integer, ByVal endPh As Integer) As Integer
        Try
            'make sure we hsve ph data
            If PhLookupTable Is Nothing OrElse PhLookupTable.Rows.Count <= 0 Then Return 0

            Dim startAmount As Integer = 0
            Dim endAmount As Integer = 0
            For Each row As System.Data.DataRow In PhLookupTable.Rows
                'Debug.Print(row("ph").ToString & "  " & row("amount").ToString)
                If Integer.Parse(row("St").ToString) = startPh Then startAmount = Integer.Parse(row("St2").ToString)
                If Integer.Parse(row("St").ToString) = endPh Then endAmount = Integer.Parse(row("St2").ToString)
            Next row

            Return endAmount - startAmount
        Catch ex As Exception
            'ignore errors
        End Try
        Return 0
    End Function

    Public Shared Function GetAmount1(ByVal startPh As Integer) As Integer
        Try
            'make sure we hsve ph data
            If PhLookupTable Is Nothing OrElse PhLookupTable.Rows.Count <= 0 Then Return 0

            Dim startAmount As Integer = 0

            For Each row As System.Data.DataRow In PhLookupTable.Rows
                'Debug.Print(row("ph").ToString & "  " & row("amount").ToString)
                If Integer.Parse(row("St2").ToString) > startPh Then
                    startAmount = Integer.Parse(row("St").ToString)
                    GoTo PH
                End If

                'If Integer.Parse(row("amount").ToString) = startPh Then endAmount = Integer.Parse(row("ph").ToString)
            Next row
PH:
            Return startAmount
        Catch ex As Exception
            'ignore errors
        End Try
        Return 0
    End Function

    Public Shared Sub Load()
        'Load the ph.xml file
        Try
            'Get application and file path
            Dim appPath As String = My.Application.Info.DirectoryPath
            Dim filePath As String = appPath & "\St.xml"

            'If the file exists load the lookup table
            If System.IO.File.Exists(filePath) Then
                'Read the settings into a dataset
                Dim data As New System.Data.DataSet
                data.ReadXml(filePath)

                'Use the first table in the dataset - should only be one table actually...
                If data.Tables IsNot Nothing AndAlso data.Tables.Count > 0 Then
                    PhLookupTable = data.Tables(0)
                End If
            End If

        Catch ex As Exception
            'Ignore errors
        End Try
    End Sub

End Class
