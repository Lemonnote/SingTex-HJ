<Command("Unload", , , , "20"),
 TranslateCommand("zh-TW", "出布")>
Public NotInheritable Class Command33
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S33
    Off
    WaitCallAck
    WaitTime
    WaitCallAck2
  End Enum

  Public Wait As New Timer

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command10.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .TemperatureControl.Cancel()
      .Command17.Cancel() : .Command01.Cancel()

      .TempControlFlag = False
      ' .IO.CallLamp = True
      .MessageUnloadFiber = False
      State = S33.Off
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S33.Off
          .MessageUnloadFiber = False

        Case S33.WaitCallAck
          If Not .IO.CallAck Then Exit Select
          Wait.TimeRemaining = 5
          State = S33.WaitTime

        Case S33.WaitTime
          If Not Wait.Finished Then Exit Select
          State = S33.WaitCallAck2

        Case S33.WaitCallAck2
          If Not .IO.CallAck Then Exit Select
          .MessageUnloadFiber = False
          State = S33.Off
      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S33.Off
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
      Return State <> S33.Off
    End Get
  End Property
  Public ReadOnly Property IsCall() As Boolean
    Get
      Return State = S33.WaitCallAck
    End Get
  End Property


  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S33
  Public Property State() As S33
    Get
      Return state_
    End Get
    Private Set(ByVal value As S33)
      state_ = value
    End Set
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command33 As New Command33(Me)
End Class
#End Region
