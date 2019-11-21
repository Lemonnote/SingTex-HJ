<Command("Rinse", "Type |0-3| Time |1-99|", , "60", "'2"), _
 TranslateCommand("zh-TW", "溢流水洗", "水源選擇 |0-3| 水洗時間 |0-99|"), _
 Description("3=ReCycleWater 2=COLD+HOT 1=HOT 0=COLD,99=MAX 1=MIN"), _
 TranslateDescription("zh-TW", "3=降溫回收水 2=冷+熱水 1=熱水 0=冷水,99=最高,0=最小")> _
Public NotInheritable Class Command12
  Inherits MarshalByRefObject
  Implements ACCommand

  Public Enum S12
    Off
    WaitTempSafe
    ResetCounter
    WaitLowLevel
    WaitMainPumpFB
    WaitForRinseTime
    WaitForRinseLevel
    WaitOverflowTime
    Pause
  End Enum

  Public Wait As New Timer
  Public WaitLevel As New Timer
  Public FillTimeOver As New Timer
  Public RinseTime As Integer
  Public RinseTimeWas As Integer
  Public WaterType As Integer
  Public CoolFill As Boolean
  Public StateString As String
  Public DesiredVolume As Integer
  Public TargetPulses As Integer


  Public Function Start(ByVal ParamArray param() As Integer) As Boolean Implements ACCommand.Start
    With ControlCode
      'cancels for all other forground functions
      .Command02.Cancel() : .Command03.Cancel() : .Command04.Cancel() : .Command05.Cancel()
      .Command11.Cancel() : .Command10.Cancel() : .Command13.Cancel() : .Command14.Cancel()
      .Command16.Cancel() : .Command20.Cancel() : .Command31.Cancel() : .Command32.Cancel()
      .Command33.Cancel() : .Command51.Cancel() : .Command52.Cancel() : .Command54.Cancel()
      .Command55.Cancel() : .Command56.Cancel() : .Command57.Cancel() : .Command58.Cancel()
      .Command59.Cancel() : .Command61.Cancel() : .TemperatureControl.Cancel()
      .Command17.Cancel() : .Command01.Cancel()
      .TempControlFlag = False
      WaterType = MinMax(param(1), 0, 3)
      If WaterType = 3 And .Parameters.ReCycleWater = 1 Then
        CoolFill = True
      End If
      RinseTime = param(2)
      'DesiredVolume = MinMax(param(3), 0, 9999)
      DesiredVolume = 0
      .FillWaterLiters = 0

      State = S12.WaitTempSafe
    End With
  End Function

  Public Function Run() As Boolean Implements ACCommand.Run
    With ControlCode
      Select Case State

        Case S12.Off
          StateString = ""

        Case S12.WaitTempSafe
          StateString = If(.Language = LanguageValue.ZhTW, "溫度異常", "Interlocked Temperature")
          If .IO.MainTemperature >= .Parameters.SetSafetyTemp * 10 Then
            .Alarms.HighTempNoFill = True
            Exit Select
          End If

          .Alarms.HighTempNoFill = False
          FillTimeOver.TimeRemaining = 60
          State = S12.WaitLowLevel
          TargetPulses = CType(DesiredVolume / .Parameters.VolumePerCount, Integer) + .IO.HSCounter1
          If FillTimeOver.Finished Then WaterType = 2


        Case S12.ResetCounter
          StateString = If(.Language = LanguageValue.ZhTW, "流量計歸零", "Reset Counter")
          If .IO.HSCounter1 <> 0 Then Exit Select
          .IO.CounterReset = False
          State = S12.WaitLowLevel


        Case S12.WaitLowLevel
          StateString = If(.Language = LanguageValue.ZhTW, "主缸沒水", "Main tank no water ")
          If Not (.IO.LowLevel Or .IO.MiddleLevel) Then Exit Select
          Wait.TimeRemaining = 2
          State = S12.WaitMainPumpFB

        Case S12.WaitMainPumpFB
          StateString = If(.Language = LanguageValue.ZhTW, "主泵沒有運行", "Main pump not running")
          If Not .IO.MainPumpFB Then Exit Select
          State = S12.WaitForRinseTime
          Wait.TimeRemaining = 60 * RinseTime

        Case S12.WaitForRinseTime
          'StateString = If(.Language = LanguageValue.ZhTW, "溢流水洗中", "Rinse for the time ") & TimerString(Wait.TimeRemaining) & ": " & .IO.HSCounter1 * .Parameters.VolumePerCount & "/" & DesiredVolume
          StateString = If(.Language = LanguageValue.ZhTW, "溢流水洗中", "Rinse for the time ") & TimerString(Wait.TimeRemaining)
          If .Parent.IsPaused Or Not .IO.MainPumpFB Then
            StateWas = State
            State = S12.Pause
            Wait.Pause()
          End If
          '中水位到達後停止進水
          If .IO.MiddleLevel Then
            WaitLevel.TimeRemaining = .Parameters.OverflowWaitLevelTime
            State = S12.WaitForRinseLevel
          End If
          If RinseTime > 0 And DesiredVolume > 0 Then
            If .IO.HSCounter1 < TargetPulses Then Exit Select
          ElseIf RinseTime > 0 Then
            If Not Wait.Finished Then Exit Select
          ElseIf DesiredVolume > 0 Then
            If .IO.HSCounter1 < TargetPulses Then Exit Select
          End If
          Wait.TimeRemaining = .Parameters.OverflowTimeAfterRinse
          State = S12.WaitOverflowTime

        Case S12.WaitForRinseLevel
          StateString = If(.Language = LanguageValue.ZhTW, "溢流水洗中", "Rinse for the time ") & TimerString(Wait.TimeRemaining)
          If .Parent.IsPaused Or Not .IO.MainPumpFB Then
            StateWas = State
            State = S12.Pause
            Wait.Pause()
          End If
          '中水位消失時間到達後開始進水
          If .IO.MiddleLevel Then WaitLevel.TimeRemaining = .Parameters.OverflowWaitLevelTime
          If WaitLevel.Finished Then
            State = S12.WaitForRinseTime
          End If
          If RinseTime > 0 And DesiredVolume > 0 Then
            If .IO.HSCounter1 < TargetPulses Then Exit Select
          ElseIf RinseTime > 0 Then
            If Not Wait.Finished Then Exit Select
          ElseIf DesiredVolume > 0 Then
            If .IO.HSCounter1 < TargetPulses Then Exit Select
          End If
          Wait.TimeRemaining = .Parameters.OverflowTimeAfterRinse
          State = S12.WaitOverflowTime

        Case S12.WaitOverflowTime
          StateString = If(.Language = LanguageValue.ZhTW, "溢流排水中", "Overflowing") & "" & TimerString(Wait.TimeRemaining)
          If Not Wait.Finished Then Exit Select
          State = S12.Off

            Case S12.Pause
          StateString = If(.Language = LanguageValue.ZhTW, "暫停", "Paused") & " " & TimerString(Wait.TimeRemaining)
          If Not .Parent.IsPaused And .IO.MainPumpFB Then
            If WaterType = 3 And .Parameters.ReCycleWater = 1 Then
              CoolFill = True
            End If
            If StateWas = S12.WaitForRinseTime Then
              State = StateWas
              StateWas = S12.Off
              If RinseTimeWas = RinseTime Then
                Wait.Restart()
              Else
                Wait.TimeRemaining = RinseTime * 60
              End If
            Else
              State = StateWas
              StateWas = S12.Off
              Wait.Restart()
            End If
          End If

      End Select
    End With
  End Function

  Public Sub Cancel() Implements ACCommand.Cancel
    State = S12.Off
    CoolFill = False
    Wait.Cancel()
  End Sub

  Public Sub ParametersChanged(ByVal ParamArray param() As Integer) Implements ACCommand.ParametersChanged
    RinseTimeWas = RinseTime
    WaterType = MinMax(param(1), 0, 3)
    RinseTime = param(2)
    'DesiredVolume = MinMax(param(3), 0, 9999)
    DesiredVolume = 0
  End Sub


#Region "Standard Definitions"
  Private ReadOnly ControlCode As ControlCode
  Public Sub New(ByVal controlCode As ControlCode)
    Me.ControlCode = controlCode
  End Sub
  Friend ReadOnly Property IsOn() As Boolean Implements ACCommand.IsOn
    Get
      Return State <> S12.Off
    End Get
  End Property

  <EditorBrowsable(EditorBrowsableState.Advanced)> Private state_ As S12
  <EditorBrowsable(EditorBrowsableState.Advanced)> Private statewas_ As S12

  Public Property State() As S12
    Get
      Return state_
    End Get
    Private Set(ByVal value As S12)
      state_ = value
    End Set
  End Property
  Public Property StateWas() As S12
    Get
      Return statewas_
    End Get
    Private Set(ByVal value As S12)
      statewas_ = value
    End Set
  End Property
  Public ReadOnly Property IsFillHot() As Boolean
    Get
      Return (WaterType <> 0 And WaterType < 3) AndAlso (State = S12.WaitForRinseTime Or State = S12.WaitLowLevel)
    End Get
  End Property
  Public ReadOnly Property IsFillCold() As Boolean
    Get
      Return (WaterType <> 1 And WaterType < 3) AndAlso (State = S12.WaitForRinseTime Or State = S12.WaitLowLevel)
    End Get
  End Property
  Public ReadOnly Property IsCoolFill() As Boolean
    Get
      Return CoolFill AndAlso (State = S12.WaitForRinseTime Or State = S12.WaitLowLevel)
    End Get
  End Property
  Public ReadOnly Property IsOverFlow() As Boolean
    Get
      Return (State = S12.WaitForRinseTime) Or (State = S12.WaitOverflowTime) Or (State = S12.WaitForRinseLevel)
    End Get
  End Property
#End Region
End Class

#Region "Class Instance"
Partial Public Class ControlCode
  Public ReadOnly Command12 As New Command12(Me)
End Class
#End Region
