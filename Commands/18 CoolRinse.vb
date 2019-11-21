<Command("Cool Rinse", "Temperature |0-99| Type|0-2|", , "'1", "10"), _
 TranslateCommand("zh-TW", "降溫水洗", "水洗溫度 |0-99| 水源種類|0-2|"), _
 Description("99=MAX 0=MIN,0=Cooling 1=B tank 2=Cooling+B Tank"), _
 TranslateDescription("zh-TW", "99=最高 0=最小,0=降溫 1=B缸 2=降溫+B缸")> _
Public NotInheritable Class Command18
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S18
    Off
    WaitTempSafe
    WaitLowLevel
    WaitMainPumpFB
    WaitReachTemp
    WaitOverflow
    Pause
  End Enum

  Public Wait As New Timer
  Public RinseTemp As New Integer
  Public CoolFill As Boolean
  Public Type As Integer
  Public StateString As String

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command10.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .TemperatureControl.Cancel()
      .Command17.Cancel() : .Command01.Cancel()

      .TempControlFlag = False
      RinseTemp = param(1) * 10
      Type = param(2)

      If Type = 0 And .Parameters.ReCycleWater = 1 Then
        CoolFill = True
      ElseIf Type = 0 And .Parameters.ReCycleWater = 0 Then
        CoolFill = False
      End If
      State = S18.WaitTempSafe
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S18.Off
          StateString = ""

        Case S18.WaitTempSafe
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S18.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
          If .IO.MainTemperature >= .Parameters.SetSafetyTemp * 10 Then
            .Alarms.HighTempNoFill = True
            Exit Select
          End If

          .Alarms.HighTempNoFill = False
          '.IO.Cool = True
          If RinseTemp = 0 Then
            State = S18.Off
          Else
            State = S18.WaitLowLevel
          End If

        Case S18.WaitLowLevel
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S18.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "主缸沒水", "Main tank no water ")
          If Not .IO.LowLevel Then Exit Select
          .PumpStartRequest = True
          State = S18.WaitMainPumpFB

        Case S18.WaitMainPumpFB
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S18.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "主泵沒有運行", "Main pump not running")
          If Not .IO.MainPumpFB Then Exit Select
          .PumpStartRequest = False
          .PumpOn = True
          State = S18.WaitReachTemp

        Case S18.WaitReachTemp
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S18.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "主缸溢流洗至設定溫度", "Rinse to target temp ")
          If Not (.IO.MainTemperature < RinseTemp) Then Exit Select
          Wait.TimeRemaining = .Parameters.OverflowTimeAfterRinse
          State = S18.Off

        Case S18.WaitOverflow
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S18.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "溢流排水中", "Overflowing") & "" & TimerString(Wait.TimeRemaining)
          If Not Wait.Finished Then Exit Select
          State = S18.Off

        Case S18.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停", "Paused") & " " & TimerString(Wait.TimeRemaining)
          'no longer pause restart the timer and go back to the wait state
          If (Not .Parent.IsPaused) And .IO.SystemAuto Then
            Wait.Restart()
            State = StateWas
            StateWas = S18.Off
          End If

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S18.Off
    CoolFill = False
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
      Return State <> S18.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S18
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S18
  Public Property State() As S18
    Get
      Return state_
    End Get
    Private Set(ByVal value As S18)
      state_ = value
    End Set
  End Property
  Public Property StateWas() As S18
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S18)
      statewas_ = value
    End Set
  End Property
  Public ReadOnly Property IsCoolFill() As Boolean
    Get
      Return CoolFill AndAlso (State = S18.WaitReachTemp)
    End Get
  End Property
  Public ReadOnly Property IsOverFlowDrain() As Boolean
    Get
      Return (State = S18.WaitReachTemp) Or (State = S18.WaitOverflow)
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command18 As New Command18(Me)
End Class
#End Region
