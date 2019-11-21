<Command("Drain", "Drain Type |0-1|", , "30", "5"), _
 TranslateCommand("zh-TW", "主缸排水", "水管選擇 |0-2|"), _
 Description("2=HOT+COLD,1=HOT 0=COLD"), _
 TranslateDescription("zh-TW", "2=熱水+冷水,1=熱水 0=冷水")> _
Public NotInheritable Class Command14
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S14
    Off
    WaitStopReel
    WaitTime
    WaitTempSafe
    WaitDrain
    WaitTime5
    WaitNotEntanglement2
    WaitTime6
    Pause
  End Enum

  Public Wait As New Timer
  Public StateString As String

  Public DrainType As Integer
  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command10.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .TemperatureControl.Cancel()
      .Command17.Cancel() : .Command01.Cancel()

      .TempControlFlag = False
      DrainType = MinMax(param(1), 0, 1)
      .ReelStopRequest = True
      Wait.TimeRemaining = .Parameters.ReelStopDelayTime
      .SimulateFillWaterLiters = 0
      State = S14.WaitStopReel
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S14.Off
          StateString = ""


        Case S14.WaitStopReel
          StateString = If(.Language = LanguageValue.ZhTW, "停止帶布輪", "Stopping Reel")
          If Not Wait.Finished Then Exit Select
          .ReelStopRequest = False
          .PumpStopRequest = True
          Wait.TimeRemaining = 1
          State = S14.WaitTime


        Case S14.WaitTime
          If Not Wait.Finished Then Exit Select
          .PumpStopRequest = False
          '.IO.PumpSpeedControl = 0
          .PumpOn = False
          State = S14.WaitTempSafe

        Case S14.WaitTempSafe
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S14.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
          If .IO.MainTemperature >= .Parameters.SetSafetyTemp * 10 Then
            .Alarms.HighTempNoFill = True
            Exit Select
          End If

          .Alarms.HighTempNoDrain = False
          ' .IO.Overflow = True
          ' If DrainType = 0 Then
          ' .IO.Drain = True
          ' ElseIf DrainType = 1 Then
          ' .IO.HotDrain = True
          ' Else
          ' .IO.HotDrain = True
          ' .IO.Drain = True
          ' End If

          Wait.TimeRemaining = 600 ' after 10 minutes we maybe have a stuck LowLevel, so go on anyway
          State = S14.WaitDrain

        Case S14.WaitDrain
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S14.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "排水", "Draining ") & TimerString(Wait.TimeRemaining)
          If .IO.LowLevel And Not Wait.Finished Then Exit Select

          ' .IO.Overflow = False
          State = S14.WaitNotEntanglement2

        Case S14.WaitNotEntanglement2
          StateString = If(.Language = LanguageValue.ZhTW, "等待", "Waiting for not entangled2 ")
          If .IO.Entanglement2 Then Exit Select
          Wait.TimeRemaining = .Parameters.DrainDelay
          State = S14.WaitTime6

        Case S14.WaitTime6
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S14.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "排水", "Draining ") & TimerString(Wait.TimeRemaining)
          If Not Wait.Finished Then Exit Select
          ' .IO.HotDrain = False
          ' .IO.Drain = False
          State = S14.Off
        Case S14.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停", "Paused") & " " & TimerString(Wait.TimeRemaining)
          'no longer pause restart the timer and go back to the wait state
          If (Not .Parent.IsPaused) And .IO.SystemAuto Then
            Wait.Restart()
            State = StateWas
            StateWas = S14.Off
          End If


      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S14.Off
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
      Return State <> S14.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S14
  Public Property State() As S14
    Get
      Return state_
    End Get
    Private Set(ByVal value As S14)
      state_ = value
    End Set
  End Property
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S14
  Public Property StateWas() As S14
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S14)
      statewas_ = value
    End Set
  End Property

  Public ReadOnly Property IsHotDrain() As Boolean
    Get
      Return (DrainType <> 0) AndAlso ((State = S14.WaitDrain) Or (State = S14.WaitNotEntanglement2) Or (State = S14.WaitTime6))
    End Get
  End Property
  Public ReadOnly Property IsColdDrain() As Boolean
    Get
      Return (DrainType <> 1) AndAlso ((State = S14.WaitDrain) Or (State = S14.WaitNotEntanglement2) Or (State = S14.WaitTime6))
    End Get
  End Property
  Public ReadOnly Property IsOverflowDrain() As Boolean
    Get
      Return (State = S14.WaitDrain)
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command14 As New Command14(Me)
End Class
#End Region
