<Command("Hi Low Rinse", "Type |0-3| Time |1-99|", , "60", "'2"), _
 TranslateCommand("zh-TW", "高低水洗", "水源選擇 |0-3| 水洗時間 |0-99|"), _
 Description("3=ReCycleWater 2=COLD+HOT 1=HOT 0=COLD,99=MAX 1=MIN"), _
 TranslateDescription("zh-TW", "3=降溫回收水 2=冷+熱水 1=熱水 0=冷水,99=最高 0=最小")> _
Public NotInheritable Class Command13
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S13
    Off
    WaitTempSafe
    WaitMiddleLevel
    WaitTime3
    WaitMiddleLevel2
    WaitMiddleLevel3
    WaitTime4
    WaitMainPumpFB
    WaitHighLevel
    WaitDrainToMiddleLevel
    WaitTime5
    WaitOverflow
    Pause
  End Enum

  Public Wait As New Timer
  Public RinseTime As Integer
  Public RinseTimeWas As Integer
  Public WaterType As Integer
  Public CoolFill As Boolean
  Public StateString As String
  Public WaitRunning As New Timer

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command12.Cancel() : .Command10.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .TemperatureControl.Cancel()
      .Command17.Cancel() : .Command01.Cancel()

      .TemperatureControlFlag = False
      WaterType = MinMax(param(1), 0, 3)
      If WaterType = 3 And .Parameters.ReCycleWater = 1 Then
        CoolFill = True
      End If
      RinseTime = param(2)
      .FillWaterLiters = 0

      State = S13.WaitTempSafe
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S13.Off
          StateString = ""

        Case S13.WaitTempSafe
          StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
          If .IO.MainTemperature >= .Parameters.SetSafetyTemp * 10 Then
            .Alarms.HighTempNoFill = True
            Exit Select
          End If
          .Alarms.HighTempNoFill = False
          WaitRunning.TimeRemaining = .Parameters.MainTankFillDelayTimeMinute * 60
          State = S13.WaitMiddleLevel

        Case S13.WaitMiddleLevel
          StateString = If(.Language = LanguageValue.ZhTW, "主缸進水至中水位", "Filling to middle level ")
          If Not .IO.MiddleLevel Then Exit Select
          Wait.TimeRemaining = 2
          State = S13.WaitTime3

        Case S13.WaitTime3
          StateString = If(.Language = LanguageValue.ZhTW, "主缸進水至中水位", "Filling to middle level ")
          If Not Wait.Finished Then Exit Select
          State = S13.WaitMiddleLevel2

        Case S13.WaitMiddleLevel2
          StateString = If(.Language = LanguageValue.ZhTW, "主缸進水至中水位", "Filling to middle level ")
          If Not .IO.MiddleLevel Then Exit Select
          State = S13.WaitMiddleLevel3

        Case S13.WaitMiddleLevel3
          StateString = If(.Language = LanguageValue.ZhTW, "主缸進水至中水位", "Filling to middle level ")
          If Not .IO.MiddleLevel Then Exit Select
          ' .IO.PumpSpeedControl = CType(.PumpSpeed * 10, Short)
          WaitRunning.Cancel()
          .PumpStartRequest = True
          Wait.TimeRemaining = 1
          State = S13.WaitTime4

        Case S13.WaitTime4
          StateString = If(.Language = LanguageValue.ZhTW, "主缸進水至中水位", "Filling to middle level ")
          If Not Wait.Finished Then Exit Select
          .PumpStartRequest = False
          .PumpOn = True
          State = S13.WaitMainPumpFB

        Case S13.WaitMainPumpFB
          StateString = If(.Language = LanguageValue.ZhTW, "主泵沒有運行", "Main pump not running")
          If Not .IO.MainPumpFB Then Exit Select
          State = S13.WaitHighLevel

        Case S13.WaitHighLevel
          StateString = If(.Language = LanguageValue.ZhTW, "主缸進水至高水位", "Filling to high level ") & TimerString(Wait.TimeRemaining)
          If Not .IO.HighLevel Then Exit Select
          Wait.TimeRemaining = 60 * RinseTime
          State = S13.WaitTime5

        Case S13.WaitDrainToMiddleLevel
          If .Parent.IsPaused Or Not .IO.MainPumpFB Then
            StateWas = State
            State = S13.Pause
            Wait.Pause()
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "排水至中水位", "Draining to middle level ") & TimerString(Wait.TimeRemaining)
          If Not .IO.MiddleLevel Then
            State = S13.WaitTime5
          End If
          If Wait.Finished Then
            Wait.TimeRemaining = .Parameters.OverflowTimeAfterRinse
            State = S13.WaitOverflow
          End If

        Case S13.WaitTime5
          If .Parent.IsPaused Or Not .IO.MainPumpFB Then
            StateWas = State
            State = S13.Pause
            Wait.Pause()
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "進水至高水位", "Filling to high level ") & TimerString(Wait.TimeRemaining)
          If Wait.Finished Then
            Wait.TimeRemaining = .Parameters.OverflowTimeAfterRinse
            State = S13.WaitOverflow
          End If
          If .IO.HighLevel Then State = S13.WaitDrainToMiddleLevel

        Case S13.WaitOverflow
          StateString = If(.Language = LanguageValue.ZhTW, "溢流排水中", "Overflowing") & "" & TimerString(Wait.TimeRemaining)
          If Not Wait.Finished Then Exit Select
          State = S13.Off

            Case S13.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停", "Paused") & " " & TimerString(Wait.TimeRemaining)
          If Not .Parent.IsPaused And .IO.MainPumpFB Then
            If WaterType = 3 And .Parameters.ReCycleWater = 1 Then
              CoolFill = True
            End If
            If StateWas = S13.WaitTime5 Or StateWas = S13.WaitDrainToMiddleLevel Then
              State = StateWas
              StateWas = S13.Off
              If RinseTimeWas = RinseTime Then
                Wait.Restart()
              Else
                Wait.TimeRemaining = RinseTime
              End If
            Else
              State = StateWas
              StateWas = S13.Off
              Wait.Restart()
            End If
          End If
      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S13.Off
    CoolFill = False
    Wait.Cancel()
    Wait.Cancel()
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
    RinseTimeWas = RinseTime
    WaterType = MinMax(param(1), 0, 3)
    RinseTime = param(2)
  End Sub


#Region "Standard Definitions"
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S13.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S13
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S13

  Public Property State() As S13
    Get
      Return state_
    End Get
    Private Set(ByVal value As S13)
      state_ = value
    End Set
  End Property
  Public Property StateWas() As S13
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S13)
      statewas_ = value
    End Set
  End Property
  Public ReadOnly Property IsFillHot() As Boolean
    Get
      Return (WaterType <> 0 And WaterType < 3) AndAlso ((State = S13.WaitMiddleLevel) Or (State = S13.WaitTime3) Or (State = S13.WaitMiddleLevel2) Or _
             (State = S13.WaitMiddleLevel3) Or (State = S13.WaitTime4) Or (State = S13.WaitHighLevel) Or (State = S13.WaitTime5))
    End Get
  End Property
  Public ReadOnly Property IsFillCold() As Boolean
    Get
      Return (WaterType <> 1 And WaterType < 3) AndAlso ((State = S13.WaitMiddleLevel) Or (State = S13.WaitTime3) Or (State = S13.WaitMiddleLevel2) Or _
             (State = S13.WaitMiddleLevel3) Or (State = S13.WaitTime4) Or (State = S13.WaitHighLevel) Or (State = S13.WaitTime5))
    End Get
  End Property
  Public ReadOnly Property IsCoolFill() As Boolean
    Get
      Return CoolFill AndAlso ((State = S13.WaitMiddleLevel) Or (State = S13.WaitTime3) Or (State = S13.WaitMiddleLevel2) Or _
             (State = S13.WaitMiddleLevel3) Or (State = S13.WaitTime4) Or (State = S13.WaitHighLevel) Or (State = S13.WaitTime5))
    End Get
  End Property
  Public ReadOnly Property IsOverflow() As Boolean
    Get
      Return (State = S13.WaitOverflow)
    End Get
  End Property
  Public ReadOnly Property IsDrainToMiddleLevel() As Boolean
    Get
      Return (State = S13.WaitDrainToMiddleLevel)
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command13 As New Command13(Me)
End Class
#End Region
