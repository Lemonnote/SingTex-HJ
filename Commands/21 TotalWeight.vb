<Command("TotalWeight", "Weight |1-999| kg", , , , CommandType.BatchParameter), _
 TranslateCommand("zh-TW", "布重", "布重 |1-999|")> _
Public NotInheritable Class Command21
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S21
    Off
  End Enum

  Public StateString As String
  Public Wait As New Timer

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode

      .TemperatureControlFlag = True
      .TotalWeight = param(1)

    End With
  End Function
  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S21.Off
          .MessageTakeSample = False

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S21.Off
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
      Return State <> S21.Off
    End Get
  End Property
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S21
  Public Property State() As S21
    Get
      Return state_
    End Get
    Private Set(ByVal value As S21)
      state_ = value
    End Set
  End Property

#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command21 As New Command21(Me)
End Class
#End Region
