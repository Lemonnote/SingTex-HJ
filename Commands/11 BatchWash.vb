<Command("Batch Wash", "Type |0-3| Time |1-30| Times |1-9| Level |0-2|", , "60", "'2"), _
 TranslateCommand("zh-TW", "批次水洗", "水源選擇 |0-3| 水洗時間 |1-30| 水洗次數 |1-9| 水洗水位 |0-2|"), _
 Description("3=ReCycleWater 2=COLD+HOT 1=HOT 0=COLD,30=MAX 1=MIN,MAX=9 MIN=1,2=HIGH 1=MID 0=LOW"), _
 TranslateDescription("zh-TW", "3=降溫回收水 2=冷+熱水 1=熱水 0=冷水,30分=最高 1分=最小,最高=9次 最小=1次,2=高水位 1=中水位,0=低水位")> _
Public NotInheritable Class Command11
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S11
    Off
    WaitStopReel
    WaitTime
    WaitTempSafe
    StartWash
    WaitLevel
    WaitMiddleLevel
    WaitTime4
    WaitMainPumpFB
    WaitTime5
    WaitLevel2
    WaitTime6
    WaitTime7
    WaitTime8
    WaitTime9
    WaitDrain
    WaitTime11
    WaitBTankDrain
    Pause
  End Enum

  Public Wait As New Timer
  Public MainTankLevel As Integer
  Public WashTime As Integer
  Public WashTimeWas As Integer
  Public WashesToGo As Integer
  Public WaterType As Integer
  Public CoolFill As Boolean
  Public FillByBTank As Boolean
  Public StateString As String
  Public WaitRunning As New Timer

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      'cancels for all other forground functions
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command10.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .TemperatureControl.Cancel()
      .Command17.Cancel() : .Command01.Cancel()

      .TemperatureControlFlag = False
      WaterType = MinMax(param(1), 0, 3)
      WashTime = Maximum(param(2), 30)
      WashesToGo = Maximum(param(3), 9)
      MainTankLevel = Maximum(param(4), 2)
      If WaterType = 3 And .Parameters.ReCycleWater = 1 Then
        CoolFill = True
      ElseIf WaterType = 3 And .Parameters.ReCycleWater = 0 Then
        FillByBTank = True
      End If
      .ReelStopRequest = True
      Wait.TimeRemaining = .Parameters.ReelStopDelayTime
      .FillWaterLiters = 0
      State = S11.WaitStopReel
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State

        Case S11.Off
          StateString = ""

        Case S11.WaitStopReel
          StateString = If(.Language = LanguageValue.ZhTW, "停止帶布輪", "Stopping Reel")
          If Not Wait.Finished Then Exit Select
          .ReelStopRequest = False
          .PumpStopRequest = True
          Wait.TimeRemaining = 1
          State = S11.WaitTime


        Case S11.WaitTime
          StateString = If(.Language = LanguageValue.ZhTW, "停止主泵", "Stopping pump")
          If Not Wait.Finished Then Exit Select
          .PumpStopRequest = False
          '.IO.PumpSpeedControl = CType(.PumpSpeed * 10, Short)
          .PumpOn = False
          State = S11.WaitTempSafe

        Case S11.WaitTempSafe
          StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
          If .IO.MainTemperature >= .Parameters.SetSafetyTemp * 10 Then
            .Alarms.HighTempNoFill = True
            Exit Select
          End If

          .Alarms.HighTempNoFill = False
          State = S11.StartWash

        Case S11.StartWash
          WaitRunning.TimeRemaining = .Parameters.MainTankFillDelayTimeMinute * 60
          State = S11.WaitLevel

        Case S11.WaitLevel
          If MainTankLevel = 0 Then
            StateString = If(.Language = LanguageValue.ZhTW, "主缸進水至低水位", "Filling to low level ")
            If Not .IO.LowLevel Then Exit Select
          ElseIf MainTankLevel = 1 Then
            StateString = If(.Language = LanguageValue.ZhTW, "主缸進水至中水位", "Filling to Mid level ")
            If Not .IO.MiddleLevel Then Exit Select
          Else
            StateString = If(.Language = LanguageValue.ZhTW, "主缸進水至高水位", "Filling to High level ")
            If Not .IO.HighLevel Then Exit Select
          End If
          State = S11.WaitMiddleLevel

        Case S11.WaitMiddleLevel
          StateString = If(.Language = LanguageValue.ZhTW, "主缸進水", "Level to low")
          If Not .IO.MiddleLevel Then Exit Select
          WaitRunning.Cancel()
          '.IO.PumpSpeedControl = CType(.PumpSpeed * 10, Short)
          .PumpStartRequest = True
          Wait.TimeRemaining = 1
          State = S11.WaitTime4

        Case S11.WaitTime4
          If Not Wait.Finished Then Exit Select
          .PumpStartRequest = False
          .PumpOn = True
          State = S11.WaitMainPumpFB

        Case S11.WaitMainPumpFB
          StateString = If(.Language = LanguageValue.ZhTW, "主泵沒有運行", "Main pump not running")
          If Not .IO.MainPumpFB Then Exit Select
          Wait.TimeRemaining = 2
          State = S11.WaitTime5

        Case S11.WaitTime5
          If Not Wait.Finished Then Exit Select
          WaitRunning.TimeRemaining = .Parameters.MainTankFillDelayTimeMinute * 60
          State = S11.WaitLevel2

        Case S11.WaitLevel2
          StateString = If(.Language = LanguageValue.ZhTW, "主缸進水", "Level to low")
          If MainTankLevel = 0 Then
            If Not .IO.LowLevel Then Exit Select
          ElseIf MainTankLevel = 1 Then
            If Not .IO.MiddleLevel Then Exit Select
          Else
            If Not .IO.HighLevel Then Exit Select
          End If
          Wait.TimeRemaining = 2
          State = S11.WaitTime6

        Case S11.WaitTime6
          If Not Wait.Finished Then Exit Select
          WaitRunning.Cancel()
          '.IO.ColdFill = False
          '.IO.HotFill = False
          '.IO.Cool = False
          Wait.TimeRemaining = WashTime * 60 ' TODO: should be 60
          State = S11.WaitTime7

        Case S11.WaitTime7
          If .Parent.IsPaused Or Not .IO.MainPumpFB Then
            StateWas = State
            State = S11.Pause
            Wait.Pause()
          End If

          StateString = If(.Language = LanguageValue.ZhTW, "水洗中", "Washing ") & TimerString(Wait.TimeRemaining)
          If Not Wait.Finished Then Exit Select
          .PumpStopRequest = True
          Wait.TimeRemaining = 1
          State = S11.WaitTime8

        Case S11.WaitTime8
          If Not Wait.Finished Then Exit Select
          .PumpStopRequest = False
          '.IO.PumpSpeedControl = 0
          .PumpOn = False
          '.IO.Overflow = True
          '.IO.HotDrain = True
          '.IO.Drain = True

          Wait.TimeRemaining = 600 ' after 10 minutes we maybe have a stuck LowLevel, so go on anyway
          WaitRunning.Cancel()
          State = S11.WaitDrain

        Case S11.WaitDrain
          StateString = If(.Language = LanguageValue.ZhTW, "排水到低水位", "Draining to low level") & TimerString(Wait.TimeRemaining)
          If .IO.LowLevel And Not Wait.Finished Then Exit Select

          '.IO.Overflow = False
          '.IO.HotDrain = False
          '.IO.Drain = False
          WashesToGo = WashesToGo - 1
          If WashesToGo > 0 Then
            State = S11.StartWash
            Exit Select
          End If
          ' .IO.HotDrain = True
          ' .IO.Drain = True
          Wait.TimeRemaining = .Parameters.DrainDelay
          State = S11.WaitTime11

        Case S11.WaitTime11
          If Not Wait.Finished Then Exit Select
          '.IO.HotDrain = False
          '.IO.Drain = False
          If FillByBTank Then
            State = S11.WaitBTankDrain
          Else
            State = S11.Off
          End If

        Case S11.WaitBTankDrain
          StateString = If(.Language = LanguageValue.ZhTW, "B缸排水中", "B tank is draining")
          If .BTankLowLevel Then Wait.TimeRemaining = .Parameters.AddTransferDrainTime
          If Wait.Finished Then
            State = S11.Off
          End If

        Case S11.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停", "Paused") & " " & TimerString(Wait.TimeRemaining)
          If Not .Parent.IsPaused And .IO.MainPumpFB Then
            If StateWas = S11.WaitTime7 Then
              State = StateWas
              StateWas = S11.Off
              If WashTimeWas = WashTime Then
                Wait.Restart()
              Else
                Wait.TimeRemaining = WashTime * 60
              End If
            Else
              State = StateWas
              StateWas = S11.Off
              Wait.Restart()
            End If
          End If
      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S11.Off
    CoolFill = False
    FillByBTank = False
    Wait.Cancel()
    WaitRunning.Cancel()
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
    WashTimeWas = WashTime
    WaterType = MinMax(param(1), 0, 3)
    WashTime = Maximum(param(2), 30)
    WashesToGo = Maximum(param(3), 9)
    MainTankLevel = Maximum(param(4), 2)
  End Sub


#Region "Standard Definitions"
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S11.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S11
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S11
  Public Property State() As S11
    Get
      Return state_
    End Get
    Private Set(ByVal value As S11)
      state_ = value
    End Set
  End Property
  Public Property StateWas() As S11
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S11)
      statewas_ = value
    End Set
  End Property
  Public ReadOnly Property IsFillHot() As Boolean
    Get
      Return (WaterType <> 0) And (WaterType < 3) AndAlso ((State = S11.WaitLevel) Or (State = S11.WaitMiddleLevel) Or _
             (State = S11.WaitTime4) Or (State = S11.WaitMainPumpFB) Or (State = S11.WaitTime5) Or _
            (State = S11.WaitLevel2) Or (State = S11.WaitTime6))
    End Get
  End Property
  Public ReadOnly Property IsFillCold() As Boolean
    Get
      Return (WaterType <> 1) And (WaterType < 3) AndAlso ((State = S11.WaitLevel) Or (State = S11.WaitMiddleLevel) Or _
             (State = S11.WaitTime4) Or (State = S11.WaitMainPumpFB) Or (State = S11.WaitTime5) Or _
            (State = S11.WaitLevel2) Or (State = S11.WaitTime6))
    End Get
  End Property
  Public ReadOnly Property IsCoolFill() As Boolean
    Get
      Return CoolFill AndAlso ((State = S11.WaitLevel) Or (State = S11.WaitMiddleLevel) Or _
             (State = S11.WaitTime4) Or (State = S11.WaitMainPumpFB) Or (State = S11.WaitTime5) Or _
            (State = S11.WaitLevel2) Or (State = S11.WaitTime6))
    End Get
  End Property
  Public ReadOnly Property IsFillbyBtank() As Boolean
    Get
      Return FillByBTank AndAlso ((State = S11.WaitLevel) Or (State = S11.WaitMiddleLevel) Or _
             (State = S11.WaitTime4) Or (State = S11.WaitMainPumpFB) Or (State = S11.WaitTime5) Or _
            (State = S11.WaitLevel2) Or (State = S11.WaitTime6))
    End Get
  End Property
  Public ReadOnly Property IsDraining() As Boolean
    Get
      Return (State = S11.WaitBTankDrain)
    End Get
  End Property
  Public ReadOnly Property IsDrainingToLowLevel() As Boolean
    Get
      Return (State = S11.WaitDrain)
    End Get
  End Property
  Public ReadOnly Property IsDrainingEmpty() As Boolean
    Get
      Return (State = S11.WaitTime11)
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command11 As New Command11(Me)
End Class
#End Region
