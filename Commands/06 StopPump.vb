<Command("Stop Pump"), _
 TranslateCommand("zh-TW", "停止馬達")> _
Public NotInheritable Class Command06
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S06
    Off
    WaitStopReel
    WaitTime
  End Enum

  Public StateString As String
  Public Wait As New Timer

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      .Command10.Cancel() : .Command01.Cancel()
      .TemperatureControl.Cancel()
      .TemperatureControlFlag = False
      .ReelStopRequest = True
      Wait.TimeRemaining = .Parameters.ReelStopDelayTime
      State = S06.WaitStopReel
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S06.Off
          StateString = ""

        Case S06.WaitStopReel
          If Not Wait.Finished Then Exit Select
          .ReelStopRequest = False
          .PumpStopRequest = True
          Wait.TimeRemaining = 1
          State = S06.WaitTime

        Case S06.WaitTime
          If Not Wait.Finished Then Exit Select
          .PumpStopRequest = False
          ' .IO.PumpSpeedControl = 0
          .PumpOn = False
          State = S06.Off
      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S06.Off
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
      Return State <> S06.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S06
  Public Property State() As S06
    Get
      Return state_
    End Get
    Private Set(ByVal value As S06)
      state_ = value
    End Set
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command06 As New Command06(Me)
End Class
#End Region
