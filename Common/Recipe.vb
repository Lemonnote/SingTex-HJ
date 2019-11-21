Public Class Recipe
  'Recipe data... data should be in the following format in the column RecipeProducts
  '  step,product1Code,product1Name,grams;step,product2Code,product2Name,grams;step,product3Code,product3Name,grams etc...

  Public Code As String
  Public Name As String
  Public Products As List(Of Product)

  Public Function Load(ByVal dr As System.Data.DataRow) As Boolean
    Try
      'Reset data good flag
      DataGood = False

      'Fill in recipe header data
      Code = dr("RecipeCode").ToString
      Name = dr("RecipeName").ToString

      'Clear any existing rows
      Products = New List(Of Product)

      'Get the product string from the datarow
      Dim productString As String = dr("RecipeProducts").ToString

      'Split the product string into individual rows - product rows should be separated with ";"
      Dim productRows() As String = productString.Split(";".ToCharArray)

      'Make sure we have rows to add
      If productRows.Length <= 0 Then Return False

      'Create a row for each element in the productRows array
      For i As Integer = productRows.GetLowerBound(0) To productRows.GetUpperBound(0)
        Dim newProduct As New Product
        If newProduct.Load(productRows(i)) Then Products.Add(newProduct)
      Next

      'Set datagood to true if everything was read ok
      DataGood = True
      Return True
    Catch ex As Exception
      'TODO Log the error?
    End Try
    Return False
  End Function

  Private dataGood_ As Boolean = False
  Public Property DataGood() As Boolean
    Get
      Return dataGood_
    End Get
    Private Set(ByVal value As Boolean)
      dataGood_ = value
    End Set
  End Property


  Public Class Product
    'Product data... should be in the following format
    '  step,productCode,productName,grams

    Public StepNumber As Integer
    Public Code As String
    Public Name As String
    Public Grams As Double

    Public Function Load(ByVal rowString As String) As Boolean
      Try
        'Split the string into individual columns - columns should be separated with ","
        Dim columns() As String = rowString.Split(",".ToCharArray)

        'Update each colum checking array bounds as we go
        If columns.Length > 0 Then StepNumber = Integer.Parse(columns(0))
        If columns.Length > 1 Then Code = columns(1)
        If columns.Length > 2 Then Name = columns(2)
        If columns.Length > 3 Then Grams = Double.Parse(columns(3))

        Return True
      Catch ex As Exception
        'TODO Log Error ?
      End Try
      Return False
    End Function

    Public ReadOnly Property Amount() As String
      Get
        Select Case Grams
          Case Is > 1000
            Return (Grams / 1000).ToString("#0.00") & " kg"
          Case Else
            Return Grams.ToString("#0.00") & " g"
        End Select
      End Get
    End Property

    Public ReadOnly Property Text() As String
      Get
        Return Code & "" & Name & "  " & Amount
      End Get
    End Property
  End Class
End Class