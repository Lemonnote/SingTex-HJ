<Command("Sample", , , , "5"), _
 TranslateCommand("zh-TW", "取樣通知")> _
Public NotInheritable Class Command20
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S20
    Off
    WaitCallAck
    WaitTime
    WaitCallAck2
  End Enum

  Public Wait As New Timer
  Public WaitRunning As New Timer

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .Command10.Cancel() : .Command01.Cancel()
      .Command17.Cancel() : .TemperatureControl.Cancel()
      .TempControlFlag = False
      .MessageTakeSample = True
      WaitRunning.TimeRemaining = .Parameters.WaitOverTime * 60
      State = S20.WaitCallAck
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S20.Off
          .MessageTakeSample = False

        Case S20.WaitCallAck
          If Not .IO.CallAck Then Exit Select
          Wait.TimeRemaining = 5
          State = S20.WaitTime

        Case S20.WaitTime
          If Not Wait.Finished Then Exit Select
          Wait.TimeRemaining = 600
          State = S20.WaitCallAck2

        Case S20.WaitCallAck2
          If Wait.Finished Then
            State = S20.WaitCallAck
          End If
          If Not .IO.CallAck Then Exit Select
          .MessageTakeSample = False
          State = S20.Off
          WaitRunning.Cancel()
      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S20.Off
    Wait.Cancel()
    WaitRunning.Cancel()
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
      Return State <> S20.Off
    End Get
  End Property
  Public ReadOnly Property IsCall() As Boolean
    Get
      Return State = S20.WaitCallAck
    End Get
  End Property


  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S20
  Public Property State() As S20
    Get
      Return state_
    End Get
    Private Set(ByVal value As S20)
      state_ = value
    End Set
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command20 As New Command20(Me)
End Class
#End Region
