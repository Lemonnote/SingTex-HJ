<Command("Air Cool Rinse", "Temperature |0-99| Type |0-3| Time |1-99|", , "'1", "10"),
 TranslateCommand("zh-TW", "氣冷水洗溫度", "水洗溫度 |0-99| 水源選擇 |0-3| 水洗時間 |0-99|"),
 Description("Air Cool Rinse To Temperature=0~99, 3=ReCycleWater 2=COLD+HOT 1=HOT 0=COLD,99=MAX 1=MIN"),
 TranslateDescription("zh-TW", "氣冷水洗至0~99度, 3=降溫回收水 2=冷+熱水 1=熱水 0=冷水,99=最高,0=最小")>
Public NotInheritable Class Command15
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S15
    Off
    WaitTempSafe
    WaitLowLevel
    WaitMainPumpFB
    WaitReachTemp
    WaitForRinseTime
    WaitOverflow
    Pause
  End Enum

  Public Wait As New Timer
  Public RinseTemp As New Integer
  Public StateString As String
  Public FillTimeOver As New Timer
  Public RinseTime As Integer
  Public RinseTimeWas As Integer
  Public WaterType As Integer
  Public CoolFill As Boolean

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command10.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .TemperatureControl.Cancel()
      .Command17.Cancel() : .Command01.Cancel()
      .TempControlFlag = False
      RinseTemp = param(1) * 10
      WaterType = MinMax(param(2), 0, 3)
      If WaterType = 3 And .Parameters.ReCycleWater = 1 Then
        CoolFill = True
      End If
      RinseTime = param(3)


      State = S15.WaitTempSafe
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S15.Off
          StateString = ""

        Case S15.WaitTempSafe
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S15.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
          If .IO.MainTemperature >= .Parameters.SetSafetyTemp * 10 Then
            .Alarms.HighTempNoFill = True
            Exit Select
          End If

          .Alarms.HighTempNoFill = False
          '.IO.Cool = True
          If RinseTemp = 0 Then
            State = S15.Off
          Else
            State = S15.WaitLowLevel
          End If

        Case S15.WaitLowLevel
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S15.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "主缸沒水", "Main tank no water ")
          If Not .IO.LowLevel Then Exit Select
          .PumpStartRequest = True
          State = S15.WaitMainPumpFB

        Case S15.WaitMainPumpFB
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S15.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "主泵沒有運行", "Main pump not running")
          If Not .IO.MainPumpFB Then Exit Select
          .PumpStartRequest = False
          .PumpOn = True
          If .Parameters.AirCooling = 1 Then
            State = S15.WaitReachTemp
          Else
            Wait.TimeRemaining = RinseTime * 60
            State = S15.WaitForRinseTime
          End If

        Case S15.WaitReachTemp
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            Wait.Pause()
            StateWas = State
            State = S15.Pause
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "氣冷水洗至設定溫度", "Air Cool Rinse to target temp ")
          If Not (.IO.MainTemperature <= RinseTemp) Then Exit Select
          Wait.TimeRemaining = .Parameters.OverflowTimeAfterRinse
          State = S15.WaitOverflow

        Case S15.WaitForRinseTime
          StateString = If(.Language = LanguageValue.ZhTW, "溢流水洗中", "Rinse for the time ") & TimerString(Wait.TimeRemaining)
          If .Parent.IsPaused Or Not .IO.MainPumpFB Then
            StateWas = State
            State = S15.Pause
            Wait.Pause()
          End If
          If Not Wait.Finished Then Exit Select
          Wait.TimeRemaining = .Parameters.OverflowTimeAfterRinse
          State = S15.WaitOverflow

        Case S15.WaitOverflow
          StateString = If(.Language = LanguageValue.ZhTW, "溢流排水中", "Overflowing") & "" & TimerString(Wait.TimeRemaining)
          If Not Wait.Finished Then Exit Select
          State = S15.Off


        Case S15.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停", "Paused") & " " & TimerString(Wait.TimeRemaining)
          'no longer pause restart the timer and go back to the wait state
          If .Parent.CurrentStep <> .Parent.ChangingStep Then
            State = S15.Off
            Wait.Cancel()
          End If
          If (Not .Parent.IsPaused) And .IO.SystemAuto Then
            Wait.Restart()
            State = StateWas
            StateWas = S15.Off
          End If

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S15.Off
    Wait.Cancel()
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
    RinseTemp = param(1) * 10
  End Sub


#Region "Standard Definitions"
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S15.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S15
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S15
  Public Property State() As S15
    Get
      Return state_
    End Get
    Private Set(ByVal value As S15)
      state_ = value
    End Set
  End Property
  Public Property StateWas() As S15
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S15)
      statewas_ = value
    End Set
  End Property
  Public ReadOnly Property IsAirCoolRinse() As Boolean
    Get
      Return (State = S15.WaitReachTemp)
    End Get
  End Property
  Public ReadOnly Property IsFillHot() As Boolean
    Get
      Return (WaterType <> 0 And WaterType < 3) AndAlso (State = S15.WaitForRinseTime Or State = S15.WaitLowLevel)
    End Get
  End Property
  Public ReadOnly Property IsFillCold() As Boolean
    Get
      Return (WaterType <> 1 And WaterType < 3) AndAlso (State = S15.WaitForRinseTime Or State = S15.WaitLowLevel)
    End Get
  End Property
  Public ReadOnly Property IsCoolFill() As Boolean
    Get
      Return CoolFill AndAlso (State = S15.WaitForRinseTime Or State = S15.WaitLowLevel)
    End Get
  End Property
  Public ReadOnly Property IsOverFlow() As Boolean
    Get
      Return (State = S15.WaitForRinseTime) Or (State = S15.WaitOverflow)
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command15 As New Command15(Me)
End Class
#End Region
