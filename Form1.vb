Imports System.IO

Public Class UserLogin
  Public LoginName As String
  Private Sub Button1_Click(sender As Object, e As EventArgs) Handles OK.Click
    LoginName = ComboBox1.SelectedItem.ToString
    My.Computer.FileSystem.WriteAllText("LoginUser.txt", LoginName, False)
  End Sub

  Private Sub UserLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    ComboBox1.Items.Clear()
    Dim readText As String = My.Computer.FileSystem.ReadAllText("OperatorList.txt")
    Dim objStreamReader As StreamReader
    Dim strLine As String
    Dim i As Integer = 0
    'Pass the file path and the file name to the StreamReader constructor.
    objStreamReader = New StreamReader("OperatorList.txt")

    'Read the first line of text.
    strLine = objStreamReader.ReadLine

    'Continue to read until you reach the end of the file.
    Do While Not strLine Is Nothing
      ComboBox1.Items.Add(strLine)
      Console.WriteLine(strLine)
      strLine = objStreamReader.ReadLine
    Loop
  End Sub
End Class