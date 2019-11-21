<Command("Flow Meter Fill", "Type |0-3| Qty |1-9999|", , , "5"), _
 TranslateCommand("zh-TW", "主缸流量進水", "水源選擇 |0-2| 水量設定 |1-9999|"), _
 Description("2=COLD+HOT 1=HOT 0=COLD, Qty=1~9999 Liters"), _
 TranslateDescription("zh-TW", "2=冷+熱水 1=熱水 0=冷水, 水量=1~9999 公升")> _
Public NotInheritable Class Command03
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S03
    Off
    WaitTempSafe
    ResetCounter
    WaitWater
    WaitMiddleLevel
    WaitTime4
    WaitMainPumpFB
    Pause
  End Enum

  Public Wait As New Timer
  Public StateString As String

  Public DesiredVolume As Integer
  Public TargetPulses As Integer
  Public WaterType As Integer
  Public CoolFill As Boolean
  Public ZeroPointPulses As Integer
  Public WaitRunning As New Timer

  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      'cancel all other foreground commands
      .Command02.Cancel() : .Command04.Cancel() : .Command10.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command12.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .TemperatureControl.Cancel()
      .Command17.Cancel() : .Command01.Cancel()
      '.Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      '.Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      '.Command59.Cancel() : .Command61.Cancel() 

      .TemperatureControlFlag = False
      WaterType = MinMax(param(1), 0, 2)
      .TotalWeight = CType(.Dyelot_Weight, Integer)
      .LiquidRatio = .Dyelot_LiquidRatio
      .TotalVolume = .Dyelot_TotalVolume
      If .TotalWeight > 0 And .LiquidRatio > 0 And .TotalVolume = 0 Then
        .TotalVolume = .TotalWeight * .LiquidRatio
      End If
      If .TotalVolume > 0 Then
        DesiredVolume = MinMax(.TotalVolume, 1, 9999)
      Else
        DesiredVolume = MinMax(param(2), 1, 9999)
      End If
      If DesiredVolume - .Parameters.AdditionVolume < .Parameters.MainTankFillMinVolume Then
        DesiredVolume = .Parameters.MainTankFillMinVolume
      Else
        DesiredVolume = DesiredVolume - .Parameters.AdditionVolume
      End If
      WaitRunning.TimeRemaining = .Parameters.MainTankFillDelayTimeMinute * 60
      State = S03.WaitTempSafe
      If WaterType = 0 Then
        CoolFill = True
      Else
        CoolFill = False
      End If
      .FillWaterLiters = 0

    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State
        Case S03.Off
          StateString = ""

        Case S03.WaitTempSafe
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            StateWas = State
            State = S03.Pause
            Wait.Pause()
          End If
          If .IO.MainTemperature >= .Parameters.SetSafetyTemp * 10 Then
            StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
            .Alarms.HighTempNoFill = True
            Exit Select
          End If

          .Alarms.HighTempNoFill = False
          TargetPulses = CType(DesiredVolume / .Parameters.VolumePerCount, Integer) + .IO.HSCounter1
          ZeroPointPulses = .IO.HSCounter1
          State = S03.WaitWater

        Case S03.ResetCounter
          StateString = If(.Language = LanguageValue.ZhTW, "流量計歸零", "Reset Counter")
          If .IO.HSCounter1 <> 0 Then Exit Select
          .IO.CounterReset = False
          State = S03.WaitWater

        Case S03.WaitWater
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            StateWas = State
            State = S03.Pause
            Wait.Pause()
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "主缸進水中", "Filling") & ":" & (.IO.HSCounter1 - ZeroPointPulses) * .Parameters.VolumePerCount & "/" & DesiredVolume
          If .IO.HSCounter1 >= TargetPulses Or .IO.HighLevel Then
            State = S03.WaitMiddleLevel
          End If
          Exit Select

        Case S03.WaitMiddleLevel
          If .Parent.IsPaused Or Not .IO.SystemAuto Then
            StateWas = State
            State = S03.Pause
            Wait.Pause()
          End If
          StateString = If(.Language = LanguageValue.ZhTW, "主缸進水中", "Filling") & ":" & (.IO.HSCounter1 - ZeroPointPulses) * .Parameters.VolumePerCount & "/" & DesiredVolume
          If Not .IO.LowLevel Then Exit Select
          .PumpStartRequest = True
          Wait.TimeRemaining = 1
          State = S03.WaitTime4

        Case S03.WaitTime4
          StateString = If(.Language = LanguageValue.ZhTW, "主缸進水中", "Filling") & ":" & (.IO.HSCounter1 - ZeroPointPulses) * .Parameters.VolumePerCount & "/" & DesiredVolume
          If Not Wait.Finished Then Exit Select
          .PumpStartRequest = False
          .PumpOn = True
          State = S03.WaitMainPumpFB

        Case S03.WaitMainPumpFB
          StateString = If(.Language = LanguageValue.ZhTW, "主泵沒有運行", "Main pump not running")
          If Not .IO.MainPumpFB Then Exit Select
          WaitRunning.Cancel()
          State = S03.Off

        Case S03.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停", "Paused") & " " & TimerString(Wait.TimeRemaining)
          'no longer pause restart the timer and go back to the wait state
          If (Not .Parent.IsPaused) And .IO.SystemAuto Then
            Wait.Restart()
            State = StateWas
            StateWas = S03.Off
          End If

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S03.Off
    CoolFill = False
    Wait.Cancel()
    WaitRunning.Cancel()
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
    With ControlCode
      WaterType = MinMax(param(1), 0, 2)
      If .TotalWeight > 0 And .LiquidRatio > 0 Then
        .TotalVolume = .TotalWeight * .LiquidRatio
      End If
      If .TotalVolume > 0 Then
        DesiredVolume = MinMax(.TotalVolume, 1, 9999)
      Else
        DesiredVolume = MinMax(param(2), 1, 9999)
      End If
    End With
  End Sub


#Region "Standard Definitions"
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S03.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S03
  Public Property State() As S03
    Get
      Return state_
    End Get
    Private Set(ByVal value As S03)
      state_ = value
    End Set
  End Property
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S03
  Public Property StateWas() As S03
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S03)
      statewas_ = value
    End Set
  End Property


  Public ReadOnly Property IsFillHot() As Boolean
    Get
      Return (WaterType <> 0) AndAlso (State = S03.WaitWater)
    End Get
  End Property
  Public ReadOnly Property IsFillCold() As Boolean
    Get
      Return (WaterType <> 1) AndAlso (State = S03.WaitWater)
    End Get
  End Property
  Public ReadOnly Property IsCoolFill() As Boolean
    Get
      Return CoolFill AndAlso (State = S03.WaitWater)
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command03 As New Command03(Me)
End Class
#End Region
