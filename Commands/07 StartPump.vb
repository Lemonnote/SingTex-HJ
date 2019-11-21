<Command("Start Pump", "Speed |1-100|%"), _
 TranslateCommand("zh-TW", "起動馬達", "運轉速度 |1-100|%")> _
Public NotInheritable Class Command07
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S07
    Off
    WaitMiddleLevel
    WaitTime
    WaitMainPumpFB
  End Enum

  Public StateString As String
  Public Wait As New Timer

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      .PumpSpeed = param(1)
      State = S07.WaitMiddleLevel
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S07.Off
          StateString = ""

        Case S07.WaitMiddleLevel
          If Not .IO.MiddleLevel Then Exit Select
          ' .IO.PumpSpeedControl = CType(.PumpSpeed * 10, Short)
          .PumpStartRequest = True
          Wait.TimeRemaining = 1
          State = S07.WaitTime

        Case S07.WaitTime
          If Not Wait.Finished Then Exit Select
          .PumpStartRequest = False
          .PumpOn = True
          State = S07.WaitMainPumpFB

        Case S07.WaitMainPumpFB
          If Not .IO.MainPumpFB Then Exit Select
          State = S07.Off
      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S07.Off
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
      Return State <> S07.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S07
  Public Property State() As S07
    Get
      Return state_
    End Get
    Private Set(ByVal value As S07)
      state_ = value
    End Set
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command07 As New Command07(Me)
End Class
#End Region
