<Command("LiquidRatio", "LR |1-99| ", , , , CommandType.BatchParameter), _
 TranslateCommand("zh-TW", "浴比", "浴比 |1-99|")> _
Public NotInheritable Class Command22
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S22
    Off
  End Enum
  Public StateString As String
  Public Wait As New Timer

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      .TemperatureControlFlag = True
      .LiquidRatio = param(1)
      State = S22.Off
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S22.Off
          StateString = ""


      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S22.Off
    Wait.Cancel()
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged

  End Sub


#Region "Standard Definitions"
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S22.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S22
  Public Property State() As S22
    Get
      Return state_
    End Get
    Private Set(ByVal value As S22)
      state_ = value
    End Set
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command22 As New Command22(Me)
End Class
#End Region
